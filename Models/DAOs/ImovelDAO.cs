using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Azure;
using Newtonsoft.Json;

namespace Imob.Models
{
    public class ImovelDAO
    {
        public int Id { get; set; }
        public bool Ativo { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string Observacao { get; set; }
        public ClienteDAO Proprietario { get; set; }
        public TipoImovelDAO TipoImovel { get; set; }
        public IntencaoDAO Intencao { get; set; }
        public string Cep { get; set; }
        public string Logradouro { get; set; }
        public int Numero { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string Pais { get; set; }
        public string ? Complemento { get; set; }
        public decimal Metragem { get; set; }
        public decimal Valor { get; set; }
        public decimal? Condominio { get; set; }
        public decimal? Iptu { get; set; }
        public decimal? TaxaIncendio { get; set; }
        public decimal? Foro { get; set; }
        public UsuarioDAO Cadastrador { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }

        public static List<ImovelDAO> GetImoveis()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7251/");

                var response = client.GetAsync("Imovel/ObterTodos").Result;

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<List<ImovelDAO>>(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    throw new Exception("Erro ao obter lista de imóveis: " + response.StatusCode);
                }
            }
        }
    }

}
