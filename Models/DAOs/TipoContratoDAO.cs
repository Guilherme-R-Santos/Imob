using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Imob.Models
{
    public class TipoContratoDAO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public UsuarioDAO Cadastrador { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }

        public static async Task<List<TipoContratoDAO>> GetTiposContrato(HttpClient httpClient)
        {
            var response = await httpClient.GetAsync("TipoContrato/ObterTodos");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<TipoContratoDAO>>(json) ?? new List<TipoContratoDAO>();
            }

            throw new Exception("Erro ao obter tipos de contrato: " + response.StatusCode);
        }
    }
}
