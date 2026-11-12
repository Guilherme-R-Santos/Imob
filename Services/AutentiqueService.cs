using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using Imob.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Imob.Services
{
    public class AutentiqueService
    {
        private const int TamanhoMaximoPdf = 20 * 1024 * 1024;

        public static IReadOnlyList<ClienteDAO> ObterSignatarios(ContratoDAO contrato)
        {
            ArgumentNullException.ThrowIfNull(contrato);

            if (contrato.Id <= 0 || !contrato.Ativo)
                throw new InvalidOperationException("É necessário um contrato cadastrado e ativo para enviar à assinatura.");
            if (!string.IsNullOrWhiteSpace(contrato.IdDocumentoAutentique) || contrato.EnvioAutentiqueEmAndamento)
                throw new InvalidOperationException("O contrato já possui documento ou há um envio pendente de conciliação.");
            if (string.IsNullOrWhiteSpace(contrato.Nome) || contrato.Nome.Trim().Length > 255 || contrato.Imovel == null || contrato.TipoContrato == null)
                throw new InvalidOperationException("O contrato retornado pela API não contém nome válido, imóvel ou tipo de contrato necessários para gerar o PDF.");

            var modalidadeId = contrato.ModalidadeContrato?.Id ?? contrato.ModalidadeContratoId;
            if (!modalidadeId.HasValue)
                throw new InvalidOperationException("A modalidade do contrato não foi retornada pela API. Não é possível gerar o PDF para assinatura.");
            if (modalidadeId == 3 && contrato.Fiador == null)
                throw new InvalidOperationException("A modalidade exige fiador, mas seus dados não foram retornados pela API.");
            if (modalidadeId != 3 && (contrato.Fiador != null || contrato.FiadorId.HasValue || contrato.Fiador2 != null || contrato.Fiador2Id.HasValue))
                throw new InvalidOperationException("O PDF desta modalidade não qualifica o fiador cadastrado. Revise o contrato antes de enviá-lo.");

            var signatarios = new List<ClienteDAO>();
            AdicionarSignatario(signatarios, "Proprietário", contrato.Proprietario, contrato.ProprietarioId, true);
            AdicionarSignatario(signatarios, "Contratante 1", contrato.Contratante1, contrato.Contratante1Id, true);
            AdicionarSignatario(signatarios, "Contratante 2", contrato.Contratante2, contrato.Contratante2Id, false);
            AdicionarSignatario(signatarios, "Contratante 3", contrato.Contratante3, contrato.Contratante3Id, false);
            AdicionarSignatario(signatarios, "Contratante 4", contrato.Contratante4, contrato.Contratante4Id, false);
            AdicionarSignatario(signatarios, "Fiador", contrato.Fiador, contrato.FiadorId, false);
            AdicionarSignatario(signatarios, "Fiador 2", contrato.Fiador2, contrato.Fiador2Id, false);

            var testemunhas = new[] { contrato.Testemunha1, contrato.Testemunha2, contrato.Testemunha3 }
                .Where(email => !string.IsNullOrWhiteSpace(email)).Select(email => email.Trim()).ToArray();
            if (testemunhas.Any(email => email.Length > 255 || !new EmailAddressAttribute().IsValid(email)) ||
                testemunhas.Distinct(StringComparer.OrdinalIgnoreCase).Count() != testemunhas.Length)
                throw new InvalidOperationException("Os e-mails das testemunhas devem ser válidos e distintos. Corrija o contrato antes de enviar.");
            if (signatarios.Any(s => testemunhas.Contains(s.Email.Trim(), StringComparer.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Uma testemunha possui o mesmo e-mail de uma parte do contrato. Corrija os dados antes do envio.");
            return signatarios;
        }

        private static void AdicionarSignatario(List<ClienteDAO> signatarios, string papel, ClienteDAO cliente, int? id, bool obrigatorio)
        {
            if (cliente == null)
            {
                if (obrigatorio || id.HasValue)
                    throw new InvalidOperationException($"Os dados de {papel} não foram retornados pela API. Não é possível enviar o contrato.");
                return;
            }

            if (id.HasValue && cliente.Id != id.Value)
                throw new InvalidOperationException($"Os dados de {papel} não correspondem ao contrato. Não é possível enviá-lo.");

            if (string.IsNullOrWhiteSpace(cliente.Nome) || string.IsNullOrWhiteSpace(cliente.Email) ||
                cliente.Email.Trim().Length > 255 || !new EmailAddressAttribute().IsValid(cliente.Email.Trim()))
                throw new InvalidOperationException($"Informe um nome e um e-mail válido de até 255 caracteres no cadastro de {papel} antes de enviar o contrato.");

            if (cliente.Id > 0 && signatarios.Any(s => s.Id == cliente.Id))
                return;

            if (signatarios.Any(s => string.Equals(s.Email.Trim(), cliente.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"O e-mail de {papel} também está cadastrado para outro signatário. Corrija os cadastros antes de enviar.");

            signatarios.Add(cliente);
        }

        public async Task<JObject> EnviarAsync(HttpClient client, ContratoDAO contrato, byte[] pdf, IReadOnlyList<ClienteDAO> signatarios)
        {
            ArgumentNullException.ThrowIfNull(client);
            ArgumentNullException.ThrowIfNull(pdf);
            var signatariosEsperados = ObterSignatarios(contrato);
            if (signatarios == null || signatarios.Count != signatariosEsperados.Count ||
                !signatarios.Select(s => s.Email.Trim()).SequenceEqual(signatariosEsperados.Select(s => s.Email.Trim()), StringComparer.OrdinalIgnoreCase))
                throw new InvalidOperationException("Os signatários não correspondem aos participantes do contrato.");
            if (pdf.Length < 5 || pdf.Length > TamanhoMaximoPdf || !pdf.AsSpan(0, 5).SequenceEqual("%PDF-"u8))
                throw new InvalidOperationException("O arquivo gerado não é um PDF válido de até 20 MiB.");

            var request = new
            {
                ContratoId = contrato.Id,
                Nome = contrato.Nome.Trim(),
                Arquivo = pdf,
                Signatarios = signatarios.Select(s => new { Email = s.Email.Trim(), Acao = "SIGN" }).ToArray()
            };
            using var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            string body;
            try
            {
                response = await client.PostAsync("Autentique/CriarDocumento", content);
                using (response)
                {
                    body = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                        throw CriarErroResposta(response, body);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Falha de comunicação com a API. Verifique o painel da Autentique e os logs da API antes de qualquer novo envio; o contrato pode estar bloqueado para conciliação.", ex);
            }
            catch (System.IO.IOException ex)
            {
                throw new InvalidOperationException("Falha de comunicação com a API. Verifique o painel da Autentique e os logs da API antes de qualquer novo envio; o contrato pode estar bloqueado para conciliação.", ex);
            }
            catch (OperationCanceledException ex)
            {
                throw new InvalidOperationException("Tempo limite ao chamar a API. Verifique o painel da Autentique e os logs da API antes de qualquer novo envio; o contrato pode estar bloqueado para conciliação.", ex);
            }

            try
            {
                var resultado = JObject.Parse(body);
                if (resultado["sandbox"]?.Type != JTokenType.Boolean || resultado["documento"] is not JObject documento ||
                    string.IsNullOrWhiteSpace(documento.Value<string>("id")))
                    throw new JsonException("A resposta não contém o ambiente e o identificador do documento criado.");
                return resultado;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("A API retornou uma resposta inesperada. Verifique o painel da Autentique e os logs da API antes de qualquer novo envio; o contrato pode estar bloqueado para conciliação.", ex);
            }
        }

        private static InvalidOperationException CriarErroResposta(HttpResponseMessage response, string body)
        {
            var codigo = (int)response.StatusCode;
            var explicacao = codigo switch
            {
                400 => "Dados inválidos ou testemunha repetida na lista de signatários.",
                404 => "Contrato ativo não encontrado.",
                409 => "O contrato já possui documento ou há envio pendente de conciliação.",
                503 => "Integração não configurada na API.",
                _ => "Falha ao criar documento."
            };
            var detalhe = string.IsNullOrWhiteSpace(body) ? string.Empty : $"\nDetalhes da API: {body}";
            var aviso = codigo >= 500 || codigo == 409 ? "\nVerifique o painel da Autentique e os logs da API antes de qualquer novo envio; o contrato pode estar bloqueado para conciliação." : string.Empty;
            return new InvalidOperationException($"API HTTP {codigo}: {explicacao}{detalhe}{aviso}");
        }

        public async Task<IReadOnlyList<DocumentoAutentique>> ObterDocumentosAsync(HttpClient client, int contratoId)
        {
            if (contratoId <= 0)
                throw new ArgumentOutOfRangeException(nameof(contratoId));
            using var response = await client.GetAsync($"Autentique/DocumentosDoContrato/{contratoId}");
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Não foi possível consultar os registros da API (HTTP {(int)response.StatusCode}): {body}");
            try
            {
                return JsonConvert.DeserializeObject<List<DocumentoAutentique>>(body)
                    ?? throw new JsonException("A lista de documentos está vazia na resposta.");
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("A API retornou registros de documentos inválidos.", ex);
            }
        }
    }
}
