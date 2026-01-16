using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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

        public static List<IntencaoDAO> GetPorNome(string nome)
        {
            using var client = new HttpClient { BaseAddress = new Uri("https://localhost:7251/") };
            var response = client.GetAsync($"Intencao/ObterPorNome/{Uri.EscapeDataString(nome)}").Result;
            response.EnsureSuccessStatusCode();

            var content = response.Content.ReadAsStringAsync().Result;
            var token = Newtonsoft.Json.Linq.JToken.Parse(content);

            if (token.Type == Newtonsoft.Json.Linq.JTokenType.Array)
            {
                return token.ToObject<List<IntencaoDAO>>();
            }
            else if (token.Type == Newtonsoft.Json.Linq.JTokenType.Object)
            {
                var single = token.ToObject<IntencaoDAO>();
                return single == null ? new List<IntencaoDAO>() : new List<IntencaoDAO> { single };
            }
            else
            {
                return new List<IntencaoDAO>();
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

        public static async Task<List<IntencaoDAO>> GetPorNomeAsync(string nome)
        {
            using var client = new HttpClient { BaseAddress = new Uri("https://localhost:7251/") };
            var response = await client.GetAsync($"Intencao/ObterPorNome/{Uri.EscapeDataString(nome)}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var token = Newtonsoft.Json.Linq.JToken.Parse(content);

            if (token.Type == Newtonsoft.Json.Linq.JTokenType.Array)
            {
                return token.ToObject<List<IntencaoDAO>>();
            }
            else if (token.Type == Newtonsoft.Json.Linq.JTokenType.Object)
            {
                var single = token.ToObject<IntencaoDAO>();
                return single == null ? new List<IntencaoDAO>() : new List<IntencaoDAO> { single };
            }
            else
            {
                return new List<IntencaoDAO>();
            }
        }

        public static int GetIdPorNome(string nome)
        {
            var intecoes = GetPorNome(nome);
            return intecoes?.FirstOrDefault()?.Id ?? 0;
        }
    }
}
