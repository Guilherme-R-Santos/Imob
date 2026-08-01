using Imob.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Imob.Services
{
    public class SistemaListagemService : ISistemaListagemService
    {
        private readonly HttpClient _httpClient;

        public SistemaListagemService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<ImovelDAO>> ObterImoveisAsync()
        {
            return (await ImovelDAO.GetImoveis(_httpClient)).ToList();
        }

        public async Task<IReadOnlyList<ClienteDAO>> ObterProprietariosAsync()
        {
            return (await ClienteDAO.GetProprietarios(_httpClient)).ToList();
        }

        public async Task<IReadOnlyList<ClienteDAO>> ObterLocatariosAsync()
        {
            return (await ClienteDAO.GetLocatários(_httpClient)).ToList();
        }

        public async Task<IReadOnlyList<ClienteDAO>> ObterFiadoresAsync()
        {
            return (await ClienteDAO.GetFiadores(_httpClient)).ToList();
        }

        public async Task<IReadOnlyList<ContratoDAO>> ObterContratosAsync()
        {
            return (await ContratoDAO.GetContratos(_httpClient)).ToList();
        }

        public async Task<IReadOnlyList<TipoContratoDAO>> ObterTiposContratoAsync()
        {
            return (await TipoContratoDAO.GetTiposContrato(_httpClient)).ToList();
        }

        public async Task<IReadOnlyList<ModalidadeContratoDAO>> ObterModalidadesContratoAsync()
        {
            return (await ModalidadeContratoDAO.GetModalidadesContrato(_httpClient)).ToList();
        }

        public async Task<IReadOnlyList<ObjetoContratoDAO>> ObterObjetosContratoAsync()
        {
            return (await ObjetoContratoDAO.GetObjetosContrato(_httpClient)).ToList();
        }

        public async Task<IReadOnlyList<FotoDAO>> ObterFotosPorImovelAsync(int imovelId)
        {
            return (await FotoDAO.GetFotosPorImovel(imovelId, _httpClient)).ToList();
        }

        public async Task<IReadOnlyList<IntencaoDAO>> ObterIntencoesAsync()
        {
            return await Task.Run(() => (IReadOnlyList<IntencaoDAO>)IntencaoDAO.GetIntencao(_httpClient));
        }

        public async Task<IReadOnlyList<TipoImovelDAO>> ObterTiposImovelAsync()
        {
            return await Task.Run(() => (IReadOnlyList<TipoImovelDAO>)TipoImovelDAO.GetTipoImovel(_httpClient));
        }

        public async Task<IReadOnlyList<Imob.Models.DAOs.FinalidadeDAO>> ObterFinalidadesAsync()
        {
            return await Task.Run(() => (IReadOnlyList<Imob.Models.DAOs.FinalidadeDAO>)Imob.Models.DAOs.FinalidadeDAO.GetFinalidades(_httpClient));
        }
    }
}
