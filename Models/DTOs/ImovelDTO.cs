using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Imob.Models
{
    public class ImovelDTO
    {
        public string Descricao { get; set; }
        public string Observacao { get; set; }
        public int Proprietario { get; set; }
        public int TipoImovel { get; set; }
        public int Intencao { get; set; }
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public int Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string? Complemento { get; set; }
        public string? InscricaoIptu { get; set; }
        public string? NumeroCbmerj { get; set; }
        public decimal Metragem { get; set; }
        public decimal Valor { get; set; }
        public decimal? Condominio { get; set; }
        public decimal? Iptu { get; set; }
        public decimal? TaxaIncendio { get; set; }
        public decimal? Foro { get; set; }
        public int Cadastrador { get; set; }

        public async Task<int> CadastrarImovel(HttpClient httpClient)
        {
            var imovelJson = new
            {
                Descricao = this.Descricao,
                Observacao = this.Observacao,
                Proprietario = new { Id = this.Proprietario },
                TipoImovel = new { Id = this.TipoImovel },
                Intencao = new { Id = this.Intencao },
                Cep = this.Cep,
                Logradouro = this.Logradouro,
                Numero = this.Numero,
                Bairro = this.Bairro,
                Cidade = this.Cidade,
                Estado = this.Estado,
                Pais = this.Pais,
                Complemento = this.Complemento,
                Metragem = this.Metragem,
                Valor = this.Valor,
                Condominio = this.Condominio,
                Iptu = this.Iptu,
                TaxaIncendio = this.TaxaIncendio,
                Foro = this.Foro,
                InscricaoIptu = this.InscricaoIptu,
                NumeroCbmerj = this.NumeroCbmerj,
                Cadastrador = new { Id = this.Cadastrador }
            };

            var json = JsonConvert.SerializeObject(imovelJson);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("Imovel/Criar", content);

            if (!response.IsSuccessStatusCode)
            {
                var respBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao cadastrar imóvel: {response.StatusCode} - {respBody}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(responseBody))
            {
                throw new Exception("Resposta vazia ao cadastrar imóvel.");
            }

            try
            {
                var obj = JObject.Parse(responseBody);
                var idToken = obj.SelectToken("id") ?? obj.SelectToken("Id") ?? obj.SelectToken("data.id") ?? obj.SelectToken("data.Id");
                if (idToken == null || !int.TryParse(idToken.ToString(), out var createdId))
                {
                    throw new Exception("Não foi possível obter o ID do imóvel criado na resposta da API.");
                }

                return createdId;
            }
            catch (JsonException ex)
            {
                throw new Exception($"Falha ao interpretar resposta da API: {ex.Message}");
            }
        }

        public async Task AtualizarImovel(int id, HttpClient httpClient)
        {
            var imovelJson = new
            {
                Descricao = this.Descricao,
                Observacao = this.Observacao,
                Proprietario = new { Id = this.Proprietario },
                TipoImovel = new { Id = this.TipoImovel },
                Intencao = new { Id = this.Intencao },
                Cep = this.Cep,
                Logradouro = this.Logradouro,
                Numero = this.Numero,
                Bairro = this.Bairro,
                Cidade = this.Cidade,
                Estado = this.Estado,
                Pais = this.Pais,
                Complemento = this.Complemento,
                Metragem = this.Metragem,
                Valor = this.Valor,
                Condominio = this.Condominio,
                Iptu = this.Iptu,
                TaxaIncendio = this.TaxaIncendio,
                Foro = this.Foro,
                InscricaoIptu = this.InscricaoIptu,
                NumeroCbmerj = this.NumeroCbmerj
            };

            var json = JsonConvert.SerializeObject(imovelJson);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"Imovel/Atualizar/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var respBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao atualizar imóvel: {response.StatusCode} - {respBody}");
            }
        }

        public async Task InativarImovel(int id, HttpClient httpClient)
        {
            var content = new StringContent(string.Empty);

            var response = await httpClient.PostAsync($"Imovel/Inativar/{id}", content);

            if (!response.IsSuccessStatusCode)
            {
                var respBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao excluir imóvel: {response.StatusCode} - {respBody}");
            }

        }

    }

}
