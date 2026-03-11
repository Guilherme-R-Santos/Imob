
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Imob.Models
{
    public class ModalidadeContratoDAO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public UsuarioDAO Cadastrador { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }

        public static async Task<List<ModalidadeContratoDAO>> GetModalidadesContrato(HttpClient httpClient)
        {
            var response = await httpClient.GetAsync("ModalidadeContrato/ObterTodos");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ModalidadeContratoDAO>>(json) ?? new List<ModalidadeContratoDAO>();
            }

            throw new Exception("Erro ao obter modalidades de contrato: " + response.StatusCode);
        }
    }
}
