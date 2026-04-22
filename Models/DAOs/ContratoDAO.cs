using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Imob.Models
{
    public class ContratoDAO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public decimal ValorContrato { get; set; }
        public int? TipoContratoId { get; set; }
        public int? ProprietarioId { get; set; }
        public int? Contratante1Id { get; set; }
        public int? Contratante2Id { get; set; }
        public int? Contratante3Id { get; set; }
        public int? Contratante4Id { get; set; }
        public int? FiadorId { get; set; }
        public int? ImovelId { get; set; }
        public int? ObjetoContratoId { get; set; }
        public int? ModalidadeContratoId { get; set; }

        public string NomeTipoContrato
        {
            get
            {
                return TipoContrato != null ? TipoContrato.Nome : string.Empty;
            }
        }

        public string NomeProprietario
        {
            get
            {
                return Proprietario != null ? Proprietario.Nome : string.Empty;
            }
        }

        public string NomeContratante1
        {
            get
            {
                return Contratante1 != null ? Contratante1.Nome : string.Empty;
            }
        }

        public string NomeContratante2
        {
            get
            {
                return Contratante2 != null ? Contratante2.Nome : string.Empty;
            }
        }

        public string NomeContratante3
        {
            get
            {
                return Contratante3 != null ? Contratante3.Nome : string.Empty;
            }
        }

        public string NomeContratante4
        {
            get
            {
                return Contratante4 != null ? Contratante4.Nome : string.Empty;
            }
        }

        public string NomeFiador
        {
            get
            {
                return Fiador != null ? Fiador.Nome : string.Empty;
            }
        }

        public string NomeImovel
        {
            get
            {
                return Imovel != null ? Imovel.Nome : string.Empty;
            }
        }

        public string NomeObjetoContrato
        {
            get
            {
                return ObjetoContrato != null ? ObjetoContrato.Nome : string.Empty;
            }
        }

        public string NomeModalidadeContrato
        {
            get
            {
                return ModalidadeContrato != null ? ModalidadeContrato.Nome : string.Empty;
            }
        }

        public UsuarioDAO Cadastrador { get; set; }
        public TipoContratoDAO TipoContrato { get; set; }
        public ClienteDAO Proprietario { get; set; }
        public ClienteDAO Contratante1 { get; set; }
        public ClienteDAO Contratante2 { get; set; }
        public ClienteDAO Contratante3 { get; set; }
        public ClienteDAO Contratante4 { get; set; }
        public ClienteDAO Fiador { get; set; }
        public ImovelDAO Imovel { get; set; }
        public ObjetoContratoDAO ObjetoContrato { get; set; }
        public ModalidadeContratoDAO ModalidadeContrato { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataInicioVigencia { get; set; }
        public int PrazoMeses { get; set; }
        public int Vencimento { get; set; }
        public DateTime? DataFimVigencia { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }
        public string PropostaSegFianca { get; set; }
        public string ApoliceSegFianca { get; set; }

        public static async Task<List<ContratoDAO>> GetContratos(HttpClient httpClient)
        {
            var response = await httpClient.GetAsync("Contrato/ObterTodos");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<ContratoDAO>>(json);
            }

            throw new Exception("Erro ao obter lista de contratos: " + response.StatusCode);
        }

        public static async Task<ContratoDAO> GetContratoPorId(int id, HttpClient httpClient)
        {
            var response = await httpClient.GetAsync($"Contrato/ObterPorId/{id}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ContratoDAO>(json);
            }

            throw new Exception("Erro ao obter contrato: " + response.StatusCode);
        }
    }
}
