using Imob.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Imob.Services
{
    public interface ISistemaListagemService
    {
        Task<IReadOnlyList<ImovelDAO>> ObterImoveisAsync();
        Task<IReadOnlyList<ClienteDAO>> ObterProprietariosAsync();
        Task<IReadOnlyList<ClienteDAO>> ObterLocatariosAsync();
        Task<IReadOnlyList<ClienteDAO>> ObterFiadoresAsync();
        Task<IReadOnlyList<ContratoDAO>> ObterContratosAsync();
        Task<IReadOnlyList<FotoDAO>> ObterFotosPorImovelAsync(int imovelId);
        Task<IReadOnlyList<IntencaoDAO>> ObterIntencoesAsync();
        Task<IReadOnlyList<TipoImovelDAO>> ObterTiposImovelAsync();
        Task<IReadOnlyList<Imob.Models.DAOs.FinalidadeDAO>> ObterFinalidadesAsync();
    }
}
