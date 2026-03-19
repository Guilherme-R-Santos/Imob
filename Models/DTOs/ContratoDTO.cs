using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Imob.Models
{
    public class ContratoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public UsuarioDAO Cadastrador { get; set; }
        public TipoContratoDAO TipoContrato { get; set; }
        public ClienteDAO Proprietario { get; set; }
        public ClienteDAO Contratante1 { get; set; }
        public ClienteDAO Contratante2 { get; set; }
        public ClienteDAO Contratante3 { get; set; }
        public ClienteDAO Contratante4 { get; set; }
        public ClienteDAO Fiador { get; set; }
        public ImovelDAO Imovel { get; set; }
        public ObjetoContratoDAO ObjetoContrato { get; set; }
        public ModalidadeContratoDAO ModalidadeContrato { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataInicioVigencia { get; set; }
        public int PrazoMeses { get; set; }
        public int Vencimento { get; set; }
        public DateTime? DataFimVigencia { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }
        public string PropostaSegFianca { get; set; }
        public string ApoliceSegFianca { get; set; }

        public async Task InativarContrato(int id, HttpClient httpClient)
        {
            var content = new StringContent(string.Empty);
            var response = await httpClient.PutAsync($"Contrato/Inativar/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var respBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao inativar contrato: {response.StatusCode} - {respBody}");
            }
        }

        public async Task<int> CadastrarContrato(HttpClient httpClient)
        {
            var contratoJson = new System.Collections.Generic.Dictionary<string, object>
            {
                { "Nome", this.Nome },
                { "Cadastrador", new { Id = this.Cadastrador.Id } },
                { "TipoContrato", new { Id = this.TipoContrato.Id } },
                { "ModalidadeContrato", new { Id = this.ModalidadeContrato.Id } },
                { "ObjetoContrato", new { Id = this.ObjetoContrato.Id } },
                { "Proprietario", new { Id = this.Proprietario.Id } },
                { "Imovel", new { Id = this.Imovel.Id } },
                { "Contratante1", new { Id = this.Contratante1.Id } },
                { "DataInicioVigencia", this.DataInicioVigencia },
                { "DataFimVigencia", this.DataInicioVigencia.HasValue ? this.DataInicioVigencia.Value.AddMonths(PrazoMeses) : (DateTime?)null },
                { "PrazoMeses", this.PrazoMeses },
                { "Vencimento", this.Vencimento },
                { "PropostaSegFianca", this.PropostaSegFianca },
                { "ApoliceSegFianca", this.ApoliceSegFianca }
            };

            if (this.Contratante2 != null)
            {
                contratoJson["Contratante2"] = new { Id = this.Contratante2.Id };
            }

            if (this.Contratante3 != null)
            {
                contratoJson["Contratante3"] = new { Id = this.Contratante3.Id };
            }

            if (this.Contratante4 != null)
            {
                contratoJson["Contratante4"] = new { Id = this.Contratante4.Id };
            }

            if (this.Fiador != null)
            {
                contratoJson["Fiador"] = new { Id = this.Fiador.Id };
            }

            var json = JsonConvert.SerializeObject(contratoJson);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("Contrato/Criar", content);

            if (!response.IsSuccessStatusCode)
            {
                var respBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao cadastrar contrato: {response.StatusCode} - {respBody}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(responseBody))
            {
                throw new Exception("Resposta vazia ao cadastrar contrato.");
            }

            try
            {
                var obj = JObject.Parse(responseBody);
                var idToken = obj.SelectToken("id") ?? obj.SelectToken("Id") ?? obj.SelectToken("data.id") ?? obj.SelectToken("data.Id");
                if (idToken == null || !int.TryParse(idToken.ToString(), out var createdId))
                {
                    throw new Exception("Não foi possível obter o ID do contrato criado na resposta da API.");
                }

                return createdId;
            }
            catch (JsonException ex)
            {
                throw new Exception($"Falha ao interpretar resposta da API: {ex.Message}");
            }
        }

        public async Task AtualizarContrato(int id, HttpClient httpClient)
        {
            var contratoJson = new System.Collections.Generic.Dictionary<string, object>
            {
                { "Id", this.Id },
                { "Nome", this.Nome },
                { "TipoContrato", new { Id = this.TipoContrato.Id } },
                { "ModalidadeContrato", new { Id = this.ModalidadeContrato.Id } },
                { "ObjetoContrato", new { Id = this.ObjetoContrato.Id } },
                { "Proprietario", new { Id = this.Proprietario.Id } },
                { "Imovel", new { Id = this.Imovel.Id } },
                { "Contratante1", new { Id = this.Contratante1.Id } },
                { "DataFimVigencia", this.DataInicioVigencia.HasValue ? this.DataInicioVigencia.Value.AddMonths(PrazoMeses) : (DateTime?)null },
                { "DataInicioVigencia", this.DataInicioVigencia },
                { "PrazoMeses", this.PrazoMeses },
                { "Vencimento", this.Vencimento },
                { "PropostaSegFianca", this.PropostaSegFianca },
                { "ApoliceSegFianca", this.ApoliceSegFianca }
            };

            if (this.Contratante2 != null)
            {
                contratoJson["Contratante2"] = new { Id = this.Contratante2.Id };
            }

            if (this.Contratante3 != null)
            {
                contratoJson["Contratante3"] = new { Id = this.Contratante3.Id };
            }

            if (this.Contratante4 != null)
            {
                contratoJson["Contratante4"] = new { Id = this.Contratante4.Id };
            }

            if (this.Fiador != null)
            {
                contratoJson["Fiador"] = new { Id = this.Fiador.Id };
            }

            var json = JsonConvert.SerializeObject(contratoJson);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"Contrato/Atualizar/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var respBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao atualizar contrato: {response.StatusCode} - {respBody}");
            }
        }

    }
}
