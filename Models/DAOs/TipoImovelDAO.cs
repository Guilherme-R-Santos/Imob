using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Imob.Models
{
    public class TipoImovelDAO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }
        public UsuarioDAO Cadastrador { get; set; }

        public static List<TipoImovelDAO> GetPorNome(string nome, HttpClient httpClient)
        {
            var response = httpClient.GetAsync("TipoImovel/ObterPorNome/" + nome).Result;

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<TipoImovelDAO>>(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                throw new Exception("Erro ao obter informações do Tipo: " + response.StatusCode);
            }
        }

        public static List<TipoImovelDAO> GetTipoImovel(HttpClient httpClient)
        {
            var response = httpClient.GetAsync("TipoImovel/ObterTodos").Result;

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<TipoImovelDAO>>(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                throw new Exception("Erro ao obter lista de Intenções: " + response.StatusCode);
            }
        }

        public static int GetIdPorNome(string nome, HttpClient httpClient)
        {
            var tipoImovel = GetPorNome(nome, httpClient);
            return tipoImovel?.FirstOrDefault()?.Id ?? 0;
        }
    }
}
