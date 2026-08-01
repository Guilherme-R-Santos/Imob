using Imob.Models;
using Imob.Services;
using Imob.Services.Pdf;
using Imob.ViewModels.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Imob.ViewModels
{
    public class SistemaViewModel : INotifyPropertyChanged
    {
        private readonly ISistemaListagemService _listagemService;
        private readonly ISistemaCrudService _crudService;

        private ObservableCollection<ImovelDAO> _imoveis = new ObservableCollection<ImovelDAO>();
        private ObservableCollection<ClienteDAO> _proprietarios = new ObservableCollection<ClienteDAO>();
        private ObservableCollection<ClienteDAO> _locatarios = new ObservableCollection<ClienteDAO>();
        private ObservableCollection<ClienteDAO> _fiadores = new ObservableCollection<ClienteDAO>();
        private ObservableCollection<ContratoDAO> _contratos = new ObservableCollection<ContratoDAO>();

        private Visibility _proprietariosPanelVisibility = Visibility.Hidden;
        private Visibility _locatariosPanelVisibility = Visibility.Hidden;
        private Visibility _fiadoresPanelVisibility = Visibility.Hidden;
        private Visibility _imoveisPanelVisibility = Visibility.Hidden;
        private Visibility _contratosPanelVisibility = Visibility.Hidden;
        private Visibility _vistoriasPanelVisibility = Visibility.Hidden;

        private Visibility _contratoModalCriarVisibility = Visibility.Hidden;
        private Visibility _contratoModalVisualizarVisibility = Visibility.Hidden;
        private Visibility _proprietarioModalCriarVisibility = Visibility.Hidden;
        private Visibility _proprietarioModalEditarVisibility = Visibility.Hidden;
        private Visibility _locatarioModalCriarVisibility = Visibility.Hidden;
        private Visibility _locatarioModalEditarVisibility = Visibility.Hidden;
        private Visibility _fiadorModalCriarVisibility = Visibility.Hidden;
        private Visibility _fiadorModalEditarVisibility = Visibility.Hidden;
        private Visibility _imovelModalCriarVisibility = Visibility.Hidden;
        private Visibility _imovelModalEditarVisibility = Visibility.Hidden;
        private Visibility _imovelFotosModalCriarVisibility = Visibility.Hidden;
        private Visibility _imovelFotosModalEditarVisibility = Visibility.Hidden;

        private int? _proprietarioEditarId;
        private int? _locatarioEditarId;
        private int? _fiadorEditarId;
        private ImovelDAO _imovelSelecionado;
        private ContratoDAO _contratoSelecionado;
        private int _idImovelCadastrado;

        private readonly List<byte[]> _fotosSelecionadasBinario = new List<byte[]>();
        private readonly List<int?> _fotoIdsPreview = new List<int?>();
        private readonly List<int> _fotosRemovidas = new List<int>();

        private string _searchProprietarios = string.Empty;
        private string _searchLocatarios = string.Empty;
        private string _searchFiadores = string.Empty;
        private string _searchImoveis = string.Empty;
        private string _searchContratos = string.Empty;

        private string _clienteNomeCriar = string.Empty;
        private string _clienteCpfCnpjCriar = string.Empty;
        private string _clienteIdentidadeCriar = string.Empty;
        private string _clienteOrgaoExpedidorCriar = string.Empty;
        private string _clienteNacionalidadeCriar = string.Empty;
        private string _clienteNaturalidadeCriar = string.Empty;
        private string _clienteEstadoCivilCriar = string.Empty;
        private string _clienteProfissaoCriar = string.Empty;
        private string _clienteEnderecoCriar = string.Empty;
        private string _clienteBancoCriar = string.Empty;
        private string _clienteChavePixCriar = string.Empty;
        private string _clienteAgenciaCriar = string.Empty;
        private string _clienteContaCriar = string.Empty;
        private string _clienteCodBancoCriar = string.Empty;
        private string _clienteEmailCriar = string.Empty;
        private string _clienteTelefoneCriar = string.Empty;
        private DateTime? _clienteNascimentoCriar;

        private string _clienteNomeEditar = string.Empty;
        private string _clienteCpfCnpjEditar = string.Empty;
        private string _clienteIdentidadeEditar = string.Empty;
        private string _clienteOrgaoExpedidorEditar = string.Empty;
        private string _clienteNacionalidadeEditar = string.Empty;
        private string _clienteNaturalidadeEditar = string.Empty;
        private string _clienteEstadoCivilEditar = string.Empty;
        private string _clienteProfissaoEditar = string.Empty;
        private string _clienteEnderecoEditar = string.Empty;
        private string _clienteBancoEditar = string.Empty;
        private string _clienteChavePixEditar = string.Empty;
        private string _clienteAgenciaEditar = string.Empty;
        private string _clienteContaEditar = string.Empty;
        private string _clienteCodBancoEditar = string.Empty;
        private string _clienteEmailEditar = string.Empty;
        private string _clienteTelefoneEditar = string.Empty;
        private DateTime? _clienteNascimentoEditar;

        private ObservableCollection<string> _imovelProprietariosCriar = new ObservableCollection<string>();
        private ObservableCollection<string> _imovelIntencoesCriar = new ObservableCollection<string>();
        private ObservableCollection<string> _imovelTiposCriar = new ObservableCollection<string>();
        private ObservableCollection<string> _imovelFinalidadesCriar = new ObservableCollection<string>();
        private string _imovelProprietarioCriarSelecionado = string.Empty;
        private string _imovelIntencaoCriarSelecionada = string.Empty;
        private string _imovelTipoCriarSelecionado = string.Empty;
        private string _imovelFinalidadeCriarSelecionada = string.Empty;
        private string _imovelCepCriar = string.Empty;
        private string _imovelLogradouroCriar = string.Empty;
        private string _imovelNumeroCriar = string.Empty;
        private string _imovelPaisCriar = string.Empty;
        private string _imovelEstadoCriar = string.Empty;
        private string _imovelCidadeCriar = string.Empty;
        private string _imovelBairroCriar = string.Empty;
        private string _imovelMetragemCriar = string.Empty;
        private string _imovelComplementoCriar = string.Empty;
        private string _imovelInscricaoIptuCriar = string.Empty;
        private string _imovelNumeroCbmerjCriar = string.Empty;
        private string _imovelValorVendaCriar = string.Empty;
        private string _imovelValorLocacaoCriar = string.Empty;
        private string _imovelCondominioCriar = string.Empty;
        private string _imovelTaxaIncendioCriar = string.Empty;
        private string _imovelIptuCriar = string.Empty;
        private string _imovelForoCriar = string.Empty;
        private string _imovelObservacoesCriar = string.Empty;
        private string _imovelDescricaoCriar = string.Empty;

        private ObservableCollection<string> _imovelProprietariosEditar = new ObservableCollection<string>();
        private ObservableCollection<string> _imovelIntencoesEditar = new ObservableCollection<string>();
        private ObservableCollection<string> _imovelTiposEditar = new ObservableCollection<string>();
        private ObservableCollection<string> _imovelFinalidadesEditar = new ObservableCollection<string>();
        private string _imovelProprietarioEditarSelecionado = string.Empty;
        private string _imovelIntencaoEditarSelecionada = string.Empty;
        private string _imovelTipoEditarSelecionado = string.Empty;
        private string _imovelFinalidadeEditarSelecionada = string.Empty;
        private string _imovelCepEditar = string.Empty;
        private string _imovelLogradouroEditar = string.Empty;
        private string _imovelNumeroEditar = string.Empty;
        private string _imovelPaisEditar = string.Empty;
        private string _imovelEstadoEditar = string.Empty;
        private string _imovelCidadeEditar = string.Empty;
        private string _imovelBairroEditar = string.Empty;
        private string _imovelMetragemEditar = string.Empty;
        private string _imovelComplementoEditar = string.Empty;
        private string _imovelInscricaoIptuEditar = string.Empty;
        private string _imovelNumeroCbmerjEditar = string.Empty;
        private string _imovelValorVendaEditar = string.Empty;
        private string _imovelValorLocacaoEditar = string.Empty;
        private string _imovelCondominioEditar = string.Empty;
        private string _imovelTaxaIncendioEditar = string.Empty;
        private string _imovelIptuEditar = string.Empty;
        private string _imovelForoEditar = string.Empty;
        private string _imovelObservacoesEditar = string.Empty;
        private string _imovelDescricaoEditar = string.Empty;

        private IReadOnlyList<ClienteDAO> _catalogoProprietariosImovel = Array.Empty<ClienteDAO>();
        private IReadOnlyList<IntencaoDAO> _catalogoIntencoesImovel = Array.Empty<IntencaoDAO>();
        private IReadOnlyList<TipoImovelDAO> _catalogoTiposImovel = Array.Empty<TipoImovelDAO>();
        private IReadOnlyList<Imob.Models.DAOs.FinalidadeDAO> _catalogoFinalidadesImovel = Array.Empty<Imob.Models.DAOs.FinalidadeDAO>();

        private ObservableCollection<TipoContratoDAO> _tiposContrato = new ObservableCollection<TipoContratoDAO>();
        private ObservableCollection<ModalidadeContratoDAO> _modalidadesContrato = new ObservableCollection<ModalidadeContratoDAO>();
        private ObservableCollection<ObjetoContratoDAO> _objetosContrato = new ObservableCollection<ObjetoContratoDAO>();
        private ObservableCollection<ClienteDAO> _proprietariosContrato = new ObservableCollection<ClienteDAO>();
        private ObservableCollection<ImovelDAO> _imoveisContrato = new ObservableCollection<ImovelDAO>();
        private ObservableCollection<ClienteDAO> _locatariosContrato = new ObservableCollection<ClienteDAO>();
        private ObservableCollection<ClienteDAO> _fiadoresContrato = new ObservableCollection<ClienteDAO>();

        private string _contratoNomeCriar = string.Empty;
        private TipoContratoDAO _contratoTipoCriarSelecionado;
        private ModalidadeContratoDAO _contratoModalidadeCriarSelecionada;
        private ObjetoContratoDAO _contratoObjetoCriarSelecionado;
        private ClienteDAO _contratoProprietarioCriarSelecionado;
        private ImovelDAO _contratoImovelCriarSelecionado;
        private ClienteDAO _contratoContratante1CriarSelecionado;
        private ClienteDAO _contratoContratante2CriarSelecionado;
        private ClienteDAO _contratoContratante3CriarSelecionado;
        private ClienteDAO _contratoContratante4CriarSelecionado;
        private ClienteDAO _contratoFiadorCriarSelecionado;
        private ClienteDAO _contratoFiador2CriarSelecionado;
        private DateTime? _contratoDataInicioCriar;
        private string _contratoPrazoMesesCriar = string.Empty;
        private string _contratoValorCriar = string.Empty;
        private string _contratoVencimentoCriar = string.Empty;
        private string _contratoPropostaSegFiancaCriar = string.Empty;
        private string _contratoApoliceSegFiancaCriar = string.Empty;

        private string _contratoNomeVisualizar = string.Empty;
        private TipoContratoDAO _contratoTipoVisualizarSelecionado;
        private ModalidadeContratoDAO _contratoModalidadeVisualizarSelecionada;
        private ObjetoContratoDAO _contratoObjetoVisualizarSelecionado;
        private ClienteDAO _contratoProprietarioVisualizarSelecionado;
        private ImovelDAO _contratoImovelVisualizarSelecionado;
        private ClienteDAO _contratoContratante1VisualizarSelecionado;
        private ClienteDAO _contratoContratante2VisualizarSelecionado;
        private ClienteDAO _contratoContratante3VisualizarSelecionado;
        private ClienteDAO _contratoContratante4VisualizarSelecionado;
        private ClienteDAO _contratoFiadorVisualizarSelecionado;
        private ClienteDAO _contratoFiador2VisualizarSelecionado;
        private DateTime? _contratoDataInicioVisualizar;
        private string _contratoPrazoMesesVisualizar = string.Empty;
        private string _contratoValorVisualizar = string.Empty;
        private string _contratoVencimentoVisualizar = string.Empty;
        private string _contratoPropostaSegFiancaVisualizar = string.Empty;
        private string _contratoApoliceSegFiancaVisualizar = string.Empty;

        private ICollectionView _proprietariosView;
        private ICollectionView _locatariosView;
        private ICollectionView _fiadoresView;
        private ICollectionView _imoveisView;
        private ICollectionView _contratosView;

        public ObservableCollection<ImageSource> FotosSelecionadasPreview { get; } = new ObservableCollection<ImageSource>();

        public Action<string> ShowErrorAction { get; set; }
        public Action<string> ShowInfoAction { get; set; }
        public Func<string, bool> ShowConfirmAction { get; set; }
        public int UsuarioLogadoId { get; set; }



        public ICommand AtualizarImoveisCommand { get; }
        public ICommand AtualizarProprietariosCommand { get; }
        public ICommand AtualizarLocatariosCommand { get; }
        public ICommand AtualizarFiadoresCommand { get; }
        public ICommand AtualizarContratosCommand { get; }
        public ICommand AbrirImoveisCommand { get; }
        public ICommand AbrirProprietariosCommand { get; }
        public ICommand AbrirLocatariosCommand { get; }
        public ICommand AbrirFiadoresCommand { get; }
        public ICommand AbrirContratosCommand { get; }
        public ICommand AbrirVistoriasCommand { get; }
        public ICommand InativarProprietarioCommand { get; }
        public ICommand InativarLocatarioCommand { get; }
        public ICommand InativarFiadorCommand { get; }
        public ICommand InativarContratoCommand { get; }
        public ICommand InativarImovelCommand { get; }
        public ICommand VisualizarProprietarioCommand { get; }
        public ICommand VisualizarLocatarioCommand { get; }
        public ICommand VisualizarFiadorCommand { get; }
        public ICommand VisualizarImovelCommand { get; }
        public ICommand VisualizarContratoCommand { get; }
        public ICommand RemoverFotoCommand { get; }
        public ICommand AbrirImovelCriarCommand { get; }
        public ICommand FecharImovelCriarCommand { get; }
        public ICommand SalvarImovelCriarCommand { get; }
        public ICommand FecharImovelEditarCommand { get; }
        public ICommand SalvarImovelEditarCommand { get; }
        public ICommand AbrirFotosImovelCommand { get; }
        public ICommand FecharFotosImovelCriarCommand { get; }
        public ICommand SalvarFotosImovelCriarCommand { get; }
        public ICommand FecharFotosImovelEditarCommand { get; }
        public ICommand SalvarFotosImovelEditarCommand { get; }
        public ICommand AdicionarFotosImovelCommand { get; }
        public ICommand AbrirProprietarioCriarCommand { get; }
        public ICommand FecharProprietarioCriarCommand { get; }
        public ICommand SalvarProprietarioCriarCommand { get; }
        public ICommand FecharProprietarioEditarCommand { get; }
        public ICommand SalvarProprietarioEditarCommand { get; }
        public ICommand AbrirLocatarioCriarCommand { get; }
        public ICommand FecharLocatarioCriarCommand { get; }
        public ICommand SalvarLocatarioCriarCommand { get; }
        public ICommand FecharLocatarioEditarCommand { get; }
        public ICommand SalvarLocatarioEditarCommand { get; }
        public ICommand AbrirFiadorCriarCommand { get; }
        public ICommand FecharFiadorCriarCommand { get; }
        public ICommand SalvarFiadorCriarCommand { get; }
        public ICommand FecharFiadorEditarCommand { get; }
        public ICommand SalvarFiadorEditarCommand { get; }
        public ICommand AbrirContratoCriarCommand { get; }
        public ICommand FecharContratoCriarCommand { get; }
        public ICommand SalvarContratoCriarCommand { get; }
        public ICommand GerarContratoVisualizarCommand { get; }
        public ICommand FecharContratoVisualizarCommand { get; }
        public ICommand SalvarContratoVisualizarCommand { get; }

        public SistemaViewModel(ISistemaListagemService listagemService, ISistemaCrudService crudService)
        {
            _listagemService = listagemService;
            _crudService = crudService;

            AtualizarImoveisCommand = new AsyncRelayCommand(CarregarImoveisAsync, onException: ex => NotificarErro($"Erro ao carregar imóveis: {ex.Message}"));
            AtualizarProprietariosCommand = new AsyncRelayCommand(CarregarProprietariosAsync, onException: ex => NotificarErro($"Erro ao carregar proprietários: {ex.Message}"));
            AtualizarLocatariosCommand = new AsyncRelayCommand(CarregarLocatariosAsync, onException: ex => NotificarErro($"Erro ao carregar locatários: {ex.Message}"));
            AtualizarFiadoresCommand = new AsyncRelayCommand(CarregarFiadoresAsync, onException: ex => NotificarErro($"Erro ao carregar fiadores: {ex.Message}"));
            AtualizarContratosCommand = new AsyncRelayCommand(CarregarContratosAsync, onException: ex => NotificarErro($"Erro ao carregar contratos: {ex.Message}"));

            AbrirImoveisCommand = new AsyncRelayCommand(AbrirImoveisAsync, onException: ex => NotificarErro($"Erro ao abrir imóveis: {ex.Message}"));
            AbrirProprietariosCommand = new AsyncRelayCommand(AbrirProprietariosAsync, onException: ex => NotificarErro($"Erro ao abrir proprietários: {ex.Message}"));
            AbrirLocatariosCommand = new AsyncRelayCommand(AbrirLocatariosAsync, onException: ex => NotificarErro($"Erro ao abrir locatários: {ex.Message}"));
            AbrirFiadoresCommand = new AsyncRelayCommand(AbrirFiadoresAsync, onException: ex => NotificarErro($"Erro ao abrir fiadores: {ex.Message}"));
            AbrirContratosCommand = new AsyncRelayCommand(AbrirContratosAsync, onException: ex => NotificarErro($"Erro ao abrir contratos: {ex.Message}"));
            AbrirVistoriasCommand = new RelayCommand(() => MostrarPanelVistorias());

            InativarProprietarioCommand = new AsyncRelayCommandWithParameter(InativarProprietarioAsync, onException: ex => NotificarErro($"Erro ao inativar proprietário: {ex.Message}"));
            InativarLocatarioCommand = new AsyncRelayCommandWithParameter(InativarLocatarioAsync, onException: ex => NotificarErro($"Erro ao inativar locatário: {ex.Message}"));
            InativarFiadorCommand = new AsyncRelayCommandWithParameter(InativarFiadorAsync, onException: ex => NotificarErro($"Erro ao inativar fiador: {ex.Message}"));
            InativarContratoCommand = new AsyncRelayCommandWithParameter(InativarContratoAsync, onException: ex => NotificarErro($"Erro ao inativar contrato: {ex.Message}"));
            InativarImovelCommand = new AsyncRelayCommandWithParameter(InativarImovelAsync, onException: ex => NotificarErro($"Erro ao inativar imóvel: {ex.Message}"));
            VisualizarProprietarioCommand = new RelayCommandWithParameter(VisualizarProprietario);
            VisualizarLocatarioCommand = new RelayCommandWithParameter(VisualizarLocatario);
            VisualizarFiadorCommand = new RelayCommandWithParameter(VisualizarFiador);
            VisualizarImovelCommand = new RelayCommandWithParameter(VisualizarImovel);
            VisualizarContratoCommand = new AsyncRelayCommandWithParameter(VisualizarContratoAsync, onException: ex => NotificarErro($"Erro ao abrir contrato: {ex.Message}"));
            RemoverFotoCommand = new RelayCommandWithParameter(RemoverFoto);
            AbrirImovelCriarCommand = new AsyncRelayCommand(AbrirImovelCriarAsync, onException: ex => NotificarErro($"Erro ao abrir imóvel: {ex.Message}"));
            FecharImovelCriarCommand = new RelayCommand(FecharImovelCriar);
            SalvarImovelCriarCommand = new AsyncRelayCommand(SalvarImovelCriarAsync, onException: ex => NotificarErro($"Erro ao cadastrar imóvel: {ex.Message}"));
            FecharImovelEditarCommand = new RelayCommand(FecharImovelEditar);
            SalvarImovelEditarCommand = new AsyncRelayCommand(SalvarImovelEditarAsync, onException: ex => NotificarErro($"Erro ao atualizar imóvel: {ex.Message}"));
            AbrirFotosImovelCommand = new AsyncRelayCommand(AbrirFotosImovelAsync, onException: ex => NotificarErro($"Erro ao abrir fotos do imóvel: {ex.Message}"));
            FecharFotosImovelCriarCommand = new RelayCommand(FecharFotosImovelCriar);
            SalvarFotosImovelCriarCommand = new AsyncRelayCommand(SalvarFotosImovelCriarAsync, onException: ex => NotificarErro($"Erro ao cadastrar fotos: {ex.Message}"));
            FecharFotosImovelEditarCommand = new RelayCommand(FecharFotosImovelEditar);
            SalvarFotosImovelEditarCommand = new AsyncRelayCommand(SalvarFotosImovelEditarAsync, onException: ex => NotificarErro($"Erro ao atualizar fotos: {ex.Message}"));
            AdicionarFotosImovelCommand = new AsyncRelayCommand(AdicionarFotosImovelAsync, onException: ex => NotificarErro($"Erro ao adicionar fotos: {ex.Message}"));
            AbrirProprietarioCriarCommand = new RelayCommand(AbrirProprietarioCriar);
            FecharProprietarioCriarCommand = new RelayCommand(FecharProprietarioCriar);
            SalvarProprietarioCriarCommand = new AsyncRelayCommand(SalvarProprietarioCriarAsync, onException: ex => NotificarErro($"Erro ao cadastrar proprietário: {ex.Message}"));
            FecharProprietarioEditarCommand = new RelayCommand(FecharProprietarioEditar);
            SalvarProprietarioEditarCommand = new AsyncRelayCommand(SalvarProprietarioEditarAsync, onException: ex => NotificarErro($"Erro ao atualizar proprietário: {ex.Message}"));
            AbrirLocatarioCriarCommand = new RelayCommand(AbrirLocatarioCriar);
            FecharLocatarioCriarCommand = new RelayCommand(FecharLocatarioCriar);
            SalvarLocatarioCriarCommand = new AsyncRelayCommand(SalvarLocatarioCriarAsync, onException: ex => NotificarErro($"Erro ao cadastrar locatário: {ex.Message}"));
            FecharLocatarioEditarCommand = new RelayCommand(FecharLocatarioEditar);
            SalvarLocatarioEditarCommand = new AsyncRelayCommand(SalvarLocatarioEditarAsync, onException: ex => NotificarErro($"Erro ao atualizar locatário: {ex.Message}"));
            AbrirFiadorCriarCommand = new RelayCommand(AbrirFiadorCriar);
            FecharFiadorCriarCommand = new RelayCommand(FecharFiadorCriar);
            SalvarFiadorCriarCommand = new AsyncRelayCommand(SalvarFiadorCriarAsync, onException: ex => NotificarErro($"Erro ao cadastrar fiador: {ex.Message}"));
            FecharFiadorEditarCommand = new RelayCommand(FecharFiadorEditar);
            SalvarFiadorEditarCommand = new AsyncRelayCommand(SalvarFiadorEditarAsync, onException: ex => NotificarErro($"Erro ao atualizar fiador: {ex.Message}"));
            AbrirContratoCriarCommand = new AsyncRelayCommand(AbrirContratoCriarAsync, onException: ex => NotificarErro($"Erro ao abrir contrato: {ex.Message}"));
            FecharContratoCriarCommand = new RelayCommand(FecharContratoCriar);
            SalvarContratoCriarCommand = new AsyncRelayCommand(SalvarContratoCriarAsync, onException: ex => NotificarErro($"Erro ao cadastrar contrato: {ex.Message}"));
            GerarContratoVisualizarCommand = new RelayCommand(GerarContratoVisualizar);
            FecharContratoVisualizarCommand = new RelayCommand(FecharContratoVisualizar);
            SalvarContratoVisualizarCommand = new AsyncRelayCommand(SalvarContratoVisualizarAsync, onException: ex => NotificarErro($"Erro ao atualizar contrato: {ex.Message}"));

            AtualizarProprietariosView();
            AtualizarLocatariosView();
            AtualizarFiadoresView();
            AtualizarImoveisView();
            AtualizarContratosView();
        }

        public ObservableCollection<ImovelDAO> Imoveis
        {
            get => _imoveis;
            private set
            {
                _imoveis = value;
                OnPropertyChanged();
                AtualizarImoveisView();
            }
        }

        public ObservableCollection<string> ImovelProprietariosCriar
        {
            get => _imovelProprietariosCriar;
            private set
            {
                _imovelProprietariosCriar = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ImovelIntencoesCriar
        {
            get => _imovelIntencoesCriar;
            private set
            {
                _imovelIntencoesCriar = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ImovelTiposCriar
        {
            get => _imovelTiposCriar;
            private set
            {
                _imovelTiposCriar = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ImovelFinalidadesCriar
        {
            get => _imovelFinalidadesCriar;
            private set
            {
                _imovelFinalidadesCriar = value;
                OnPropertyChanged();
            }
        }

        public string ImovelProprietarioCriarSelecionado { get => _imovelProprietarioCriarSelecionado; set { _imovelProprietarioCriarSelecionado = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelIntencaoCriarSelecionada { get => _imovelIntencaoCriarSelecionada; set { _imovelIntencaoCriarSelecionada = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelTipoCriarSelecionado { get => _imovelTipoCriarSelecionado; set { _imovelTipoCriarSelecionado = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelFinalidadeCriarSelecionada { get => _imovelFinalidadeCriarSelecionada; set { _imovelFinalidadeCriarSelecionada = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelCepCriar { get => _imovelCepCriar; set { _imovelCepCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelLogradouroCriar { get => _imovelLogradouroCriar; set { _imovelLogradouroCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelNumeroCriar { get => _imovelNumeroCriar; set { _imovelNumeroCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelPaisCriar { get => _imovelPaisCriar; set { _imovelPaisCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelEstadoCriar { get => _imovelEstadoCriar; set { _imovelEstadoCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelCidadeCriar { get => _imovelCidadeCriar; set { _imovelCidadeCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelBairroCriar { get => _imovelBairroCriar; set { _imovelBairroCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelMetragemCriar { get => _imovelMetragemCriar; set { _imovelMetragemCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelComplementoCriar { get => _imovelComplementoCriar; set { _imovelComplementoCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelInscricaoIptuCriar { get => _imovelInscricaoIptuCriar; set { _imovelInscricaoIptuCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelNumeroCbmerjCriar { get => _imovelNumeroCbmerjCriar; set { _imovelNumeroCbmerjCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelValorVendaCriar { get => _imovelValorVendaCriar; set { _imovelValorVendaCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelValorLocacaoCriar { get => _imovelValorLocacaoCriar; set { _imovelValorLocacaoCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelCondominioCriar { get => _imovelCondominioCriar; set { _imovelCondominioCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelTaxaIncendioCriar { get => _imovelTaxaIncendioCriar; set { _imovelTaxaIncendioCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelIptuCriar { get => _imovelIptuCriar; set { _imovelIptuCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelForoCriar { get => _imovelForoCriar; set { _imovelForoCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelObservacoesCriar { get => _imovelObservacoesCriar; set { _imovelObservacoesCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelDescricaoCriar { get => _imovelDescricaoCriar; set { _imovelDescricaoCriar = value ?? string.Empty; OnPropertyChanged(); } }

        public ObservableCollection<string> ImovelProprietariosEditar { get => _imovelProprietariosEditar; private set { _imovelProprietariosEditar = value; OnPropertyChanged(); } }
        public ObservableCollection<string> ImovelIntencoesEditar { get => _imovelIntencoesEditar; private set { _imovelIntencoesEditar = value; OnPropertyChanged(); } }
        public ObservableCollection<string> ImovelTiposEditar { get => _imovelTiposEditar; private set { _imovelTiposEditar = value; OnPropertyChanged(); } }
        public ObservableCollection<string> ImovelFinalidadesEditar { get => _imovelFinalidadesEditar; private set { _imovelFinalidadesEditar = value; OnPropertyChanged(); } }
        public string ImovelProprietarioEditarSelecionado { get => _imovelProprietarioEditarSelecionado; set { _imovelProprietarioEditarSelecionado = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelIntencaoEditarSelecionada { get => _imovelIntencaoEditarSelecionada; set { _imovelIntencaoEditarSelecionada = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelTipoEditarSelecionado { get => _imovelTipoEditarSelecionado; set { _imovelTipoEditarSelecionado = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelFinalidadeEditarSelecionada { get => _imovelFinalidadeEditarSelecionada; set { _imovelFinalidadeEditarSelecionada = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelCepEditar { get => _imovelCepEditar; set { _imovelCepEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelLogradouroEditar { get => _imovelLogradouroEditar; set { _imovelLogradouroEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelNumeroEditar { get => _imovelNumeroEditar; set { _imovelNumeroEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelPaisEditar { get => _imovelPaisEditar; set { _imovelPaisEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelEstadoEditar { get => _imovelEstadoEditar; set { _imovelEstadoEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelCidadeEditar { get => _imovelCidadeEditar; set { _imovelCidadeEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelBairroEditar { get => _imovelBairroEditar; set { _imovelBairroEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelMetragemEditar { get => _imovelMetragemEditar; set { _imovelMetragemEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelComplementoEditar { get => _imovelComplementoEditar; set { _imovelComplementoEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelInscricaoIptuEditar { get => _imovelInscricaoIptuEditar; set { _imovelInscricaoIptuEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelNumeroCbmerjEditar { get => _imovelNumeroCbmerjEditar; set { _imovelNumeroCbmerjEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelValorVendaEditar { get => _imovelValorVendaEditar; set { _imovelValorVendaEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelValorLocacaoEditar { get => _imovelValorLocacaoEditar; set { _imovelValorLocacaoEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelCondominioEditar { get => _imovelCondominioEditar; set { _imovelCondominioEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelTaxaIncendioEditar { get => _imovelTaxaIncendioEditar; set { _imovelTaxaIncendioEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelIptuEditar { get => _imovelIptuEditar; set { _imovelIptuEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelForoEditar { get => _imovelForoEditar; set { _imovelForoEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelObservacoesEditar { get => _imovelObservacoesEditar; set { _imovelObservacoesEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ImovelDescricaoEditar { get => _imovelDescricaoEditar; set { _imovelDescricaoEditar = value ?? string.Empty; OnPropertyChanged(); } }

        public ObservableCollection<TipoContratoDAO> TiposContrato
        {
            get => _tiposContrato;
            private set
            {
                _tiposContrato = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ModalidadeContratoDAO> ModalidadesContrato
        {
            get => _modalidadesContrato;
            private set
            {
                _modalidadesContrato = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ObjetoContratoDAO> ObjetosContrato
        {
            get => _objetosContrato;
            private set
            {
                _objetosContrato = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ClienteDAO> ProprietariosContrato
        {
            get => _proprietariosContrato;
            private set
            {
                _proprietariosContrato = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ImovelDAO> ImoveisContrato
        {
            get => _imoveisContrato;
            private set
            {
                _imoveisContrato = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ClienteDAO> LocatariosContrato
        {
            get => _locatariosContrato;
            private set
            {
                _locatariosContrato = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ClienteDAO> FiadoresContrato
        {
            get => _fiadoresContrato;
            private set
            {
                _fiadoresContrato = value;
                OnPropertyChanged();
            }
        }

        public string ContratoNomeCriar { get => _contratoNomeCriar; set { _contratoNomeCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public TipoContratoDAO ContratoTipoCriarSelecionado { get => _contratoTipoCriarSelecionado; set { _contratoTipoCriarSelecionado = value; OnPropertyChanged(); } }
        public ModalidadeContratoDAO ContratoModalidadeCriarSelecionada { get => _contratoModalidadeCriarSelecionada; set { _contratoModalidadeCriarSelecionada = value; OnPropertyChanged(); } }
        public ObjetoContratoDAO ContratoObjetoCriarSelecionado { get => _contratoObjetoCriarSelecionado; set { _contratoObjetoCriarSelecionado = value; OnPropertyChanged(); } }
        public ClienteDAO ContratoProprietarioCriarSelecionado { get => _contratoProprietarioCriarSelecionado; set { _contratoProprietarioCriarSelecionado = value; OnPropertyChanged(); } }
        public ImovelDAO ContratoImovelCriarSelecionado { get => _contratoImovelCriarSelecionado; set { _contratoImovelCriarSelecionado = value; OnPropertyChanged(); } }
        public ClienteDAO ContratoContratante1CriarSelecionado { get => _contratoContratante1CriarSelecionado; set { _contratoContratante1CriarSelecionado = value; OnPropertyChanged(); } }
        public ClienteDAO ContratoContratante2CriarSelecionado { get => _contratoContratante2CriarSelecionado; set { _contratoContratante2CriarSelecionado = value; OnPropertyChanged(); } }
        public ClienteDAO ContratoContratante3CriarSelecionado { get => _contratoContratante3CriarSelecionado; set { _contratoContratante3CriarSelecionado = value; OnPropertyChanged(); } }
        public ClienteDAO ContratoContratante4CriarSelecionado { get => _contratoContratante4CriarSelecionado; set { _contratoContratante4CriarSelecionado = value; OnPropertyChanged(); } }
        public ClienteDAO ContratoFiadorCriarSelecionado { get => _contratoFiadorCriarSelecionado; set { _contratoFiadorCriarSelecionado = value; OnPropertyChanged(); } }
        public ClienteDAO ContratoFiador2CriarSelecionado { get => _contratoFiador2CriarSelecionado; set { _contratoFiador2CriarSelecionado = value; OnPropertyChanged(); } }
        public DateTime? ContratoDataInicioCriar { get => _contratoDataInicioCriar; set { _contratoDataInicioCriar = value; OnPropertyChanged(); } }
        public string ContratoPrazoMesesCriar { get => _contratoPrazoMesesCriar; set { _contratoPrazoMesesCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ContratoValorCriar { get => _contratoValorCriar; set { _contratoValorCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ContratoVencimentoCriar { get => _contratoVencimentoCriar; set { _contratoVencimentoCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ContratoPropostaSegFiancaCriar { get => _contratoPropostaSegFiancaCriar; set { _contratoPropostaSegFiancaCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ContratoApoliceSegFiancaCriar { get => _contratoApoliceSegFiancaCriar; set { _contratoApoliceSegFiancaCriar = value ?? string.Empty; OnPropertyChanged(); } }

        public string ContratoNomeVisualizar { get => _contratoNomeVisualizar; set { _contratoNomeVisualizar = value ?? string.Empty; OnPropertyChanged(); } }
        public TipoContratoDAO ContratoTipoVisualizarSelecionado { get => _contratoTipoVisualizarSelecionado; set { _contratoTipoVisualizarSelecionado = value; OnPropertyChanged(); } }
        public ModalidadeContratoDAO ContratoModalidadeVisualizarSelecionada { get => _contratoModalidadeVisualizarSelecionada; set { _contratoModalidadeVisualizarSelecionada = value; OnPropertyChanged(); } }
        public ObjetoContratoDAO ContratoObjetoVisualizarSelecionado { get => _contratoObjetoVisualizarSelecionado; set { _contratoObjetoVisualizarSelecionado = value; OnPropertyChanged(); } }
        public ClienteDAO ContratoProprietarioVisualizarSelecionado { get => _contratoProprietarioVisualizarSelecionado; set { _contratoProprietarioVisualizarSelecionado = value; OnPropertyChanged(); } }
        public ImovelDAO ContratoImovelVisualizarSelecionado { get => _contratoImovelVisualizarSelecionado; set { _contratoImovelVisualizarSelecionado = value; OnPropertyChanged(); } }
        public ClienteDAO ContratoContratante1VisualizarSelecionado { get => _contratoContratante1VisualizarSelecionado; set { _contratoContratante1VisualizarSelecionado = value; OnPropertyChanged(); } }
        public ClienteDAO ContratoContratante2VisualizarSelecionado { get => _contratoContratante2VisualizarSelecionado; set { _contratoContratante2VisualizarSelecionado = value; OnPropertyChanged(); } }
        public ClienteDAO ContratoContratante3VisualizarSelecionado { get => _contratoContratante3VisualizarSelecionado; set { _contratoContratante3VisualizarSelecionado = value; OnPropertyChanged(); } }
        public ClienteDAO ContratoContratante4VisualizarSelecionado { get => _contratoContratante4VisualizarSelecionado; set { _contratoContratante4VisualizarSelecionado = value; OnPropertyChanged(); } }
        public ClienteDAO ContratoFiadorVisualizarSelecionado { get => _contratoFiadorVisualizarSelecionado; set { _contratoFiadorVisualizarSelecionado = value; OnPropertyChanged(); } }
        public ClienteDAO ContratoFiador2VisualizarSelecionado { get => _contratoFiador2VisualizarSelecionado; set { _contratoFiador2VisualizarSelecionado = value; OnPropertyChanged(); } }
        public DateTime? ContratoDataInicioVisualizar { get => _contratoDataInicioVisualizar; set { _contratoDataInicioVisualizar = value; OnPropertyChanged(); } }
        public string ContratoPrazoMesesVisualizar { get => _contratoPrazoMesesVisualizar; set { _contratoPrazoMesesVisualizar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ContratoValorVisualizar { get => _contratoValorVisualizar; set { _contratoValorVisualizar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ContratoVencimentoVisualizar { get => _contratoVencimentoVisualizar; set { _contratoVencimentoVisualizar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ContratoPropostaSegFiancaVisualizar { get => _contratoPropostaSegFiancaVisualizar; set { _contratoPropostaSegFiancaVisualizar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ContratoApoliceSegFiancaVisualizar { get => _contratoApoliceSegFiancaVisualizar; set { _contratoApoliceSegFiancaVisualizar = value ?? string.Empty; OnPropertyChanged(); } }

        public ImovelDAO ImovelSelecionado
        {
            get => _imovelSelecionado;
            set
            {
                _imovelSelecionado = value;
                OnPropertyChanged();
            }
        }

        public ContratoDAO ContratoSelecionado
        {
            get => _contratoSelecionado;
            set
            {
                _contratoSelecionado = value;
                OnPropertyChanged();
            }
        }

        public void DefinirImovelCadastrado(int idImovel)
        {
            _idImovelCadastrado = idImovel;
            if (idImovel > 0 && (ImovelSelecionado == null || ImovelSelecionado.Id != idImovel))
            {
                ImovelSelecionado = new ImovelDAO { Id = idImovel };
            }
        }

        public ObservableCollection<ClienteDAO> Proprietarios
        {
            get => _proprietarios;
            private set
            {
                _proprietarios = value;
                OnPropertyChanged();
                AtualizarProprietariosView();
            }
        }

        public ObservableCollection<ClienteDAO> Locatarios
        {
            get => _locatarios;
            private set
            {
                _locatarios = value;
                OnPropertyChanged();
                AtualizarLocatariosView();
            }
        }

        public ObservableCollection<ClienteDAO> Fiadores
        {
            get => _fiadores;
            private set
            {
                _fiadores = value;
                OnPropertyChanged();
                AtualizarFiadoresView();
            }
        }

        public ObservableCollection<ContratoDAO> Contratos
        {
            get => _contratos;
            private set
            {
                _contratos = value;
                OnPropertyChanged();
                AtualizarContratosView();
            }
        }

        public ICollectionView ProprietariosView
        {
            get => _proprietariosView;
            private set
            {
                _proprietariosView = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView LocatariosView
        {
            get => _locatariosView;
            private set
            {
                _locatariosView = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView FiadoresView
        {
            get => _fiadoresView;
            private set
            {
                _fiadoresView = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView ImoveisView
        {
            get => _imoveisView;
            private set
            {
                _imoveisView = value;
                OnPropertyChanged();
            }
        }

        public ICollectionView ContratosView
        {
            get => _contratosView;
            private set
            {
                _contratosView = value;
                OnPropertyChanged();
            }
        }

        public string SearchProprietarios
        {
            get => _searchProprietarios;
            set
            {
                _searchProprietarios = value ?? string.Empty;
                OnPropertyChanged();
                ProprietariosView?.Refresh();
            }
        }

        public string SearchLocatarios
        {
            get => _searchLocatarios;
            set
            {
                _searchLocatarios = value ?? string.Empty;
                OnPropertyChanged();
                LocatariosView?.Refresh();
            }
        }

        public string SearchFiadores
        {
            get => _searchFiadores;
            set
            {
                _searchFiadores = value ?? string.Empty;
                OnPropertyChanged();
                FiadoresView?.Refresh();
            }
        }

        public string SearchImoveis
        {
            get => _searchImoveis;
            set
            {
                _searchImoveis = value ?? string.Empty;
                OnPropertyChanged();
                ImoveisView?.Refresh();
            }
        }

        public string SearchContratos
        {
            get => _searchContratos;
            set
            {
                _searchContratos = value ?? string.Empty;
                OnPropertyChanged();
                ContratosView?.Refresh();
            }
        }

        public string ClienteNomeCriar { get => _clienteNomeCriar; set { _clienteNomeCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteCpfCnpjCriar { get => _clienteCpfCnpjCriar; set { _clienteCpfCnpjCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteIdentidadeCriar { get => _clienteIdentidadeCriar; set { _clienteIdentidadeCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteOrgaoExpedidorCriar { get => _clienteOrgaoExpedidorCriar; set { _clienteOrgaoExpedidorCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteNacionalidadeCriar { get => _clienteNacionalidadeCriar; set { _clienteNacionalidadeCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteNaturalidadeCriar { get => _clienteNaturalidadeCriar; set { _clienteNaturalidadeCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteEstadoCivilCriar { get => _clienteEstadoCivilCriar; set { _clienteEstadoCivilCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteProfissaoCriar { get => _clienteProfissaoCriar; set { _clienteProfissaoCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteEnderecoCriar { get => _clienteEnderecoCriar; set { _clienteEnderecoCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteBancoCriar { get => _clienteBancoCriar; set { _clienteBancoCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteChavePixCriar { get => _clienteChavePixCriar; set { _clienteChavePixCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteAgenciaCriar { get => _clienteAgenciaCriar; set { _clienteAgenciaCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteContaCriar { get => _clienteContaCriar; set { _clienteContaCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteCodBancoCriar { get => _clienteCodBancoCriar; set { _clienteCodBancoCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteEmailCriar { get => _clienteEmailCriar; set { _clienteEmailCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteTelefoneCriar { get => _clienteTelefoneCriar; set { _clienteTelefoneCriar = value ?? string.Empty; OnPropertyChanged(); } }
        public DateTime? ClienteNascimentoCriar { get => _clienteNascimentoCriar; set { _clienteNascimentoCriar = value; OnPropertyChanged(); } }

        public string ClienteNomeEditar { get => _clienteNomeEditar; set { _clienteNomeEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteCpfCnpjEditar { get => _clienteCpfCnpjEditar; set { _clienteCpfCnpjEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteIdentidadeEditar { get => _clienteIdentidadeEditar; set { _clienteIdentidadeEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteOrgaoExpedidorEditar { get => _clienteOrgaoExpedidorEditar; set { _clienteOrgaoExpedidorEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteNacionalidadeEditar { get => _clienteNacionalidadeEditar; set { _clienteNacionalidadeEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteNaturalidadeEditar { get => _clienteNaturalidadeEditar; set { _clienteNaturalidadeEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteEstadoCivilEditar { get => _clienteEstadoCivilEditar; set { _clienteEstadoCivilEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteProfissaoEditar { get => _clienteProfissaoEditar; set { _clienteProfissaoEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteEnderecoEditar { get => _clienteEnderecoEditar; set { _clienteEnderecoEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteBancoEditar { get => _clienteBancoEditar; set { _clienteBancoEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteChavePixEditar { get => _clienteChavePixEditar; set { _clienteChavePixEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteAgenciaEditar { get => _clienteAgenciaEditar; set { _clienteAgenciaEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteContaEditar { get => _clienteContaEditar; set { _clienteContaEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteCodBancoEditar { get => _clienteCodBancoEditar; set { _clienteCodBancoEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteEmailEditar { get => _clienteEmailEditar; set { _clienteEmailEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public string ClienteTelefoneEditar { get => _clienteTelefoneEditar; set { _clienteTelefoneEditar = value ?? string.Empty; OnPropertyChanged(); } }
        public DateTime? ClienteNascimentoEditar { get => _clienteNascimentoEditar; set { _clienteNascimentoEditar = value; OnPropertyChanged(); } }

        public Visibility ProprietariosPanelVisibility
        {
            get => _proprietariosPanelVisibility;
            private set
            {
                _proprietariosPanelVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility LocatariosPanelVisibility
        {
            get => _locatariosPanelVisibility;
            private set
            {
                _locatariosPanelVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility FiadoresPanelVisibility
        {
            get => _fiadoresPanelVisibility;
            private set
            {
                _fiadoresPanelVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility ImoveisPanelVisibility
        {
            get => _imoveisPanelVisibility;
            private set
            {
                _imoveisPanelVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility ContratosPanelVisibility
        {
            get => _contratosPanelVisibility;
            private set
            {
                _contratosPanelVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility VistoriasPanelVisibility
        {
            get => _vistoriasPanelVisibility;
            private set
            {
                _vistoriasPanelVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility ContratoModalCriarVisibility
        {
            get => _contratoModalCriarVisibility;
            set
            {
                _contratoModalCriarVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility ContratoModalVisualizarVisibility
        {
            get => _contratoModalVisualizarVisibility;
            set
            {
                _contratoModalVisualizarVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility ProprietarioModalCriarVisibility
        {
            get => _proprietarioModalCriarVisibility;
            set
            {
                _proprietarioModalCriarVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility ProprietarioModalEditarVisibility
        {
            get => _proprietarioModalEditarVisibility;
            set
            {
                _proprietarioModalEditarVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility LocatarioModalCriarVisibility
        {
            get => _locatarioModalCriarVisibility;
            set
            {
                _locatarioModalCriarVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility LocatarioModalEditarVisibility
        {
            get => _locatarioModalEditarVisibility;
            set
            {
                _locatarioModalEditarVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility FiadorModalCriarVisibility
        {
            get => _fiadorModalCriarVisibility;
            set
            {
                _fiadorModalCriarVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility FiadorModalEditarVisibility
        {
            get => _fiadorModalEditarVisibility;
            set
            {
                _fiadorModalEditarVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility ImovelModalCriarVisibility
        {
            get => _imovelModalCriarVisibility;
            set
            {
                _imovelModalCriarVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility ImovelModalEditarVisibility
        {
            get => _imovelModalEditarVisibility;
            set
            {
                _imovelModalEditarVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility ImovelFotosModalCriarVisibility
        {
            get => _imovelFotosModalCriarVisibility;
            set
            {
                _imovelFotosModalCriarVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility ImovelFotosModalEditarVisibility
        {
            get => _imovelFotosModalEditarVisibility;
            set
            {
                _imovelFotosModalEditarVisibility = value;
                OnPropertyChanged();
            }
        }

        public void FecharTodosOsPanels()
        {
            ProprietariosPanelVisibility = Visibility.Hidden;
            LocatariosPanelVisibility = Visibility.Hidden;
            FiadoresPanelVisibility = Visibility.Hidden;
            ImoveisPanelVisibility = Visibility.Hidden;
            ContratosPanelVisibility = Visibility.Hidden;
            VistoriasPanelVisibility = Visibility.Hidden;
        }

        public void MostrarPanelProprietarios()
        {
            FecharTodosOsPanels();
            ProprietariosPanelVisibility = Visibility.Visible;
        }

        public void MostrarPanelLocatarios()
        {
            FecharTodosOsPanels();
            LocatariosPanelVisibility = Visibility.Visible;
        }

        public void MostrarPanelFiadores()
        {
            FecharTodosOsPanels();
            FiadoresPanelVisibility = Visibility.Visible;
        }

        public void MostrarPanelImoveis()
        {
            FecharTodosOsPanels();
            ImoveisPanelVisibility = Visibility.Visible;
        }

        public void MostrarPanelContratos()
        {
            FecharTodosOsPanels();
            ContratosPanelVisibility = Visibility.Visible;
        }

        public void MostrarPanelVistorias()
        {
            FecharTodosOsPanels();
            VistoriasPanelVisibility = Visibility.Visible;
        }

        private void AbrirProprietarioCriar()
        {
            ProprietarioModalCriarVisibility = Visibility.Visible;
        }

        private void FecharProprietarioCriar()
        {
            LimparFormularioClienteCriar();
            ProprietarioModalCriarVisibility = Visibility.Hidden;
        }

        private void FecharProprietarioEditar()
        {
            ProprietarioModalEditarVisibility = Visibility.Hidden;
            _proprietarioEditarId = null;
        }

        private void AbrirLocatarioCriar()
        {
            LocatarioModalCriarVisibility = Visibility.Visible;
        }

        private void FecharLocatarioCriar()
        {
            LimparFormularioClienteCriar();
            LocatarioModalCriarVisibility = Visibility.Hidden;
        }

        private void FecharLocatarioEditar()
        {
            LocatarioModalEditarVisibility = Visibility.Hidden;
            _locatarioEditarId = null;
        }

        private void AbrirFiadorCriar()
        {
            FiadorModalCriarVisibility = Visibility.Visible;
        }

        private void FecharFiadorCriar()
        {
            LimparFormularioClienteCriar();
            FiadorModalCriarVisibility = Visibility.Hidden;
        }

        private void FecharFiadorEditar()
        {
            FiadorModalEditarVisibility = Visibility.Hidden;
            _fiadorEditarId = null;
        }

        private void AbrirContratoCriar()
        {
            ContratoModalCriarVisibility = Visibility.Visible;
        }

        private async Task AbrirContratoCriarAsync()
        {
            await CarregarCombosContratoAsync();
            LimparFormularioContratoCriar();
            AbrirContratoCriar();
        }

        private void FecharContratoCriar()
        {
            ContratoModalCriarVisibility = Visibility.Hidden;
        }

        private void FecharContratoVisualizar()
        {
            ContratoModalVisualizarVisibility = Visibility.Hidden;
        }

        private async Task CarregarCombosContratoAsync()
        {
            var tiposTask = _listagemService.ObterTiposContratoAsync();
            var modalidadesTask = _listagemService.ObterModalidadesContratoAsync();
            var objetosTask = _listagemService.ObterObjetosContratoAsync();
            var proprietariosTask = _listagemService.ObterProprietariosAsync();
            var imoveisTask = _listagemService.ObterImoveisAsync();
            var locatariosTask = _listagemService.ObterLocatariosAsync();
            var fiadoresTask = _listagemService.ObterFiadoresAsync();

            await Task.WhenAll(tiposTask, modalidadesTask, objetosTask, proprietariosTask, imoveisTask, locatariosTask, fiadoresTask);

            TiposContrato = new ObservableCollection<TipoContratoDAO>((await tiposTask).Where(x => x != null));
            ModalidadesContrato = new ObservableCollection<ModalidadeContratoDAO>((await modalidadesTask).Where(x => x != null));
            ObjetosContrato = new ObservableCollection<ObjetoContratoDAO>((await objetosTask).Where(x => x != null));
            ProprietariosContrato = new ObservableCollection<ClienteDAO>((await proprietariosTask).Where(x => x != null));
            ImoveisContrato = new ObservableCollection<ImovelDAO>((await imoveisTask).Where(x => x != null));
            LocatariosContrato = new ObservableCollection<ClienteDAO>((await locatariosTask).Where(x => x != null));
            FiadoresContrato = new ObservableCollection<ClienteDAO>((await fiadoresTask).Where(x => x != null));
        }

        private void GerarContratoVisualizar()
        {
            var contrato = ContratoSelecionado;
            if (contrato == null)
            {
                NotificarInfo("Selecione um contrato para gerar.");
                return;
            }

            try
            {
                var gerador = new GeradorContratoPdf();
                NotificarInfo("Em andamento");
                gerador.CriarContratoLocacao(contrato);
            }
            catch (Exception ex)
            {
                NotificarErro($"Erro ao gerar contrato: {ex.Message}");
            }
        }

        private void AbrirImovelCriar()
        {
            ImovelModalCriarVisibility = Visibility.Visible;
        }

        private async Task AbrirImovelCriarAsync()
        {
            await CarregarCombosImovelCriarAsync();
            AbrirImovelCriar();
        }

        private async Task CarregarCombosImovelCriarAsync()
        {
            var proprietariosTask = _listagemService.ObterProprietariosAsync();
            var intencoesTask = _listagemService.ObterIntencoesAsync();
            var tiposTask = _listagemService.ObterTiposImovelAsync();
            var finalidadesTask = _listagemService.ObterFinalidadesAsync();

            await Task.WhenAll(proprietariosTask, intencoesTask, tiposTask, finalidadesTask);

            _catalogoProprietariosImovel = await proprietariosTask;
            _catalogoIntencoesImovel = await intencoesTask;
            _catalogoTiposImovel = await tiposTask;
            _catalogoFinalidadesImovel = await finalidadesTask;

            ImovelProprietariosCriar = new ObservableCollection<string>(_catalogoProprietariosImovel.Where(x => x != null).Select(x => x.Nome ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList());
            ImovelIntencoesCriar = new ObservableCollection<string>(_catalogoIntencoesImovel.Where(x => x != null).Select(x => x.Nome ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList());
            ImovelTiposCriar = new ObservableCollection<string>(_catalogoTiposImovel.Where(x => x != null).Select(x => x.Nome ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList());
            ImovelFinalidadesCriar = new ObservableCollection<string>(_catalogoFinalidadesImovel.Where(x => x != null).Select(x => x.Nome ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList());
        }

        private async Task CarregarCombosImovelEditarAsync()
        {
            var proprietariosTask = _listagemService.ObterProprietariosAsync();
            var intencoesTask = _listagemService.ObterIntencoesAsync();
            var tiposTask = _listagemService.ObterTiposImovelAsync();
            var finalidadesTask = _listagemService.ObterFinalidadesAsync();

            await Task.WhenAll(proprietariosTask, intencoesTask, tiposTask, finalidadesTask);

            _catalogoProprietariosImovel = await proprietariosTask;
            _catalogoIntencoesImovel = await intencoesTask;
            _catalogoTiposImovel = await tiposTask;
            _catalogoFinalidadesImovel = await finalidadesTask;

            ImovelProprietariosEditar = new ObservableCollection<string>(_catalogoProprietariosImovel.Where(x => x != null).Select(x => x.Nome ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList());
            ImovelIntencoesEditar = new ObservableCollection<string>(_catalogoIntencoesImovel.Where(x => x != null).Select(x => x.Nome ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList());
            ImovelTiposEditar = new ObservableCollection<string>(_catalogoTiposImovel.Where(x => x != null).Select(x => x.Nome ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList());
            ImovelFinalidadesEditar = new ObservableCollection<string>(_catalogoFinalidadesImovel.Where(x => x != null).Select(x => x.Nome ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList());
        }

        private void PreencherFormularioImovelEditar(ImovelDAO imovel)
        {
            ImovelProprietarioEditarSelecionado = SelecionarNomePorIdOuNome(_catalogoProprietariosImovel, imovel.Proprietario?.Id ?? 0, imovel.Proprietario?.Nome);
            ImovelIntencaoEditarSelecionada = SelecionarNomePorIdOuNome(_catalogoIntencoesImovel, imovel.Intencao?.Id ?? 0, imovel.Intencao?.Nome);
            ImovelTipoEditarSelecionado = SelecionarNomePorIdOuNome(_catalogoTiposImovel, imovel.TipoImovel?.Id ?? 0, imovel.TipoImovel?.Nome);
            var finalidadeId = imovel.Finalidade?.Id ?? 0;
            var finalidadeNome = imovel.Finalidade?.Nome;
            if (finalidadeId <= 0 || string.IsNullOrWhiteSpace(finalidadeNome))
            {
                finalidadeId = ObterIdFinalidadeDoImovel(imovel);
                finalidadeNome = null;
            }
            ImovelFinalidadeEditarSelecionada = SelecionarNomePorIdOuNome(_catalogoFinalidadesImovel, finalidadeId, finalidadeNome);
            ImovelCepEditar = imovel.Cep ?? string.Empty;
            ImovelLogradouroEditar = imovel.Logradouro ?? string.Empty;
            ImovelNumeroEditar = imovel.Numero.ToString(CultureInfo.InvariantCulture);
            ImovelPaisEditar = imovel.Pais ?? string.Empty;
            ImovelEstadoEditar = imovel.Estado ?? string.Empty;
            ImovelCidadeEditar = imovel.Cidade ?? string.Empty;
            ImovelBairroEditar = imovel.Bairro ?? string.Empty;
            ImovelMetragemEditar = imovel.Metragem.ToString(CultureInfo.CurrentCulture);
            ImovelComplementoEditar = imovel.Complemento ?? string.Empty;
            ImovelInscricaoIptuEditar = imovel.InscricaoIptu ?? string.Empty;
            ImovelNumeroCbmerjEditar = imovel.NumeroCbmerj ?? string.Empty;
            ImovelValorVendaEditar = (imovel.ValorVenda ?? 0m).ToString(CultureInfo.CurrentCulture);
            ImovelValorLocacaoEditar = (imovel.ValorLocacao ?? 0m).ToString(CultureInfo.CurrentCulture);
            ImovelCondominioEditar = (imovel.Condominio ?? 0m).ToString(CultureInfo.CurrentCulture);
            ImovelTaxaIncendioEditar = (imovel.TaxaIncendio ?? 0m).ToString(CultureInfo.CurrentCulture);
            ImovelIptuEditar = (imovel.Iptu ?? 0m).ToString(CultureInfo.CurrentCulture);
            ImovelForoEditar = (imovel.Foro ?? 0m).ToString(CultureInfo.CurrentCulture);
            ImovelObservacoesEditar = imovel.Observacao ?? string.Empty;
            ImovelDescricaoEditar = imovel.Descricao ?? string.Empty;
        }

        private int ObterIdFinalidadeDoImovel(ImovelDAO imovel)
        {
            var finalidade = _catalogoFinalidadesImovel.FirstOrDefault(x =>
                x != null && string.Equals(x.Nome?.Trim(), imovel.NomeFinalidade?.Trim(), StringComparison.OrdinalIgnoreCase));
            return finalidade?.Id ?? 0;
        }

        private void FecharImovelCriar()
        {
            ImovelModalCriarVisibility = Visibility.Hidden;
        }

        private void FecharImovelEditar()
        {
            ImovelModalEditarVisibility = Visibility.Hidden;
            ImovelSelecionado = null;
        }

        private void AbrirFotosImovel()
        {
            ImovelFotosModalEditarVisibility = Visibility.Visible;
        }

        private void FecharFotosImovelCriar()
        {
            ImovelFotosModalCriarVisibility = Visibility.Hidden;
            LimparEstadoFotos();
        }

        private void FecharFotosImovelEditar()
        {
            ImovelFotosModalEditarVisibility = Visibility.Hidden;
            ImovelModalEditarVisibility = Visibility.Visible;
            LimparEstadoFotos();
        }

        public void SetImoveis(IEnumerable<ImovelDAO> itens)
        {
            Imoveis = new ObservableCollection<ImovelDAO>(itens ?? new List<ImovelDAO>());
        }

        public void SetProprietarios(IEnumerable<ClienteDAO> itens)
        {
            Proprietarios = new ObservableCollection<ClienteDAO>(itens ?? new List<ClienteDAO>());
        }

        public void SetLocatarios(IEnumerable<ClienteDAO> itens)
        {
            Locatarios = new ObservableCollection<ClienteDAO>(itens ?? new List<ClienteDAO>());
        }

        public void SetFiadores(IEnumerable<ClienteDAO> itens)
        {
            Fiadores = new ObservableCollection<ClienteDAO>(itens ?? new List<ClienteDAO>());
        }

        public void SetContratos(IEnumerable<ContratoDAO> itens)
        {
            Contratos = new ObservableCollection<ContratoDAO>(itens ?? new List<ContratoDAO>());
        }

        public async Task CarregarImoveisAsync()
        {
            SetImoveis(await _listagemService.ObterImoveisAsync());
        }

        public async Task CarregarProprietariosAsync()
        {
            SetProprietarios(await _listagemService.ObterProprietariosAsync());
        }

        public async Task CarregarLocatariosAsync()
        {
            SetLocatarios(await _listagemService.ObterLocatariosAsync());
        }

        public async Task CarregarFiadoresAsync()
        {
            SetFiadores(await _listagemService.ObterFiadoresAsync());
        }

        public async Task CarregarContratosAsync()
        {
            SetContratos(await _listagemService.ObterContratosAsync());
        }

        private async Task AbrirImoveisAsync()
        {
            await CarregarImoveisAsync();
            MostrarPanelImoveis();
        }

        private async Task AbrirProprietariosAsync()
        {
            await CarregarProprietariosAsync();
            MostrarPanelProprietarios();
        }

        private async Task AbrirLocatariosAsync()
        {
            await CarregarLocatariosAsync();
            MostrarPanelLocatarios();
        }

        private async Task AbrirFiadoresAsync()
        {
            await CarregarFiadoresAsync();
            MostrarPanelFiadores();
        }

        private async Task AbrirContratosAsync()
        {
            await CarregarContratosAsync();
            MostrarPanelContratos();
        }

        private async Task InativarProprietarioAsync(object parameter)
        {
            if (!TryGetId(parameter, out var id))
            {
                return;
            }

            if (ShowConfirmAction != null && !ShowConfirmAction("Tem certeza que deseja inativar este proprietário?"))
            {
                return;
            }

            await _crudService.InativarClienteAsync(id);
            await CarregarProprietariosAsync();
            NotificarInfo("Proprietário inativado com sucesso.");
        }

        private async Task InativarLocatarioAsync(object parameter)
        {
            if (!TryGetId(parameter, out var id))
            {
                return;
            }

            if (ShowConfirmAction != null && !ShowConfirmAction("Tem certeza que deseja inativar este locatário?"))
            {
                return;
            }

            await _crudService.InativarClienteAsync(id);
            await CarregarLocatariosAsync();
            NotificarInfo("Locatário inativado com sucesso.");
        }

        private async Task InativarFiadorAsync(object parameter)
        {
            if (!TryGetId(parameter, out var id))
            {
                return;
            }

            if (ShowConfirmAction != null && !ShowConfirmAction("Tem certeza que deseja inativar este fiador?"))
            {
                return;
            }

            await _crudService.InativarClienteAsync(id);
            await CarregarFiadoresAsync();
            NotificarInfo("Fiador inativado com sucesso.");
        }

        private async Task InativarContratoAsync(object parameter)
        {
            if (!TryGetId(parameter, out var id))
            {
                return;
            }

            if (ShowConfirmAction != null && !ShowConfirmAction("Tem certeza que deseja inativar este contrato?"))
            {
                return;
            }

            await _crudService.InativarContratoAsync(id);
            await CarregarContratosAsync();
            NotificarInfo("Contrato inativado com sucesso.");
        }

        private async Task InativarImovelAsync(object parameter)
        {
            if (!TryGetId(parameter, out var id))
            {
                return;
            }

            if (ShowConfirmAction != null && !ShowConfirmAction("Tem certeza que deseja inativar este imóvel?"))
            {
                return;
            }

            await _crudService.InativarImovelAsync(id);
            await CarregarImoveisAsync();
            NotificarInfo("Imóvel inativado com sucesso.");
        }

        private void VisualizarProprietario(object parameter)
        {
            if (!TryGetId(parameter, out var id))
            {
                NotificarInfo("Selecione um proprietário para visualizar.");
                return;
            }

            var proprietario = ObterClientePorId(Proprietarios, id);
            if (proprietario == null)
            {
                NotificarInfo("Proprietário não encontrado.");
                return;
            }

            _proprietarioEditarId = proprietario.Id;
            PreencherFormularioClienteEditar(proprietario);
            ProprietarioModalEditarVisibility = Visibility.Visible;
        }

        private async Task SalvarProprietarioCriarAsync()
        {
            if (string.IsNullOrWhiteSpace(ClienteNomeCriar) || string.IsNullOrWhiteSpace(ClienteCpfCnpjCriar))
            {
                NotificarErro("Por favor, preencha os campos obrigatórios. (*)");
                return;
            }

            var tipoClienteId = _crudService.ObterTipoClienteId("Proprietário");
            if (tipoClienteId == 0)
            {
                NotificarErro("Tipo de cliente proprietário não encontrado.");
                return;
            }

            var cliente = CriarClienteDtoCriar(tipoClienteId);
            await _crudService.CadastrarClienteAsync(cliente);

            NotificarInfo("Proprietário cadastrado com sucesso!");
            FecharProprietarioCriar();
            await CarregarProprietariosAsync();
        }

        private async Task SalvarProprietarioEditarAsync()
        {
            if (!_proprietarioEditarId.HasValue || _proprietarioEditarId.Value <= 0)
            {
                NotificarInfo("Selecione um proprietário para editar.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ClienteNomeEditar) || string.IsNullOrWhiteSpace(ClienteCpfCnpjEditar))
            {
                NotificarErro("Por favor, preencha os campos obrigatórios. (*)");
                return;
            }

            var tipoClienteId = _crudService.ObterTipoClienteId("Proprietário");
            if (tipoClienteId == 0)
            {
                NotificarErro("Tipo de cliente proprietário não encontrado.");
                return;
            }

            var clienteAtualizado = CriarClienteDtoEditar(tipoClienteId);
            await _crudService.AtualizarClienteAsync(_proprietarioEditarId.Value, clienteAtualizado);

            NotificarInfo("Proprietário atualizado com sucesso!");
            FecharProprietarioEditar();
            await CarregarProprietariosAsync();
        }

        private void VisualizarLocatario(object parameter)
        {
            if (!TryGetId(parameter, out var id))
            {
                NotificarInfo("Selecione um locatário para visualizar.");
                return;
            }

            var locatario = ObterClientePorId(Locatarios, id);
            if (locatario == null)
            {
                NotificarInfo("Locatário não encontrado.");
                return;
            }

            _locatarioEditarId = locatario.Id;
            PreencherFormularioClienteEditar(locatario);
            LocatarioModalEditarVisibility = Visibility.Visible;
        }

        private async Task SalvarLocatarioCriarAsync()
        {
            if (string.IsNullOrWhiteSpace(ClienteNomeCriar) || string.IsNullOrWhiteSpace(ClienteCpfCnpjCriar))
            {
                NotificarErro("Por favor, preencha os campos obrigatórios. (*)");
                return;
            }

            var tipoClienteId = _crudService.ObterTipoClienteId("Locatário");
            if (tipoClienteId == 0)
            {
                NotificarErro("Tipo de cliente locatário não encontrado.");
                return;
            }

            var cliente = CriarClienteDtoCriar(tipoClienteId);
            await _crudService.CadastrarClienteAsync(cliente);

            NotificarInfo("Locatário cadastrado com sucesso!");
            FecharLocatarioCriar();
            await CarregarLocatariosAsync();
        }

        private async Task SalvarLocatarioEditarAsync()
        {
            if (!_locatarioEditarId.HasValue || _locatarioEditarId.Value <= 0)
            {
                NotificarInfo("Selecione um locatário para editar.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ClienteNomeEditar) || string.IsNullOrWhiteSpace(ClienteCpfCnpjEditar))
            {
                NotificarErro("Por favor, preencha os campos obrigatórios. (*)");
                return;
            }

            var tipoClienteId = _crudService.ObterTipoClienteId("Locatário");
            if (tipoClienteId == 0)
            {
                NotificarErro("Tipo de cliente locatário não encontrado.");
                return;
            }

            var clienteAtualizado = CriarClienteDtoEditar(tipoClienteId);
            await _crudService.AtualizarClienteAsync(_locatarioEditarId.Value, clienteAtualizado);

            NotificarInfo("Locatário atualizado com sucesso!");
            FecharLocatarioEditar();
            await CarregarLocatariosAsync();
        }

        private void VisualizarFiador(object parameter)
        {
            if (!TryGetId(parameter, out var id))
            {
                NotificarInfo("Selecione um fiador para visualizar.");
                return;
            }

            var fiador = ObterClientePorId(Fiadores, id);
            if (fiador == null)
            {
                NotificarInfo("Fiador não encontrado.");
                return;
            }

            _fiadorEditarId = fiador.Id;
            PreencherFormularioClienteEditar(fiador);
            FiadorModalEditarVisibility = Visibility.Visible;
        }

        private async void VisualizarImovel(object parameter)
        {
            if (!TryGetId(parameter, out var id))
            {
                NotificarInfo("Selecione um imóvel para visualizar.");
                return;
            }

            var imovel = Imoveis.FirstOrDefault(x => x != null && x.Id == id);
            if (imovel == null)
            {
                NotificarInfo("Imóvel não encontrado.");
                return;
            }

            var detalhes = await ImovelDAO.GetImovelPorId(id, Sistema.HttpClientFixo);
            if (detalhes != null)
            {
                if (detalhes.Proprietario == null || detalhes.Proprietario.Id <= 0)
                {
                    detalhes.Proprietario = imovel.Proprietario;
                }

                if (detalhes.Intencao == null || detalhes.Intencao.Id <= 0)
                {
                    detalhes.Intencao = imovel.Intencao;
                }

                if (detalhes.TipoImovel == null || detalhes.TipoImovel.Id <= 0)
                {
                    detalhes.TipoImovel = imovel.TipoImovel;
                }

                if (detalhes.Finalidade == null || detalhes.Finalidade.Id <= 0)
                {
                    detalhes.Finalidade = imovel.Finalidade;
                }

                imovel = detalhes;
            }

            ImovelSelecionado = imovel;
            await CarregarCombosImovelEditarAsync();
            PreencherFormularioImovelEditar(imovel);
            ImovelModalEditarVisibility = Visibility.Visible;
        }

        private async Task VisualizarContratoAsync(object parameter)
        {
            if (!TryGetId(parameter, out var id))
            {
                NotificarInfo("Selecione um contrato para visualizar.");
                return;
            }

            var contrato = Contratos.FirstOrDefault(x => x != null && x.Id == id);
            if (contrato == null)
            {
                NotificarInfo("Contrato não encontrado.");
                return;
            }

            var detalhes = await ContratoDAO.GetContratoPorId(id, Sistema.HttpClientFixo);
            if (detalhes != null)
            {
                contrato = detalhes;
            }

            ContratoSelecionado = contrato;
            await CarregarCombosContratoAsync();
            PreencherFormularioContratoVisualizar(contrato);
            ContratoModalVisualizarVisibility = Visibility.Visible;
        }

        private async Task AdicionarFotosImovelAsync()
        {
            var openFileDlg = new OpenFileDialog
            {
                Multiselect = true
            };

            var result = openFileDlg.ShowDialog();
            if (result != true)
            {
                return;
            }

            foreach (var fileName in openFileDlg.FileNames)
            {
                if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
                {
                    continue;
                }

                if (!ArquivoImagemSuportado(fileName))
                {
                    NotificarInfo($"Arquivo '{fileName}' não é um formato de imagem suportado.");
                    continue;
                }

                var binFoto = File.ReadAllBytes(fileName);
                _fotosSelecionadasBinario.Add(binFoto);
                _fotoIdsPreview.Add(null);
                FotosSelecionadasPreview.Add(CriarImagemPreview(binFoto));
            }

            await Task.CompletedTask;
        }

        private void RemoverFoto(object parameter)
        {
            if (parameter is not ImageSource imageSource)
            {
                return;
            }

            var index = FotosSelecionadasPreview.IndexOf(imageSource);
            if (index < 0)
            {
                return;
            }

            FotosSelecionadasPreview.RemoveAt(index);

            if (index < _fotosSelecionadasBinario.Count)
            {
                _fotosSelecionadasBinario.RemoveAt(index);
            }

            if (index < _fotoIdsPreview.Count)
            {
                var fotoId = _fotoIdsPreview[index];
                _fotoIdsPreview.RemoveAt(index);
                if (fotoId.HasValue)
                {
                    _fotosRemovidas.Add(fotoId.Value);
                }
            }
        }

        private async Task SalvarFotosImovelCriarAsync()
        {
            if (_idImovelCadastrado <= 0)
            {
                NotificarErro("Nenhum imóvel selecionado para cadastrar fotos.");
                return;
            }

            for (var i = 0; i < _fotosSelecionadasBinario.Count; i++)
            {
                var foto = new FotoDTO
                {
                    ImovelId = _idImovelCadastrado,
                    Bin = _fotosSelecionadasBinario[i],
                    NomeArquivo = $"ImovelId[{_idImovelCadastrado}] - Foto[{i}]",
                    CadastradorId = UsuarioLogadoId,
                    TipoFoto = 1,
                    Principal = false
                };

                await _crudService.CadastrarFotoAsync(foto);
            }

            NotificarInfo("Fotos cadastradas com sucesso!");
            LimparEstadoFotos();
            ImovelFotosModalCriarVisibility = Visibility.Hidden;
        }

        private async Task SalvarImovelCriarAsync()
        {
            if (string.IsNullOrWhiteSpace(ImovelProprietarioCriarSelecionado) ||
                string.IsNullOrWhiteSpace(ImovelIntencaoCriarSelecionada) ||
                string.IsNullOrWhiteSpace(ImovelTipoCriarSelecionado) ||
                string.IsNullOrWhiteSpace(ImovelFinalidadeCriarSelecionada) ||
                string.IsNullOrWhiteSpace(ImovelCepCriar) ||
                string.IsNullOrWhiteSpace(ImovelLogradouroCriar) ||
                string.IsNullOrWhiteSpace(ImovelNumeroCriar) ||
                string.IsNullOrWhiteSpace(ImovelPaisCriar) ||
                string.IsNullOrWhiteSpace(ImovelEstadoCriar) ||
                string.IsNullOrWhiteSpace(ImovelCidadeCriar) ||
                string.IsNullOrWhiteSpace(ImovelBairroCriar))
            {
                NotificarErro("Por favor, preencha todos os campos obrigatórios. (*)");
                return;
            }

            if (string.IsNullOrWhiteSpace(ImovelValorLocacaoCriar) && string.IsNullOrWhiteSpace(ImovelValorVendaCriar))
            {
                NotificarErro("Por favor, preencha pelo menos Valor de Venda ou Valor de Locação.");
                return;
            }

            var numeroValido = int.TryParse(ImovelNumeroCriar, out var numero);
            var metragemValida = decimal.TryParse(ImovelMetragemCriar, NumberStyles.Number, CultureInfo.CurrentCulture, out var metragem);
            var valorVendaValido = TryParseDecimal(ImovelValorVendaCriar, out var valorVenda);
            var valorLocacaoValido = TryParseDecimal(ImovelValorLocacaoCriar, out var valorLocacao);
            var condominioValido = TryParseDecimal(ImovelCondominioCriar, out var condominio);
            var iptuValido = TryParseDecimal(ImovelIptuCriar, out var iptu);
            var taxaIncendioValida = TryParseDecimal(ImovelTaxaIncendioCriar, out var taxaIncendio);
            var foroValido = TryParseDecimal(ImovelForoCriar, out var foro);

            if (!numeroValido || !metragemValida || !valorVendaValido || !valorLocacaoValido || !condominioValido || !iptuValido || !taxaIncendioValida || !foroValido)
            {
                NotificarErro("Um ou mais valores numéricos estão inválidos.");
                return;
            }

            var dto = new ImovelDTO
            {
                Proprietario = ObterIdCatalogoPorNome(_catalogoProprietariosImovel, ImovelProprietarioCriarSelecionado),
                TipoImovel = ObterIdCatalogoPorNome(_catalogoTiposImovel, ImovelTipoCriarSelecionado),
                Intencao = ObterIdCatalogoPorNome(_catalogoIntencoesImovel, ImovelIntencaoCriarSelecionada),
                Finalidade = ObterIdCatalogoPorNome(_catalogoFinalidadesImovel, ImovelFinalidadeCriarSelecionada),
                Cep = ImovelCepCriar,
                Logradouro = ImovelLogradouroCriar,
                Numero = numero,
                Bairro = ImovelBairroCriar,
                Cidade = ImovelCidadeCriar,
                Estado = ImovelEstadoCriar,
                Pais = ImovelPaisCriar,
                Complemento = ImovelComplementoCriar,
                Metragem = metragem,
                ValorVenda = valorVenda,
                ValorLocacao = valorLocacao,
                Condominio = condominio,
                Iptu = iptu,
                TaxaIncendio = taxaIncendio,
                Foro = foro,
                Observacao = ImovelObservacoesCriar,
                Descricao = ImovelDescricaoCriar,
                InscricaoIptu = ImovelInscricaoIptuCriar,
                NumeroCbmerj = ImovelNumeroCbmerjCriar,
                Cadastrador = UsuarioLogadoId
            };

            var idImovel = await _crudService.CadastrarImovelAsync(dto);
            DefinirImovelCadastrado(idImovel);

            NotificarInfo("Imóvel cadastrado com sucesso!");
            ImovelModalCriarVisibility = Visibility.Hidden;
            await CarregarImoveisAsync();
            ImovelFotosModalCriarVisibility = Visibility.Visible;
        }

        private async Task SalvarImovelEditarAsync()
        {
            var imovelSelecionado = ImovelSelecionado;
            if (imovelSelecionado == null || imovelSelecionado.Id <= 0)
            {
                NotificarInfo("Selecione um imóvel para editar.");
                return;
            }

            if (!int.TryParse(ImovelNumeroEditar, out var numero) ||
                !decimal.TryParse(ImovelMetragemEditar, NumberStyles.Number, CultureInfo.CurrentCulture, out var metragem) ||
                !TryParseDecimal(ImovelValorVendaEditar, out var valorVenda) ||
                !TryParseDecimal(ImovelValorLocacaoEditar, out var valorLocacao) ||
                !TryParseDecimal(ImovelCondominioEditar, out var condominio) ||
                !TryParseDecimal(ImovelIptuEditar, out var iptu) ||
                !TryParseDecimal(ImovelTaxaIncendioEditar, out var taxaIncendio) ||
                !TryParseDecimal(ImovelForoEditar, out var foro))
            {
                NotificarErro("Existem valores inválidos no imóvel selecionado.");
                return;
            }

            var dto = new ImovelDTO
            {
                TaxaIncendio = taxaIncendio,
                Foro = foro,
                Iptu = iptu,
                ValorVenda = valorVenda,
                ValorLocacao = valorLocacao,
                Metragem = metragem,
                Descricao = ImovelDescricaoEditar,
                Observacao = ImovelObservacoesEditar,
                Condominio = condominio,
                Complemento = ImovelComplementoEditar,
                Bairro = ImovelBairroEditar,
                Cidade = ImovelCidadeEditar,
                Estado = ImovelEstadoEditar,
                Pais = ImovelPaisEditar,
                Numero = numero,
                Logradouro = ImovelLogradouroEditar,
                Cep = ImovelCepEditar,
                InscricaoIptu = ImovelInscricaoIptuEditar,
                NumeroCbmerj = ImovelNumeroCbmerjEditar,
                Intencao = ObterIdCatalogoPorNome(_catalogoIntencoesImovel, ImovelIntencaoEditarSelecionada),
                TipoImovel = ObterIdCatalogoPorNome(_catalogoTiposImovel, ImovelTipoEditarSelecionado),
                Finalidade = ObterIdCatalogoPorNome(_catalogoFinalidadesImovel, ImovelFinalidadeEditarSelecionada),
                Proprietario = ObterIdCatalogoPorNome(_catalogoProprietariosImovel, ImovelProprietarioEditarSelecionado)
            };

            await _crudService.AtualizarImovelAsync(imovelSelecionado.Id, dto);

            NotificarInfo("Imóvel atualizado com sucesso!");
            ImovelModalEditarVisibility = Visibility.Hidden;
            await CarregarImoveisAsync();
        }

        private async Task SalvarContratoCriarAsync()
        {
            if (!int.TryParse(ContratoPrazoMesesCriar, out var prazo))
            {
                NotificarErro("Prazo inválido. Utilize apenas números.");
                return;
            }

            if (!int.TryParse(ContratoVencimentoCriar, out var vencimento) || vencimento < 1 || vencimento > 31)
            {
                NotificarErro("Vencimento inválido. Utilize apenas números de 1 a 31.");
                return;
            }

            if (!decimal.TryParse(ContratoValorCriar, NumberStyles.Number, CultureInfo.CurrentCulture, out var valorContrato))
            {
                NotificarErro("Valor do contrato inválido. Utilize apenas números.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ContratoNomeCriar)
                || ContratoTipoCriarSelecionado == null
                || ContratoModalidadeCriarSelecionada == null
                || ContratoObjetoCriarSelecionado == null
                || ContratoProprietarioCriarSelecionado == null
                || ContratoImovelCriarSelecionado == null
                || ContratoContratante1CriarSelecionado == null
                || !ContratoDataInicioCriar.HasValue)
            {
                NotificarErro("Por favor, preencha todos os campos obrigatórios.");
                return;
            }

            if (PossuiContratantesRepetidos(ContratoContratante1CriarSelecionado, ContratoContratante2CriarSelecionado, ContratoContratante3CriarSelecionado, ContratoContratante4CriarSelecionado))
            {
                NotificarErro("Não é permitido repetir contratantes.");
                return;
            }

            var modalidadeEhSeguroFianca = ModalidadeEhSeguroFianca(ContratoModalidadeCriarSelecionada);
            var modalidadeEhFiador = ModalidadeEhFiador(ContratoModalidadeCriarSelecionada);

            var proposta = modalidadeEhSeguroFianca ? TratarTextoOpcional(ContratoPropostaSegFiancaCriar) : null;
            var apolice = modalidadeEhSeguroFianca ? TratarTextoOpcional(ContratoApoliceSegFiancaCriar) : null;
            var fiador = modalidadeEhFiador ? ContratoFiadorCriarSelecionado : null;
            var fiador2 = modalidadeEhFiador ? ContratoFiador2CriarSelecionado : null;

            var contrato = new ContratoDTO
            {
                Nome = ContratoNomeCriar,
                Cadastrador = new UsuarioDAO { Id = UsuarioLogadoId },
                TipoContrato = new TipoContratoDAO { Id = ContratoTipoCriarSelecionado.Id },
                ModalidadeContrato = new ModalidadeContratoDAO { Id = ContratoModalidadeCriarSelecionada.Id },
                ObjetoContrato = new ObjetoContratoDAO { Id = ContratoObjetoCriarSelecionado.Id },
                Proprietario = new ClienteDAO { Id = ContratoProprietarioCriarSelecionado.Id },
                Imovel = new ImovelDAO { Id = ContratoImovelCriarSelecionado.Id },
                Contratante1 = new ClienteDAO { Id = ContratoContratante1CriarSelecionado.Id },
                Contratante2 = ContratoContratante2CriarSelecionado != null ? new ClienteDAO { Id = ContratoContratante2CriarSelecionado.Id } : null,
                Contratante3 = ContratoContratante3CriarSelecionado != null ? new ClienteDAO { Id = ContratoContratante3CriarSelecionado.Id } : null,
                Contratante4 = ContratoContratante4CriarSelecionado != null ? new ClienteDAO { Id = ContratoContratante4CriarSelecionado.Id } : null,
                Fiador = fiador != null ? new ClienteDAO { Id = fiador.Id } : null,
                Fiador2 = fiador2 != null ? new ClienteDAO { Id = fiador2.Id } : null,
                ValorContrato = valorContrato,
                DataInicioVigencia = ContratoDataInicioCriar.Value,
                PrazoMeses = prazo,
                Vencimento = vencimento,
                PropostaSegFianca = proposta,
                ApoliceSegFianca = apolice
            };

            await _crudService.CadastrarContratoAsync(contrato);
            NotificarInfo("Contrato cadastrado com sucesso!");

            ContratoModalCriarVisibility = Visibility.Hidden;
            LimparFormularioContratoCriar();
            await CarregarContratosAsync();
        }

        private async Task SalvarContratoVisualizarAsync()
        {
            var contratoSelecionado = ContratoSelecionado;
            if (contratoSelecionado == null || contratoSelecionado.Id <= 0)
            {
                NotificarErro("Nenhum contrato selecionado.");
                return;
            }

            if (!int.TryParse(ContratoPrazoMesesVisualizar, out var prazo))
            {
                NotificarErro("Prazo inválido. Utilize apenas números.");
                return;
            }

            if (!int.TryParse(ContratoVencimentoVisualizar, out var vencimento) || vencimento < 1 || vencimento > 31)
            {
                NotificarErro("Vencimento inválido. Utilize apenas números de 1 a 31.");
                return;
            }

            if (!decimal.TryParse(ContratoValorVisualizar, NumberStyles.Number, CultureInfo.CurrentCulture, out var valorContrato))
            {
                NotificarErro("Valor do contrato inválido. Utilize apenas números.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ContratoNomeVisualizar)
                || ContratoTipoVisualizarSelecionado == null
                || ContratoModalidadeVisualizarSelecionada == null
                || ContratoObjetoVisualizarSelecionado == null
                || ContratoProprietarioVisualizarSelecionado == null
                || ContratoImovelVisualizarSelecionado == null
                || ContratoContratante1VisualizarSelecionado == null
                || !ContratoDataInicioVisualizar.HasValue)
            {
                NotificarErro("Por favor, preencha todos os campos obrigatórios.");
                return;
            }

            if (PossuiContratantesRepetidos(ContratoContratante1VisualizarSelecionado, ContratoContratante2VisualizarSelecionado, ContratoContratante3VisualizarSelecionado, ContratoContratante4VisualizarSelecionado))
            {
                NotificarErro("Não é permitido repetir contratantes.");
                return;
            }

            var modalidadeEhSeguroFianca = ModalidadeEhSeguroFianca(ContratoModalidadeVisualizarSelecionada);
            var modalidadeEhFiador = ModalidadeEhFiador(ContratoModalidadeVisualizarSelecionada);

            var proposta = modalidadeEhSeguroFianca ? TratarTextoOpcional(ContratoPropostaSegFiancaVisualizar) : null;
            var apolice = modalidadeEhSeguroFianca ? TratarTextoOpcional(ContratoApoliceSegFiancaVisualizar) : null;
            var fiador = modalidadeEhFiador ? ContratoFiadorVisualizarSelecionado : null;
            var fiador2 = modalidadeEhFiador ? ContratoFiador2VisualizarSelecionado : null;

            var contratoAtualizado = new ContratoDTO
            {
                Id = contratoSelecionado.Id,
                Nome = ContratoNomeVisualizar,
                Cadastrador = new UsuarioDAO { Id = UsuarioLogadoId },
                TipoContrato = new TipoContratoDAO { Id = ContratoTipoVisualizarSelecionado.Id },
                ModalidadeContrato = new ModalidadeContratoDAO { Id = ContratoModalidadeVisualizarSelecionada.Id },
                ObjetoContrato = new ObjetoContratoDAO { Id = ContratoObjetoVisualizarSelecionado.Id },
                Proprietario = new ClienteDAO { Id = ContratoProprietarioVisualizarSelecionado.Id },
                Imovel = new ImovelDAO { Id = ContratoImovelVisualizarSelecionado.Id },
                Contratante1 = new ClienteDAO { Id = ContratoContratante1VisualizarSelecionado.Id },
                Contratante2 = ContratoContratante2VisualizarSelecionado != null ? new ClienteDAO { Id = ContratoContratante2VisualizarSelecionado.Id } : null,
                Contratante3 = ContratoContratante3VisualizarSelecionado != null ? new ClienteDAO { Id = ContratoContratante3VisualizarSelecionado.Id } : null,
                Contratante4 = ContratoContratante4VisualizarSelecionado != null ? new ClienteDAO { Id = ContratoContratante4VisualizarSelecionado.Id } : null,
                Fiador = fiador != null ? new ClienteDAO { Id = fiador.Id } : null,
                Fiador2 = fiador2 != null ? new ClienteDAO { Id = fiador2.Id } : null,
                ValorContrato = valorContrato,
                DataInicioVigencia = ContratoDataInicioVisualizar.Value,
                PrazoMeses = prazo,
                Vencimento = vencimento,
                PropostaSegFianca = proposta,
                ApoliceSegFianca = apolice
            };

            await _crudService.AtualizarContratoAsync(contratoSelecionado.Id, contratoAtualizado);
            NotificarInfo("Contrato atualizado com sucesso!");

            ContratoModalVisualizarVisibility = Visibility.Hidden;
            await CarregarContratosAsync();
        }

        private async Task SalvarFotosImovelEditarAsync()
        {
            if (_idImovelCadastrado <= 0)
            {
                NotificarErro("Nenhum imóvel selecionado para atualizar fotos.");
                return;
            }

            foreach (var fotoId in _fotosRemovidas)
            {
                await _crudService.InativarFotoAsync(fotoId);
            }

            for (var i = 0; i < _fotosSelecionadasBinario.Count; i++)
            {
                if (_fotoIdsPreview.Count > i && _fotoIdsPreview[i].HasValue)
                {
                    continue;
                }

                var foto = new FotoDTO
                {
                    ImovelId = _idImovelCadastrado,
                    Bin = _fotosSelecionadasBinario[i],
                    NomeArquivo = $"ImovelId[{_idImovelCadastrado}] - Foto[{i}]",
                    CadastradorId = UsuarioLogadoId,
                    TipoFoto = 1,
                    Principal = false
                };

                await _crudService.CadastrarFotoAsync(foto);
            }

            NotificarInfo("Fotos atualizadas com sucesso!");
            LimparEstadoFotos();
            ImovelFotosModalEditarVisibility = Visibility.Hidden;
            ImovelModalEditarVisibility = Visibility.Visible;
        }

        private async Task AbrirFotosImovelAsync()
        {
            if (ImovelSelecionado == null || ImovelSelecionado.Id <= 0)
            {
                NotificarInfo("Selecione um imóvel para editar as fotos.");
                return;
            }

            _idImovelCadastrado = ImovelSelecionado.Id;

            ImovelModalEditarVisibility = Visibility.Hidden;
            ImovelFotosModalEditarVisibility = Visibility.Visible;
            LimparEstadoFotos();

            var fotosImovel = await _listagemService.ObterFotosPorImovelAsync(_idImovelCadastrado);
            foreach (var foto in fotosImovel)
            {
                if (foto?.Bin == null || foto.Bin.Length == 0)
                {
                    continue;
                }

                _fotosSelecionadasBinario.Add(foto.Bin);
                _fotoIdsPreview.Add(foto.Id);
                FotosSelecionadasPreview.Add(CriarImagemPreview(foto.Bin));
            }
        }

        private async Task SalvarFiadorCriarAsync()
        {
            if (string.IsNullOrWhiteSpace(ClienteNomeCriar) || string.IsNullOrWhiteSpace(ClienteCpfCnpjCriar))
            {
                NotificarErro("Por favor, preencha os campos obrigatórios. (*)");
                return;
            }

            var tipoClienteId = _crudService.ObterTipoClienteId("Fiador");
            if (tipoClienteId == 0)
            {
                NotificarErro("Tipo de cliente fiador não encontrado.");
                return;
            }

            var cliente = CriarClienteDtoCriar(tipoClienteId);
            await _crudService.CadastrarClienteAsync(cliente);

            NotificarInfo("Fiador cadastrado com sucesso!");
            FecharFiadorCriar();
            await CarregarFiadoresAsync();
        }

        private async Task SalvarFiadorEditarAsync()
        {
            if (!_fiadorEditarId.HasValue || _fiadorEditarId.Value <= 0)
            {
                NotificarInfo("Selecione um fiador para editar.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ClienteNomeEditar) || string.IsNullOrWhiteSpace(ClienteCpfCnpjEditar))
            {
                NotificarErro("Por favor, preencha os campos obrigatórios. (*)");
                return;
            }

            var tipoClienteId = _crudService.ObterTipoClienteId("Fiador");
            if (tipoClienteId == 0)
            {
                NotificarErro("Tipo de cliente fiador não encontrado.");
                return;
            }

            var clienteAtualizado = CriarClienteDtoEditar(tipoClienteId);
            await _crudService.AtualizarClienteAsync(_fiadorEditarId.Value, clienteAtualizado);

            NotificarInfo("Fiador atualizado com sucesso!");
            FecharFiadorEditar();
            await CarregarFiadoresAsync();
        }

        private static bool ModalidadeEhFiador(ModalidadeContratoDAO modalidade)
        {
            var nome = Normalizar(modalidade?.Nome);
            return nome.Contains("fiador");
        }

        private static bool ModalidadeEhSeguroFianca(ModalidadeContratoDAO modalidade)
        {
            var nome = Normalizar(modalidade?.Nome);
            return nome.Contains("seguro fianca") || nome.Contains("segurofianca");
        }

        private static bool PossuiContratantesRepetidos(params ClienteDAO[] contratantes)
        {
            var ids = contratantes
                .Where(c => c != null && c.Id > 0)
                .Select(c => c.Id)
                .ToList();

            return ids.Count != ids.Distinct().Count();
        }

        private static string TratarTextoOpcional(string texto)
        {
            return string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
        }

        private void LimparFormularioContratoCriar()
        {
            ContratoNomeCriar = string.Empty;
            ContratoTipoCriarSelecionado = null;
            ContratoModalidadeCriarSelecionada = null;
            ContratoObjetoCriarSelecionado = null;
            ContratoProprietarioCriarSelecionado = null;
            ContratoImovelCriarSelecionado = null;
            ContratoContratante1CriarSelecionado = null;
            ContratoContratante2CriarSelecionado = null;
            ContratoContratante3CriarSelecionado = null;
            ContratoContratante4CriarSelecionado = null;
            ContratoFiadorCriarSelecionado = null;
            ContratoFiador2CriarSelecionado = null;
            ContratoDataInicioCriar = null;
            ContratoPrazoMesesCriar = string.Empty;
            ContratoValorCriar = string.Empty;
            ContratoVencimentoCriar = string.Empty;
            ContratoPropostaSegFiancaCriar = string.Empty;
            ContratoApoliceSegFiancaCriar = string.Empty;
        }

        private void PreencherFormularioContratoVisualizar(ContratoDAO contrato)
        {
            ContratoNomeVisualizar = contrato.Nome ?? string.Empty;
            ContratoTipoVisualizarSelecionado = TiposContrato.FirstOrDefault(x => x.Id == ObterIdContrato(contrato.TipoContrato, contrato.TipoContratoId));
            if (ContratoTipoVisualizarSelecionado == null && contrato.TipoContrato != null)
            {
                ContratoTipoVisualizarSelecionado = TiposContrato.FirstOrDefault(x => string.Equals(x.Nome, contrato.TipoContrato.Nome, StringComparison.OrdinalIgnoreCase));
            }
            ContratoModalidadeVisualizarSelecionada = ModalidadesContrato.FirstOrDefault(x => x.Id == ObterIdContrato(contrato.ModalidadeContrato, contrato.ModalidadeContratoId));
            if (ContratoModalidadeVisualizarSelecionada == null && contrato.ModalidadeContrato != null)
            {
                ContratoModalidadeVisualizarSelecionada = ModalidadesContrato.FirstOrDefault(x => string.Equals(x.Nome, contrato.ModalidadeContrato.Nome, StringComparison.OrdinalIgnoreCase));
            }
            ContratoObjetoVisualizarSelecionado = ObjetosContrato.FirstOrDefault(x => x.Id == ObterIdContrato(contrato.ObjetoContrato, contrato.ObjetoContratoId));
            if (ContratoObjetoVisualizarSelecionado == null && contrato.ObjetoContrato != null)
            {
                ContratoObjetoVisualizarSelecionado = ObjetosContrato.FirstOrDefault(x => string.Equals(x.Nome, contrato.ObjetoContrato.Nome, StringComparison.OrdinalIgnoreCase));
            }
            ContratoProprietarioVisualizarSelecionado = SelecionarPorIdOuNome(
                ProprietariosContrato,
                ObterIdContrato(contrato.Proprietario, contrato.ProprietarioId),
                contrato.Proprietario?.Nome ?? contrato.NomeProprietario);
            ContratoImovelVisualizarSelecionado = SelecionarPorIdOuNome(
                ImoveisContrato,
                ObterIdContrato(contrato.Imovel, contrato.ImovelId),
                contrato.Imovel?.Nome ?? contrato.NomeImovel,
                x => x?.Logradouro);
            ContratoContratante1VisualizarSelecionado = SelecionarPorIdOuNome(LocatariosContrato, ObterIdContrato(contrato.Contratante1, contrato.Contratante1Id), contrato.Contratante1?.Nome ?? contrato.NomeContratante1);
            ContratoContratante2VisualizarSelecionado = SelecionarPorIdOuNome(LocatariosContrato, ObterIdContrato(contrato.Contratante2, contrato.Contratante2Id), contrato.Contratante2?.Nome ?? contrato.NomeContratante2);
            ContratoContratante3VisualizarSelecionado = SelecionarPorIdOuNome(LocatariosContrato, ObterIdContrato(contrato.Contratante3, contrato.Contratante3Id), contrato.Contratante3?.Nome ?? contrato.NomeContratante3);
            ContratoContratante4VisualizarSelecionado = SelecionarPorIdOuNome(LocatariosContrato, ObterIdContrato(contrato.Contratante4, contrato.Contratante4Id), contrato.Contratante4?.Nome ?? contrato.NomeContratante4);
            ContratoFiadorVisualizarSelecionado = SelecionarPorIdOuNome(FiadoresContrato, ObterIdContrato(contrato.Fiador, contrato.FiadorId), contrato.Fiador?.Nome ?? contrato.NomeFiador);
            ContratoFiador2VisualizarSelecionado = SelecionarPorIdOuNome(FiadoresContrato, ObterIdContrato(contrato.Fiador2, contrato.Fiador2Id), contrato.Fiador2?.Nome ?? contrato.NomeFiador2);

            ContratoDataInicioVisualizar = contrato.DataInicioVigencia;
            ContratoPrazoMesesVisualizar = contrato.PrazoMeses.ToString(CultureInfo.InvariantCulture);
            ContratoValorVisualizar = contrato.ValorContrato.ToString(CultureInfo.CurrentCulture);
            ContratoVencimentoVisualizar = contrato.Vencimento.ToString(CultureInfo.InvariantCulture);
            ContratoPropostaSegFiancaVisualizar = contrato.PropostaSegFianca ?? string.Empty;
            ContratoApoliceSegFiancaVisualizar = contrato.ApoliceSegFianca ?? string.Empty;
        }

        private static ClienteDAO ObterClientePorId(IEnumerable<ClienteDAO> origem, int id)
        {
            if (origem == null)
            {
                return null;
            }

            foreach (var cliente in origem)
            {
                if (cliente?.Id == id)
                {
                    return cliente;
                }
            }

            return null;
        }

        private static int ObterIdContrato(object entidadeRelacionada, int? idFallback)
        {
            if (entidadeRelacionada != null)
            {
                var propriedadeId = entidadeRelacionada.GetType().GetProperty("Id");
                if (propriedadeId != null)
                {
                    var valor = propriedadeId.GetValue(entidadeRelacionada);
                    switch (valor)
                    {
                        case int idInt when idInt > 0:
                            return idInt;
                        case long idLong when idLong > 0:
                            return (int)idLong;
                    }
                }
            }

            return idFallback.GetValueOrDefault();
        }

        private static T SelecionarPorIdOuNome<T>(IEnumerable<T> origem, int id, string nome, Func<T, string> nomeAlternativo = null)
            where T : class
        {
            if (origem == null)
            {
                return null;
            }

            var lista = origem.Where(x => x != null).ToList();

            if (id > 0)
            {
                var porId = lista.FirstOrDefault(x =>
                {
                    var prop = x.GetType().GetProperty("Id");
                    if (prop == null)
                    {
                        return false;
                    }

                    var valor = prop.GetValue(x);
                    return valor is int idInt && idInt == id;
                });

                if (porId != null)
                {
                    return porId;
                }
            }

            if (string.IsNullOrWhiteSpace(nome))
            {
                return null;
            }

            var comparador = nome.Trim();
            return lista.FirstOrDefault(x =>
            {
                var propNome = x.GetType().GetProperty("Nome");
                var nomePrincipal = propNome?.GetValue(x) as string;
                if (string.Equals(nomePrincipal, comparador, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (nomeAlternativo != null)
                {
                    var alt = nomeAlternativo(x);
                    return string.Equals(alt, comparador, StringComparison.OrdinalIgnoreCase);
                }

                return false;
            });
        }

        private int ObterIdCatalogoPorNome<T>(IEnumerable<T> origem, string nome)
            where T : class
        {
            if (origem == null || string.IsNullOrWhiteSpace(nome))
            {
                return 0;
            }

            var item = origem.FirstOrDefault(x =>
            {
                var propNome = x?.GetType().GetProperty("Nome");
                var valorNome = propNome?.GetValue(x) as string;
                return string.Equals(valorNome?.Trim(), nome.Trim(), StringComparison.OrdinalIgnoreCase);
            });

            if (item == null)
            {
                return 0;
            }

            var propId = item.GetType().GetProperty("Id");
            var valor = propId?.GetValue(item);
            return valor is int id ? id : 0;
        }

        private static string SelecionarNomePorIdOuNome<T>(IEnumerable<T> origem, int id, string nome)
            where T : class
        {
            if (origem != null && id > 0)
            {
                var porId = origem.FirstOrDefault(x =>
                {
                    var propId = x?.GetType().GetProperty("Id");
                    var valor = propId?.GetValue(x);
                    return valor is int idInt && idInt == id;
                });

                if (porId != null)
                {
                    var propNome = porId.GetType().GetProperty("Nome");
                    var nomeId = propNome?.GetValue(porId) as string;
                    if (!string.IsNullOrWhiteSpace(nomeId))
                    {
                        return nomeId;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(nome) || origem == null)
            {
                return nome ?? string.Empty;
            }

            var porNome = origem.FirstOrDefault(x =>
            {
                var propNome = x?.GetType().GetProperty("Nome");
                var valorNome = propNome?.GetValue(x) as string;
                return string.Equals(valorNome?.Trim(), nome.Trim(), StringComparison.OrdinalIgnoreCase);
            });

            if (porNome == null)
            {
                return nome;
            }

            var propNomeFinal = porNome.GetType().GetProperty("Nome");
            return propNomeFinal?.GetValue(porNome) as string ?? nome;
        }

        private ClienteDTO CriarClienteDtoCriar(int tipoClienteId)
        {
            return new ClienteDTO
            {
                Nome = ClienteNomeCriar,
                CpfCnpj = ClienteCpfCnpjCriar,
                Identidade = ClienteIdentidadeCriar,
                OrgaoExpedidor = ClienteOrgaoExpedidorCriar,
                Nacionalidade = ClienteNacionalidadeCriar,
                Naturalidade = ClienteNaturalidadeCriar,
                EstadoCivil = ClienteEstadoCivilCriar,
                Profissao = ClienteProfissaoCriar,
                Endereco = ClienteEnderecoCriar,
                Banco = ClienteBancoCriar,
                ChavePix = ClienteChavePixCriar,
                Agencia = ClienteAgenciaCriar,
                Conta = ClienteContaCriar,
                CodBanco = ClienteCodBancoCriar,
                Email = ClienteEmailCriar,
                Telefone = ClienteTelefoneCriar,
                DataNascimento = ClienteNascimentoCriar,
                TipoCliente = new TipoClienteDAO { Id = tipoClienteId },
                Cadastrador = new UsuarioDAO { Id = UsuarioLogadoId }
            };
        }

        private ClienteDTO CriarClienteDtoEditar(int tipoClienteId)
        {
            return new ClienteDTO
            {
                Nome = ClienteNomeEditar,
                CpfCnpj = ClienteCpfCnpjEditar,
                Identidade = ClienteIdentidadeEditar,
                OrgaoExpedidor = ClienteOrgaoExpedidorEditar,
                Nacionalidade = ClienteNacionalidadeEditar,
                Naturalidade = ClienteNaturalidadeEditar,
                EstadoCivil = ClienteEstadoCivilEditar,
                Profissao = ClienteProfissaoEditar,
                Endereco = ClienteEnderecoEditar,
                Banco = ClienteBancoEditar,
                ChavePix = ClienteChavePixEditar,
                Agencia = ClienteAgenciaEditar,
                Conta = ClienteContaEditar,
                CodBanco = ClienteCodBancoEditar,
                Email = ClienteEmailEditar,
                Telefone = ClienteTelefoneEditar,
                DataNascimento = ClienteNascimentoEditar,
                TipoCliente = new TipoClienteDAO { Id = tipoClienteId }
            };
        }

        private void PreencherFormularioClienteEditar(ClienteDAO cliente)
        {
            ClienteNomeEditar = cliente.Nome ?? string.Empty;
            ClienteCpfCnpjEditar = cliente.CpfCnpj ?? string.Empty;
            ClienteIdentidadeEditar = cliente.Identidade ?? string.Empty;
            ClienteOrgaoExpedidorEditar = cliente.OrgaoExpedidor ?? string.Empty;
            ClienteNacionalidadeEditar = cliente.Nacionalidade ?? string.Empty;
            ClienteNaturalidadeEditar = cliente.Naturalidade ?? string.Empty;
            ClienteEstadoCivilEditar = cliente.EstadoCivil ?? string.Empty;
            ClienteProfissaoEditar = cliente.Profissao ?? string.Empty;
            ClienteEnderecoEditar = cliente.Endereco ?? string.Empty;
            ClienteBancoEditar = cliente.Banco ?? string.Empty;
            ClienteChavePixEditar = cliente.ChavePix ?? string.Empty;
            ClienteAgenciaEditar = cliente.Agencia ?? string.Empty;
            ClienteContaEditar = cliente.Conta ?? string.Empty;
            ClienteCodBancoEditar = cliente.CodBanco ?? string.Empty;
            ClienteEmailEditar = cliente.Email ?? string.Empty;
            ClienteTelefoneEditar = cliente.Telefone ?? string.Empty;
            ClienteNascimentoEditar = cliente.DataNascimento;
        }

        private void LimparFormularioClienteCriar()
        {
            ClienteNomeCriar = string.Empty;
            ClienteCpfCnpjCriar = string.Empty;
            ClienteIdentidadeCriar = string.Empty;
            ClienteOrgaoExpedidorCriar = string.Empty;
            ClienteNacionalidadeCriar = string.Empty;
            ClienteNaturalidadeCriar = string.Empty;
            ClienteEstadoCivilCriar = string.Empty;
            ClienteProfissaoCriar = string.Empty;
            ClienteEnderecoCriar = string.Empty;
            ClienteBancoCriar = string.Empty;
            ClienteChavePixCriar = string.Empty;
            ClienteAgenciaCriar = string.Empty;
            ClienteContaCriar = string.Empty;
            ClienteCodBancoCriar = string.Empty;
            ClienteEmailCriar = string.Empty;
            ClienteTelefoneCriar = string.Empty;
            ClienteNascimentoCriar = null;
        }

        private static bool TryGetId(object parameter, out int id)
        {
            switch (parameter)
            {
                case int valorInt:
                    id = valorInt;
                    return id > 0;
                case string valorString when int.TryParse(valorString, out var parsed):
                    id = parsed;
                    return id > 0;
                default:
                    id = 0;
                    return false;
            }
        }

        private void AtualizarProprietariosView()
        {
            ProprietariosView = CollectionViewSource.GetDefaultView(Proprietarios);
            ProprietariosView.Filter = item => FiltrarCliente(item as ClienteDAO, SearchProprietarios);
        }

        private void AtualizarLocatariosView()
        {
            LocatariosView = CollectionViewSource.GetDefaultView(Locatarios);
            LocatariosView.Filter = item => FiltrarCliente(item as ClienteDAO, SearchLocatarios);
        }

        private void AtualizarFiadoresView()
        {
            FiadoresView = CollectionViewSource.GetDefaultView(Fiadores);
            FiadoresView.Filter = item => FiltrarCliente(item as ClienteDAO, SearchFiadores);
        }

        private void AtualizarImoveisView()
        {
            ImoveisView = CollectionViewSource.GetDefaultView(Imoveis);
            ImoveisView.Filter = item =>
            {
                var imovel = item as ImovelDAO;
                if (imovel == null)
                {
                    return false;
                }

                var filtro = Normalizar(SearchImoveis);
                if (string.IsNullOrWhiteSpace(filtro))
                {
                    return true;
                }

                return ContemTexto(imovel.Nome, filtro)
                    || ContemTexto(imovel.NomeProprietario, filtro)
                    || ContemTexto(imovel.NomeTipoImovel, filtro)
                    || ContemTexto(imovel.NomeIntencao, filtro)
                    || ContemTexto(imovel.Logradouro, filtro)
                    || ContemTexto(imovel.Bairro, filtro);
            };
        }

        private void AtualizarContratosView()
        {
            ContratosView = CollectionViewSource.GetDefaultView(Contratos);
            ContratosView.Filter = item =>
            {
                var contrato = item as ContratoDAO;
                if (contrato == null)
                {
                    return false;
                }

                var filtro = Normalizar(SearchContratos);
                if (string.IsNullOrWhiteSpace(filtro))
                {
                    return true;
                }

                return ContemTexto(contrato.Nome, filtro)
                    || ContemTexto(contrato.NomeTipoContrato, filtro)
                    || ContemTexto(contrato.NomeProprietario, filtro)
                    || ContemTexto(contrato.NomeImovel, filtro);
            };
        }

        private static bool FiltrarCliente(ClienteDAO cliente, string termo)
        {
            if (cliente == null)
            {
                return false;
            }

            var filtro = Normalizar(termo);
            if (string.IsNullOrWhiteSpace(filtro))
            {
                return true;
            }

            return ContemTexto(cliente.Nome, filtro)
                || ContemTexto(cliente.CpfCnpj, filtro)
                || ContemTexto(cliente.Email, filtro)
                || ContemTexto(cliente.Telefone, filtro)
                || ContemTexto(cliente.Endereco, filtro);
        }

        private static bool ContemTexto(string valor, string termo)
        {
            return Normalizar(valor).Contains(termo);
        }

        private static bool ArquivoImagemSuportado(string fileName)
        {
            return fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                   || fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                   || fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                   || fileName.EndsWith(".webp", StringComparison.OrdinalIgnoreCase)
                   || fileName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryParseDecimal(string text, out decimal value)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                value = 0m;
                return true;
            }

            return decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out value);
        }

        private static BitmapImage CriarImagemPreview(byte[] bytes)
        {
            var bitmap = new BitmapImage();
            using var stream = new MemoryStream(bytes);
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze();
            return bitmap;
        }

        private void LimparEstadoFotos()
        {
            FotosSelecionadasPreview.Clear();
            _fotosSelecionadasBinario.Clear();
            _fotoIdsPreview.Clear();
            _fotosRemovidas.Clear();
            _idImovelCadastrado = 0;
        }

        private static string Normalizar(string valor)
        {
            return (valor ?? string.Empty).Trim().ToLowerInvariant();
        }

        private void NotificarErro(string mensagem)
        {
            ShowErrorAction?.Invoke(mensagem);
        }

        private void NotificarInfo(string mensagem)
        {
            ShowInfoAction?.Invoke(mensagem);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
