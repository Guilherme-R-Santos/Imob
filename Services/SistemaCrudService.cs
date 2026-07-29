using Imob.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Imob.Services
{
    public class SistemaCrudService : ISistemaCrudService
    {
        private readonly HttpClient _httpClient;

        public SistemaCrudService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> CadastrarImovelAsync(ImovelDTO dto)
        {
            return await dto.CadastrarImovel(_httpClient);
        }

        public async Task AtualizarImovelAsync(int id, ImovelDTO dto)
        {
            await dto.AtualizarImovel(id, _httpClient);
        }

        public async Task InativarImovelAsync(int id)
        {
            var dto = new ImovelDTO();
            await dto.InativarImovel(id, _httpClient);
        }

        public async Task<int> CadastrarContratoAsync(ContratoDTO dto)
        {
            return await dto.CadastrarContrato(_httpClient);
        }

        public async Task AtualizarContratoAsync(int id, ContratoDTO dto)
        {
            await dto.AtualizarContrato(id, _httpClient);
        }

        public async Task InativarContratoAsync(int id)
        {
            var dto = new ContratoDTO();
            await dto.InativarContrato(id, _httpClient);
        }

        public async Task<int> CadastrarClienteAsync(ClienteDTO dto)
        {
            return await dto.CadastrarCliente(_httpClient);
        }

        public async Task AtualizarClienteAsync(int id, ClienteDTO dto)
        {
            await dto.AtualizarCliente(id, _httpClient);
        }

        public async Task InativarClienteAsync(int id)
        {
            var dto = new ClienteDTO();
            await dto.InativarCliente(id, _httpClient);
        }

        public int ObterTipoClienteId(string nomeTipoCliente)
        {
            return TipoClienteDAO.GetIdPorNome(nomeTipoCliente, _httpClient);
        }

        public async Task CadastrarFotoAsync(FotoDTO dto)
        {
            await dto.CadastrarFoto(_httpClient);
        }

        public async Task InativarFotoAsync(int idFoto)
        {
            var dto = new FotoDTO();
            await dto.InativarFoto(idFoto, _httpClient);
        }
    }
}
