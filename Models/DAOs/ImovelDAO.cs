using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows.Media.Animation;
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
        public string NomeProprietario
        {
            get
            {
                return Proprietario != null ? Proprietario.Nome : string.Empty;
            }
        }
        public TipoImovelDAO TipoImovel { get; set; }
        public string NomeTipoImovel
        {
            get
            {
                return TipoImovel != null ? TipoImovel.Nome : string.Empty;
            }
        }
        public IntencaoDAO Intencao { get; set; }
        public string NomeIntencao
        {
            get
            {
                return Intencao != null ? Intencao.Nome : string.Empty;
            }
        }
        public Imob.Models.DAOs.FinalidadeDAO Finalidade { get; set; }
        public string NomeFinalidade
        {
            get
            {
                return Finalidade != null ? Finalidade.Nome : string.Empty;
            }
        }
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
        public decimal? ValorLocacao { get; set; }
        public decimal? ValorVenda { get; set; }
        public decimal? Condominio { get; set; }
        public decimal? Iptu { get; set; }
        public decimal? TaxaIncendio { get; set; }
        public decimal? Foro { get; set; }
        public UsuarioDAO Cadastrador { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }

        public static async Task<List<ImovelDAO>> GetImoveis(HttpClient httpClient)
        {
            var response = await httpClient.GetAsync("Imovel/ObterTodos");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ImovelDAO>>(json);
            }
            else
            {
                throw new Exception("Erro ao obter lista de imóveis: " + response.StatusCode);
            }
        }

        public static async Task<ImovelDAO> GetImovelPorId(int id, HttpClient httpClient)
        {
            var response = await httpClient.GetAsync($"Imovel/ObterPorId/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ImovelDAO>(json);
            }
            else
            {
                throw new Exception("Erro ao obter imóvel: " + response.StatusCode);
            }
        }
    }
}
