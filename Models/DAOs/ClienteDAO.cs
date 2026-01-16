using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Imob.Models
{
    public class ClienteDAO
    {
        public int Id { get; set; }
        public TipoClienteDAO TipoCliente { get; set; }
        public UsuarioDAO Cadastrador { get; set; }
        public string Nome { get; set; }
        public string CpfCnpj { get; set; }
        public string Identidade { get; set; }
        public string OrgaoExpedidor { get; set; }
        public string Nacionalidade { get; set; }
        public string Naturalidade { get; set; }
        public string EstadoCivil { get; set; }
        public string Profissao { get; set; }
        public string Endereco { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public string CodBanco { get; set; }
        public string Banco { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataNascimento { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }

        public static List<ClienteDAO> GetPorNome(string nome)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7251/");

                var response = client.GetAsync("Cliente/ObterPorNome/" + nome).Result;

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<List<ClienteDAO>>(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    throw new Exception("Erro ao obter informações do Tipo do imóvel: " + response.StatusCode);
                }
            }
        }

        public static int GetIdPorNome(string nome)
        {
            var clientes = GetPorNome(nome);
            return clientes?.FirstOrDefault()?.Id ?? 0;
        }

        public static List<ClienteDAO> GetClientes()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7251/");

                var response = client.GetAsync("Cliente/ObterTodos").Result;

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<List<ClienteDAO>>(response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    throw new Exception("Erro ao obter lista de Clientes: " + response.StatusCode);
                }
            }
        }
    }
}
