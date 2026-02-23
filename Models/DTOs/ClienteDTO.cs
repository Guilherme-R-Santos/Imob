using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Imob.Models
{
    public class ClienteDTO
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

        public async Task<int> CadastrarCliente()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:7251/");

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var clienteJson = new
                {
                    Nome = this.Nome,
                    CpfCnpj = this.CpfCnpj,
                    Identidade = this.Identidade,
                    OrgaoExpedidor = this.OrgaoExpedidor,
                    Nacionalidade = this.Nacionalidade,
                    Naturalidade = this.Naturalidade,
                    EstadoCivil = this.EstadoCivil,
                    Profissao = this.Profissao,
                    Endereco = this.Endereco,
                    Agencia = this.Agencia,
                    Conta = this.Conta,
                    CodBanco = this.CodBanco,
                    Banco = this.Banco,
                    Email = this.Email,
                    Telefone = this.Telefone,
                    DataNascimento = this.DataNascimento,
                    TipoCliente = new { Id = this.TipoCliente.Id },
                    Cadastrador = new { Id = this.Cadastrador.Id }
                };

                var json = JsonConvert.SerializeObject(clienteJson);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("Cliente/Criar", content);

                if (!response.IsSuccessStatusCode)
                {
                    var respBody = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao cadastrar cliente: {response.StatusCode} - {respBody}");
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(responseBody))
                {
                    throw new Exception("Resposta vazia ao cadastrar cliente.");
                }

                try
                {
                    var obj = JObject.Parse(responseBody);
                    var idToken = obj.SelectToken("id") ?? obj.SelectToken("Id") ?? obj.SelectToken("data.id") ?? obj.SelectToken("data.Id");
                    if (idToken == null || !int.TryParse(idToken.ToString(), out var createdId))
                    {
                        throw new Exception("Não foi possível obter o ID do cliente criado na resposta da API.");
                    }

                    return createdId;
                }
                catch (JsonException ex)
                {
                    throw new Exception($"Falha ao interpretar resposta da API: {ex.Message}");
                }
            }
        }

        public async Task AtualizarCliente(int id)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:7251/");

                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var clienteJson = new
                {
                    Id = this.Id,
                    Nome = this.Nome,
                    CpfCnpj = this.CpfCnpj,
                    Identidade = this.Identidade,
                    OrgaoExpedidor = this.OrgaoExpedidor,
                    Nacionalidade = this.Nacionalidade,
                    Naturalidade = this.Naturalidade,
                    EstadoCivil = this.EstadoCivil,
                    Profissao = this.Profissao,
                    Endereco = this.Endereco,
                    Agencia = this.Agencia,
                    Conta = this.Conta,
                    CodBanco = this.CodBanco,
                    Banco = this.Banco,
                    Email = this.Email,
                    Telefone = this.Telefone,
                    DataNascimento = this.DataNascimento,
                    TipoCliente = new { Id = this.TipoCliente.Id },
                    Cadastrador = this.Cadastrador == null ? null : new { Id = this.Cadastrador.Id }
                };

                var json = JsonConvert.SerializeObject(clienteJson);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"Cliente/Atualizar/{id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var respBody = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao atualizar cliente: {response.StatusCode} - {respBody}");
                }
            }
        }

        public async Task InativarCliente(int id)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri("https://localhost:7251/");

                var content = new StringContent(string.Empty);
                var response = await httpClient.PutAsync($"Cliente/Inativar/{id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var respBody = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro ao inativar cliente: {response.StatusCode} - {respBody}");
                }
            }
        }

    }
}
