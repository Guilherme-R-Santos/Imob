using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Imob.Models
{
    public class IntencaoDAO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }
        public UsuarioDAO Cadastrador { get; set; }

        public static IntencaoDAO GetPorNome(string nome)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7251/");

                var response = client.GetAsync("Intencao/ObterPorNome/" + nome).Result;

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<IntencaoDAO>(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    throw new Exception("Erro ao obter informações do Tipo: " + response.StatusCode);
                }
            }
        }

        public static List<IntencaoDAO> GetIntencao()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7251/");

                var response = client.GetAsync("Intencao/ObterTodas").Result;

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<List<IntencaoDAO>>(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    throw new Exception("Erro ao obter lista de Intenções: " + response.StatusCode);
                }
            }
        }
    }
}
