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
    }
}
