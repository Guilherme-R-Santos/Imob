using System.Threading.Tasks;
using Imob.Models;

namespace Imob.Services
{
    public interface ISistemaCrudService
    {
        Task<int> CadastrarImovelAsync(ImovelDTO dto);
        Task AtualizarImovelAsync(int id, ImovelDTO dto);
        Task InativarImovelAsync(int id);

        Task<int> CadastrarContratoAsync(ContratoDTO dto);
        Task AtualizarContratoAsync(int id, ContratoDTO dto);
        Task InativarContratoAsync(int id);

        Task<int> CadastrarClienteAsync(ClienteDTO dto);
        Task AtualizarClienteAsync(int id, ClienteDTO dto);
        Task InativarClienteAsync(int id);
        int ObterTipoClienteId(string nomeTipoCliente);

        Task CadastrarFotoAsync(FotoDTO dto);
        Task InativarFotoAsync(int idFoto);
    }
}
