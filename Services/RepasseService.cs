using System.Net.Http;
using System.Text;
using Imob.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Imob.Services
{
    public sealed class RegistroRepasse
    {
        [JsonIgnore] public long Id { get; set; }
        public int CobrancaId { get; set; }
        public int Destinatario { get; set; }
        public string Origem { get; set; }
        public decimal Valor { get; set; }
        [JsonIgnore] public string ReferenciaExterna { get; set; }
        [JsonIgnore] public string IdTransferenciaAsaas { get; set; }
        public string Status { get; set; }
        public string OrigemDescricao => Origem switch { "AUTOMATICO" => "Automático", "MANUAL" => "Solicitado manualmente", _ => "Registrado" };
        public string DestinatarioDescricao => $"Destino {Destinatario}";
        [JsonIgnore] public string ComprovanteDescricao => string.IsNullOrWhiteSpace(ComprovanteUrl) ? "—" : "Disponível";
        [JsonIgnore] public string CriadoEmLocal => CriadoEmUtc.ToLocalTime().ToString("g");
        [JsonIgnore] public string ObservacaoAmigavel => string.IsNullOrWhiteSpace(Erro) ? "—" :
            Erro.Contains("estorno", StringComparison.OrdinalIgnoreCase) || Erro.Contains("contestação", StringComparison.OrdinalIgnoreCase) ? "Esta cobrança precisa de análise antes do envio." :
            Erro.Contains("saldo", StringComparison.OrdinalIgnoreCase) || Erro.Contains("líquido", StringComparison.OrdinalIgnoreCase) || Erro.Contains("webhook", StringComparison.OrdinalIgnoreCase) ? "Aguardando confirmação dos dados do pagamento." :
            "Peça ajuda ao suporte para verificar esta etapa.";
        public string SituacaoDescricao => Status switch
        {
            "NA_FILA" => "Aguardando envio",
            "ENVIANDO" => "Envio em andamento",
            "INCERTO" => "Confirme o envio antes de agir",
            "BLOQUEADO" => "Aguardando correção",
            "REJEITADO" => "Não foi possível enviar",
            "PENDING" => "Aguardando confirmação",
            "SCHEDULED" => "Programado",
            "BANK_PROCESSING" => "Em processamento",
            "BLOCKED" => "Pendente de análise",
            "DONE" => "Concluído",
            "FAILED" => "Não concluído",
            "CANCELLED" => "Cancelado",
            _ => "Em atualização"
        };
        public string Erro { get; set; }
        public string ComprovanteUrl { get; set; }
        public DateTime CriadoEmUtc { get; set; }
        public DateTime AtualizadoEmUtc { get; set; }
    }

    public sealed class RepasseApiException : InvalidOperationException
    {
        public System.Net.HttpStatusCode StatusCode { get; }

        public RepasseApiException(System.Net.HttpStatusCode statusCode, string mensagem) : base(mensagem) => StatusCode = statusCode;
    }

    public sealed class RepasseService
    {
        private readonly HttpClient _client;

        public RepasseService(HttpClient client) => _client = client ?? throw new ArgumentNullException(nameof(client));

        public async Task<bool> VerificarAdministradorAsync(CancellationToken cancellationToken = default)
        {
            using var response = await _client.GetAsync("Repasse/PorCobranca/0", cancellationToken);
            var body = await LerRespostaAsync(response, cancellationToken);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
                throw new InvalidOperationException($"Resposta inesperada ao verificar permissão: HTTP {(int)response.StatusCode}.");
            try
            {
                JArray.Parse(body);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Resposta inválida ao verificar permissão de administrador na API.", ex);
            }
            return true;
        }

        public async Task<IReadOnlyList<RegistroRepasse>> PorCobrancaAsync(int cobrancaId, CancellationToken cancellationToken = default)
        {
            ValidarId(cobrancaId, nameof(cobrancaId));
            using var response = await _client.GetAsync($"Repasse/PorCobranca/{cobrancaId}", cancellationToken);
            var body = await LerRespostaAsync(response, cancellationToken);
            try
            {
                return JsonConvert.DeserializeObject<List<RegistroRepasse>>(body)
                    ?? throw new JsonException("A lista retornada é nula.");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("A API retornou uma lista de repasses inválida.", ex);
            }
        }

        public async Task<ContaRepasse> DadosProprietarioAsync(int id, bool contrato, CancellationToken cancellationToken = default)
        {
            ValidarId(id, nameof(id));
            var rota = contrato ? "DadosProprietarioParaRepasse" : "DadosClienteParaRepasse";
            using var response = await _client.GetAsync($"Contrato/{rota}/{id}", cancellationToken);
            var body = await LerRespostaAsync(response, cancellationToken);
            try
            {
                return JsonConvert.DeserializeObject<ContaRepasse>(body) ?? throw new JsonException("Dados do proprietário ausentes.");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("A API retornou dados inválidos para o proprietário.", ex);
            }
        }

        public async Task ManualAsync(int cobrancaId, ContaRepasse conta1, ContaRepasse conta2, CancellationToken cancellationToken = default)
        {
            ValidarId(cobrancaId, nameof(cobrancaId));
            if (conta1 == null && conta2 == null) throw new ArgumentException("Informe ao menos um destinatário.");
            using var content = new StringContent(JsonConvert.SerializeObject(new { cobrancaId, contaRepasse1 = conta1, contaRepasse2 = conta2 }), Encoding.UTF8, "application/json");
            using var response = await _client.PostAsync("Repasse/Manual", content, cancellationToken);
            var body = await LerRespostaAsync(response, cancellationToken);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                throw new InvalidOperationException($"Resposta inesperada da API (HTTP {(int)response.StatusCode}): {body}. Consulte os registros antes de repetir.");
        }

        public async Task<string> ConciliarAsync(long id, string idTransferenciaAsaas, CancellationToken cancellationToken = default)
        {
            ValidarId(id, nameof(id));
            if (string.IsNullOrWhiteSpace(idTransferenciaAsaas) || idTransferenciaAsaas.Length > 128)
                throw new ArgumentException("Informe o ID da transferência no Asaas, com até 128 caracteres.", nameof(idTransferenciaAsaas));
            using var content = new StringContent(JsonConvert.SerializeObject(new { idTransferenciaAsaas }), Encoding.UTF8, "application/json");
            using var response = await _client.PostAsync($"Repasse/Conciliar/{id}", content, cancellationToken);
            return await LerRespostaAsync(response, cancellationToken);
        }

        public async Task<string> RetomarNaoEnviadosAsync(int cobrancaId, CancellationToken cancellationToken = default)
        {
            ValidarId(cobrancaId, nameof(cobrancaId));
            using var response = await _client.PostAsync($"Repasse/RetomarNaoEnviados/{cobrancaId}", null, cancellationToken);
            return await LerRespostaAsync(response, cancellationToken);
        }

        private static void ValidarId(long id, string nome)
        {
            if (id <= 0) throw new ArgumentOutOfRangeException(nome);
        }

        private static async Task<string> LerRespostaAsync(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (response.IsSuccessStatusCode) return body;
            var detalhe = body;
            try
            {
                var json = JToken.Parse(body);
                detalhe = json.Type == JTokenType.String ? json.Value<string>() : json.ToString(Formatting.Indented);
            }
            catch (JsonException) { }
            if (string.IsNullOrWhiteSpace(detalhe))
                detalhe = response.StatusCode switch
                {
                    System.Net.HttpStatusCode.Unauthorized => "Autenticação necessária ou sessão expirada.",
                    System.Net.HttpStatusCode.Forbidden => "A API negou a operação; é necessário ser administrador.",
                    System.Net.HttpStatusCode.NotFound => "Registro não encontrado.",
                    _ => "A API não forneceu detalhes."
                };
            throw new RepasseApiException(response.StatusCode, $"API HTTP {(int)response.StatusCode}: {detalhe}");
        }
    }
}
