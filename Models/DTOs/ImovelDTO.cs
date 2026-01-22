using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Newtonsoft.Json;

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
        public decimal Metragem { get; set; }
        public decimal Valor { get; set; }
        public decimal? Condominio { get; set; }
        public decimal? Iptu { get; set; }
        public decimal? TaxaIncendio { get; set; }
        public decimal? Foro { get; set; }
        public int Cadastrador { get; set; }

        public async Task CadastrarImovel()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:7251/");

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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
            }
        }

        public async Task AtualizarImovel(int id)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:7251/");

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

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
        }

        public async Task InativarImovel(int id)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:7251/");

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

}
