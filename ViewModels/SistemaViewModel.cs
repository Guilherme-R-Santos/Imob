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

        private ICollectionView _proprietariosView;
        private ICollectionView _locatariosView;
        private ICollectionView _fiadoresView;
        private ICollectionView _imoveisView;
        private ICollectionView _contratosView;

        public ObservableCollection<ImageSource> FotosSelecionadasPreview { get; } = new ObservableCollection<ImageSource>();

        public Action<string> ShowErrorAction { get; set; }
        public Action<string> ShowInfoAction { get; set; }
        public int UsuarioLogadoId { get; set; }


        public Action SalvarContratoCriarRequested { get; set; }

        public Action SalvarContratoVisualizarRequested { get; set; }





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
            VisualizarContratoCommand = new RelayCommandWithParameter(VisualizarContrato);
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
            AbrirContratoCriarCommand = new RelayCommand(AbrirContratoCriar);
            FecharContratoCriarCommand = new RelayCommand(FecharContratoCriar);
            SalvarContratoCriarCommand = new RelayCommand(() => SalvarContratoCriarRequested?.Invoke());
            GerarContratoVisualizarCommand = new RelayCommand(GerarContratoVisualizar);
            FecharContratoVisualizarCommand = new RelayCommand(FecharContratoVisualizar);
            SalvarContratoVisualizarCommand = new RelayCommand(() => SalvarContratoVisualizarRequested?.Invoke());

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

        private void FecharContratoCriar()
        {
            ContratoModalCriarVisibility = Visibility.Hidden;
        }

        private void FecharContratoVisualizar()
        {
            ContratoModalVisualizarVisibility = Visibility.Hidden;
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

            ImovelProprietariosCriar = new ObservableCollection<string>((await proprietariosTask).Where(x => x != null).Select(x => x.Nome ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList());
            ImovelIntencoesCriar = new ObservableCollection<string>((await intencoesTask).Where(x => x != null).Select(x => x.Nome ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList());
            ImovelTiposCriar = new ObservableCollection<string>((await tiposTask).Where(x => x != null).Select(x => x.Nome ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList());
            ImovelFinalidadesCriar = new ObservableCollection<string>((await finalidadesTask).Where(x => x != null).Select(x => x.Nome ?? string.Empty).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList());
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

        private void VisualizarImovel(object parameter)
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

            ImovelSelecionado = imovel;
            ImovelModalEditarVisibility = Visibility.Visible;
        }

        private void VisualizarContrato(object parameter)
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

            ContratoSelecionado = contrato;
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
                Proprietario = ClienteDAO.GetIdPorNome(ImovelProprietarioCriarSelecionado, Sistema.HttpClientFixo),
                TipoImovel = TipoImovelDAO.GetIdPorNome(ImovelTipoCriarSelecionado, Sistema.HttpClientFixo),
                Intencao = IntencaoDAO.GetIdPorNome(ImovelIntencaoCriarSelecionada, Sistema.HttpClientFixo),
                Finalidade = Imob.Models.DAOs.FinalidadeDAO.GetIdPorNome(ImovelFinalidadeCriarSelecionada, Sistema.HttpClientFixo),
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

            if (!int.TryParse(ImovelSelecionado.Numero.ToString(CultureInfo.InvariantCulture), out var numero) ||
                !decimal.TryParse(ImovelSelecionado.Metragem.ToString(CultureInfo.InvariantCulture), NumberStyles.Number, CultureInfo.InvariantCulture, out var metragem) ||
                !TryParseDecimal(ImovelSelecionado.ValorVenda?.ToString(CultureInfo.InvariantCulture), out var valorVenda) ||
                !TryParseDecimal(ImovelSelecionado.ValorLocacao?.ToString(CultureInfo.InvariantCulture), out var valorLocacao) ||
                !TryParseDecimal(ImovelSelecionado.Condominio?.ToString(CultureInfo.InvariantCulture), out var condominio) ||
                !TryParseDecimal(ImovelSelecionado.Iptu?.ToString(CultureInfo.InvariantCulture), out var iptu) ||
                !TryParseDecimal(ImovelSelecionado.TaxaIncendio?.ToString(CultureInfo.InvariantCulture), out var taxaIncendio) ||
                !TryParseDecimal(ImovelSelecionado.Foro?.ToString(CultureInfo.InvariantCulture), out var foro))
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
                Descricao = ImovelSelecionado.Descricao,
                Observacao = ImovelSelecionado.Observacao,
                Condominio = condominio,
                Complemento = ImovelSelecionado.Complemento,
                Bairro = ImovelSelecionado.Bairro,
                Cidade = ImovelSelecionado.Cidade,
                Estado = ImovelSelecionado.Estado,
                Pais = ImovelSelecionado.Pais,
                Numero = numero,
                Logradouro = ImovelSelecionado.Logradouro,
                Cep = ImovelSelecionado.Cep,
                InscricaoIptu = ImovelSelecionado.InscricaoIptu,
                NumeroCbmerj = ImovelSelecionado.NumeroCbmerj,
                Intencao = imovelSelecionado.Intencao?.Id ?? 0,
                TipoImovel = imovelSelecionado.TipoImovel?.Id ?? 0,
                Finalidade = imovelSelecionado.Finalidade?.Id ?? 0,
                Proprietario = imovelSelecionado.Proprietario?.Id ?? 0
            };

            await _crudService.AtualizarImovelAsync(imovelSelecionado.Id, dto);

            NotificarInfo("Imóvel atualizado com sucesso!");
            ImovelModalEditarVisibility = Visibility.Hidden;
            await CarregarImoveisAsync();
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
