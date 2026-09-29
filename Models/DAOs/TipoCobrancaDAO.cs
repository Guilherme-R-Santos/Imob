using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Imob.Models
{
    public class TipoCobrancaDAO
    {
        public int Id { get; set; }
        public UsuarioDAO Cadastrador { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }

        public static async Task<List<TipoCobrancaDAO>> GetTiposCobranca(HttpClient httpClient)
        {
            var response = await httpClient.GetAsync("TipoCobranca/ListarTipos");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<TipoCobrancaDAO>>(json) ?? new List<TipoCobrancaDAO>();
            }

            throw new Exception("Erro ao obter tipos de cobrança: " + response.StatusCode);
        }

        public static async Task<TipoCobrancaDAO> GetTipoCobrancaPorId(int id, HttpClient httpClient)
        {
            var response = await httpClient.GetAsync($"TipoCobranca/ObterTipo/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TipoCobrancaDAO>(json);
            }

            throw new Exception("Erro ao obter tipo de cobrança: " + response.StatusCode);
        }
    }
}
