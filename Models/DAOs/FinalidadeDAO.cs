using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Imob.Models.DAOs
{
    public class FinalidadeDAO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }
        public UsuarioDAO Cadastrador { get; set; }

        public static List<FinalidadeDAO> GetPorNome(string nome, HttpClient httpClient)
        {
            var response = httpClient.GetAsync("Finalidade/ObterPorNome/" + nome).Result;

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<FinalidadeDAO>>(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                throw new Exception("Erro ao obter informações do Finalidade: " + response.StatusCode);
            }
        }

        public static List<FinalidadeDAO> GetFinalidades(HttpClient httpClient)
        {
            var response = httpClient.GetAsync("Finalidade/ObterTodos").Result;

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<List<FinalidadeDAO>>(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                throw new Exception("Erro ao obter lista de Finalidades: " + response.StatusCode);
            }
        }

        public static int GetIdPorNome(string nome, HttpClient httpClient)
        {
            var finalidades = GetPorNome(nome, httpClient);
            return finalidades?.FirstOrDefault()?.Id ?? 0;
        }
    }
}
