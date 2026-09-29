using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Imob.Models
{
    public class CobrancaDTO
    {
        public int Id { get; set; }
        public string IdCobrancaAsaas { get; set; }
        public UsuarioDAO Cadastrador { get; set; }
        public ContratoDAO Contrato { get; set; }
        public TipoCobrancaDAO TipoCobranca { get; set; }
        public string Status { get; set; }
        public string LinkBoleto { get; set; }
        public string NossoNumero { get; set; }
        public bool Pago { get; set; }
        public DateTime? DataPagamento { get; set; }
        public bool ComprovanteEnviado { get; set; }
        public double Valor { get; set; }
        public double valorLiquido { get; set; }
        public string Nome { get; set; }
        public DateTime? Vencimento { get; set; }
        public bool PartilhaAutomatica { get; set; }
        public int ContaPartilha { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }
        public bool Ativo { get; set; }
        public bool SincronizadoAsaas { get; set; } = true;
        public string ErroSincronizacaoAsaas { get; set; }

        public async Task<int> CadastrarCobranca(HttpClient httpClient)
        {
            var cobrancaJson = new
            {
                Nome = this.Nome,
                Valor = this.Valor,
                Vencimento = this.Vencimento,
                PartilhaAutomatica = this.PartilhaAutomatica,
                ContaPartilha = this.ContaPartilha,
                ComprovanteEnviado = this.ComprovanteEnviado,
                Cadastrador = new { Id = this.Cadastrador.Id },
                Contrato = new { Id = this.Contrato.Id },
                TipoCobranca = new { Id = this.TipoCobranca.Id }
            };

            var json = JsonConvert.SerializeObject(cobrancaJson);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("Cobranca/Criar", content);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erro ao cadastrar cobrança: {response.StatusCode} - {responseBody}");
            }

            if (string.IsNullOrWhiteSpace(responseBody))
            {
                throw new Exception("Resposta vazia ao cadastrar cobrança.");
            }

            try
            {
                var obj = JObject.Parse(responseBody);
                var idToken = obj.SelectToken("id") ?? obj.SelectToken("Id")
                    ?? obj.SelectToken("cobranca.id") ?? obj.SelectToken("cobranca.Id")
                    ?? obj.SelectToken("data.id") ?? obj.SelectToken("data.Id");

                if (idToken == null || !int.TryParse(idToken.ToString(), out var createdId))
                {
                    throw new Exception("Não foi possível obter o ID da cobrança criada na resposta da API.");
                }

                return createdId;
            }
            catch (JsonException ex)
            {
                throw new Exception($"Falha ao interpretar resposta da API: {ex.Message}");
            }
        }

        public async Task AtualizarCobranca(int id, HttpClient httpClient)
        {
            var cobrancaJson = new
            {
                Id = this.Id,
                Nome = this.Nome,
                Valor = this.Valor,
                Vencimento = this.Vencimento,
                PartilhaAutomatica = this.PartilhaAutomatica,
                ContaPartilha = this.ContaPartilha,
                ComprovanteEnviado = this.ComprovanteEnviado,
                Contrato = new { Id = this.Contrato.Id },
                TipoCobranca = new { Id = this.TipoCobranca.Id }
            };

            var json = JsonConvert.SerializeObject(cobrancaJson);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"Cobranca/Atualizar/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var respBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao atualizar cobrança: {response.StatusCode} - {respBody}");
            }
        }

        public async Task InativarCobranca(int id, HttpClient httpClient)
        {
            var content = new StringContent(string.Empty);
            var response = await httpClient.PutAsync($"Cobranca/Inativar/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var respBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao inativar cobrança: {response.StatusCode} - {respBody}");
            }
        }

        public async Task AtivarCobranca(int id, HttpClient httpClient)
        {
            var content = new StringContent(string.Empty);
            var response = await httpClient.PutAsync($"Cobranca/Ativar/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var respBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao ativar cobrança: {response.StatusCode} - {respBody}");
            }
        }

        public async Task RetentarSincronizacaoAsaas(int id, HttpClient httpClient)
        {
            var content = new StringContent(string.Empty);
            var response = await httpClient.PostAsync($"Cobranca/RetentarSincronizacaoAsaas/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var respBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao retentar sincronização com o Asaas: {response.StatusCode} - {respBody}");
            }
        }
    }
}
