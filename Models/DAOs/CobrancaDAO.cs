using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Imob.Models
{
    public class CobrancaDAO
    {
        public int Id { get; set; }
        public string IdCobrancaAsaas { get; set; }
        public UsuarioDAO Cadastrador { get; set; }
        public ContratoDAO Contrato { get; set; }
        public TipoCobrancaDAO TipoCobranca { get; set; }
        public string Status { get; set; }
        public string LinkBoleto { get; set; }
        public string NossoNumero { get; set; }
        public bool Pago { get; set; }
        public DateTime? DataPagamento { get; set; }
        public bool ComprovanteEnviado { get; set; }
        public double Valor { get; set; }
        public double valorLiquido { get; set; }
        public string Nome { get; set; }
        public DateTime? Vencimento { get; set; }
        public bool PartilhaAutomatica { get; set; }
        public int ContaPartilha { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }
        public bool Ativo { get; set; }
        public bool SincronizadoAsaas { get; set; } = true;
        public string ErroSincronizacaoAsaas { get; set; }

        public string NomeContrato
        {
            get
            {
                return Contrato != null ? Contrato.Nome : string.Empty;
            }
        }

        public string NomeTipoCobranca
        {
            get
            {
                return TipoCobranca != null ? TipoCobranca.Nome : string.Empty;
            }
        }

        public static async Task<List<CobrancaDAO>> GetCobrancas(HttpClient httpClient)
        {
            var response = await httpClient.GetAsync("Cobranca/ObterTodos");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<CobrancaDAO>>(json) ?? new List<CobrancaDAO>();
            }

            throw new Exception("Erro ao obter lista de cobranças: " + response.StatusCode);
        }

        public static async Task<CobrancaDAO> GetCobrancaPorId(int id, HttpClient httpClient)
        {
            var response = await httpClient.GetAsync($"Cobranca/ObterPorId/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<CobrancaDAO>(json);
            }

            throw new Exception("Erro ao obter cobrança: " + response.StatusCode);
        }

        public static async Task<List<CobrancaDAO>> GetCobrancasPorContrato(int contratoId, HttpClient httpClient)
        {
            var response = await httpClient.GetAsync($"Cobranca/ObterPorContrato/{contratoId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<CobrancaDAO>>(json) ?? new List<CobrancaDAO>();
            }

            throw new Exception("Erro ao obter cobranças do contrato: " + response.StatusCode);
        }
    }
}
