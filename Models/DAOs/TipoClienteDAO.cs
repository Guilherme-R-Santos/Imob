using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;

namespace Imob.Models
{
    public class TipoClienteDAO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }
        public UsuarioDAO Cadastrador { get; set; }

        public static List<TipoClienteDAO> GetTodos(HttpClient httpClient)
        {
            var response = httpClient.GetAsync("TipoCliente/ListarTipos").Result;
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<TipoClienteDAO>>(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                throw new Exception("Erro ao obter informações do Tipo do imóvel: " + response.StatusCode);
            }
        }
        public static int GetIdPorNome(string nome, HttpClient httpClient)
        {
            var tipos = GetTodos(httpClient);
            return tipos?.FirstOrDefault(t => t.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
        }
    }
}

    
