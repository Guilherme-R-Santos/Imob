using Imob.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.IO;
using System.IO.Enumeration;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using System.Linq;
using System.Globalization;
using Imob.Services;
using Imob.Services.Pdf;
using Imob.ViewModels;

namespace Imob
{
    public partial class Sistema : Window
    {
        private readonly SistemaViewModel _viewModel;

        private const int TokenSafetyMarginMinutes = 5;

        public static HttpClient HttpClientFixo { get; } = CriarHttpClient();

        public UsuarioDAO UsuarioLogado { get; set; }
        public string TokenJwt { get; private set; }
        public DateTime? TokenExpiration { get; private set; }

        private string _loginSessao;
        private string _senhaSessao;
        private readonly SemaphoreSlim _tokenRefreshSemaphore = new SemaphoreSlim(1, 1);
        private readonly DispatcherTimer _tokenRefreshTimer;

        private ObservableCollection<ImageSource> _fotosSelecionadasPreview = new ObservableCollection<ImageSource>();
        private List<byte[]> _fotosSelecionadasBinario = new List<byte[]>();
        private List<int?> _fotoIdsPreview = new List<int?>();
        private List<int> _fotosRemovidas = new List<int>();
        private int _idImovelCadastrado;

        private class LoginJwtResponse
        {
            [JsonProperty("token")]
            public string Token { get; set; }

            [JsonProperty("expiration")]
            public DateTime? Expiration { get; set; }

            [JsonProperty("tipo")]
            public string Tipo { get; set; }
        }

        private List<ClienteDAO> _proprietariosContratoCriar = new List<ClienteDAO>();
        private List<ImovelDAO> _imoveisContratoCriar = new List<ImovelDAO>();
        private List<ClienteDAO> _locatariosContratoCriar = new List<ClienteDAO>();

        private List<ClienteDAO> _proprietariosContratoVisualizar = new List<ClienteDAO>();
        private List<ImovelDAO> _imoveisContratoVisualizar = new List<ImovelDAO>();
        private List<ClienteDAO> _locatariosContratoVisualizar = new List<ClienteDAO>();

        private bool _atualizandoCombosContratoCriar;
        private bool _atualizandoCombosContratoVisualizar;

        private static HttpClient CriarHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var baseUrlApi = App.Configuration["Api:BaseUrl"];
            client.BaseAddress = new Uri(baseUrlApi ?? throw new InvalidOperationException("Configuração 'Api:BaseUrl' não encontrada."));
            return client;
        }

        private void AtualizarCabecalhosAutenticacao()
        {
            HttpClientFixo.DefaultRequestHeaders.Authorization = null;
            HttpClientFixo.DefaultRequestHeaders.Remove("X-Token-Expiration");

            if (!string.IsNullOrWhiteSpace(TokenJwt))
            {
                HttpClientFixo.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenJwt);
            }

            if (TokenExpiration.HasValue)
            {
                HttpClientFixo.DefaultRequestHeaders.Add("X-Token-Expiration", TokenExpiration.Value.ToUniversalTime().ToString("O"));
            }
        }

        public void SetUsuarioLogado(UsuarioDAO usuarioLogado)
        {
            UsuarioLogado = usuarioLogado;
            _viewModel.UsuarioLogadoId = usuarioLogado?.Id ?? 0;
            UsuarioAtivo.Content = UsuarioLogado.Login;
        }

        public void SetAutenticacao(string tokenJwt, DateTime? tokenExpiration, string loginSessao = null, string senhaSessao = null)
        {
            TokenJwt = tokenJwt;
            TokenExpiration = tokenExpiration;
            _loginSessao = loginSessao;
            _senhaSessao = senhaSessao;
            AtualizarCabecalhosAutenticacao();
            _ = RenovarTokenSeNecessarioAsync();
        }

        private bool TokenProximoDaExpiracao()
        {
            if (!TokenExpiration.HasValue)
            {
                return false;
            }

            var limiteSeguranca = TokenExpiration.Value.ToUniversalTime().AddMinutes(-TokenSafetyMarginMinutes);
            return DateTime.UtcNow >= limiteSeguranca;
        }

        private async Task RenovarTokenSeNecessarioAsync()
        {
            if (!TokenProximoDaExpiracao())
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_loginSessao) || string.IsNullOrWhiteSpace(_senhaSessao))
            {
                return;
            }

            await _tokenRefreshSemaphore.WaitAsync();
            try
            {
                if (!TokenProximoDaExpiracao())
                {
                    return;
                }

                var loginEscapado = Uri.EscapeDataString(_loginSessao);
                var senhaEscapada = Uri.EscapeDataString(_senhaSessao);
                var response = await HttpClientFixo.GetAsync($"Usuario/Login?login={loginEscapado}&senha={senhaEscapada}");

                if (!response.IsSuccessStatusCode)
                {
                    return;
                }

                var loginResponseJson = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginJwtResponse>(loginResponseJson);

                if (loginResponse == null || string.IsNullOrWhiteSpace(loginResponse.Token))
                {
                    return;
                }

                TokenJwt = loginResponse.Token;
                TokenExpiration = loginResponse.Expiration;
                AtualizarCabecalhosAutenticacao();
            }
            finally
            {
                _tokenRefreshSemaphore.Release();
            }
        }

        private static string NormalizarTextoComparacao(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return string.Empty;
            }

            var normalizado = valor.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var ch in normalizado)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString().ToLowerInvariant();
        }


        private static void RestaurarSelecaoPorId<T>(ComboBox comboBox, int? idSelecionado) where T : class
        {
            if (!idSelecionado.HasValue)
            {
                comboBox.SelectedItem = null;
                return;
            }

            var item = comboBox.Items.OfType<T>()
                .FirstOrDefault(x => ObterIdEntidade(x) == idSelecionado.Value);
            comboBox.SelectedItem = item;
        }

        public Sistema()
        {

            _viewModel = new SistemaViewModel(new SistemaListagemService(HttpClientFixo), new SistemaCrudService(HttpClientFixo));
            InitializeComponent();
            _viewModel.ShowErrorAction = mensagem => MessageBox.Show(mensagem, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            _viewModel.ShowInfoAction = mensagem => MessageBox.Show(mensagem, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            _viewModel.SalvarContratoCriarRequested = () => BtnSalvarContratoCriar_Click(this, new RoutedEventArgs());
            _viewModel.SalvarContratoVisualizarRequested = () => BtnSalvarContratoVisualizar_Click(this, new RoutedEventArgs());
            DataContext = _viewModel;
            WindowState = WindowState.Maximized;

            _tokenRefreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _tokenRefreshTimer.Tick += async (_, __) => await RenovarTokenSeNecessarioAsync();
            _tokenRefreshTimer.Start();
            Closed += (_, __) => _tokenRefreshTimer.Stop();

            FotosSelecionadasList.ItemsSource = _viewModel.FotosSelecionadasPreview;
            FotosSelecionadasListEditar.ItemsSource = _viewModel.FotosSelecionadasPreview;

            ComboModalidadeContratoCriar.SelectionChanged += ComboModalidadeContratoCriar_SelectionChanged;
            ComboModalidadeContratoVisualizar.SelectionChanged += ComboModalidadeContratoVisualizar_SelectionChanged;
            ComboContratoProprietarioCriar.SelectionChanged += ComboContratoProprietarioCriar_SelectionChanged;
            ComboContratoImovelCriar.SelectionChanged += ComboContratoImovelCriar_SelectionChanged;
            ComboContratoProprietarioVisualizar.SelectionChanged += ComboContratoProprietarioVisualizar_SelectionChanged;
            ComboContratoImovelVisualizar.SelectionChanged += ComboContratoImovelVisualizar_SelectionChanged;
            ComboContratoContratante1Criar.SelectionChanged += ComboContratanteCriar_SelectionChanged;
            ComboContratoContratante2Criar.SelectionChanged += ComboContratanteCriar_SelectionChanged;
            ComboContratoContratante3Criar.SelectionChanged += ComboContratanteCriar_SelectionChanged;
            ComboContratoContratante4Criar.SelectionChanged += ComboContratanteCriar_SelectionChanged;
            ComboContratoContratante1Visualizar.SelectionChanged += ComboContratanteVisualizar_SelectionChanged;
            ComboContratoContratante2Visualizar.SelectionChanged += ComboContratanteVisualizar_SelectionChanged;
            ComboContratoContratante3Visualizar.SelectionChanged += ComboContratanteVisualizar_SelectionChanged;
            ComboContratoContratante4Visualizar.SelectionChanged += ComboContratanteVisualizar_SelectionChanged;

            AtualizarPermissoesModalidadeCriar();
            AtualizarPermissoesModalidadeVisualizar();

            ImgPwrOff.MouseEnter += (s, e) =>
            {
                var circle = CircPwrOff;
                Mouse.OverrideCursor = Cursors.Hand;
                circle.Fill = new SolidColorBrush(Color.FromRgb(200, 200, 200));

            };

            ImgPwrOff.MouseLeave += (s, e) =>
            {
                var circle = CircPwrOff;

                Mouse.OverrideCursor = Cursors.Arrow;
                circle.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            };

            RetangLogo.MouseEnter += (s, e) => {
                var ret = RetangLogo;
                Mouse.OverrideCursor = Cursors.Hand;
            };

            RetangLogo.MouseLeave += (s, e) =>
            {
                var ret = RetangLogo;
                Mouse.OverrideCursor = Cursors.Arrow;
            };

            LogoNav.MouseEnter += (s, e) =>
            {
                var ret = RetangLogo;
                Mouse.OverrideCursor = Cursors.Hand;
            };

            LogoNav.MouseLeave += (s, e) =>
            {
                var ret = RetangLogo;
                Mouse.OverrideCursor = Cursors.Arrow;
            };

            RetangLogo.MouseDown += (s, e) =>
            {
                var ret = RetangLogo;

                var visibilidadeMenu = MenuNav.Visibility;
                if (visibilidadeMenu == Visibility.Visible)
                {
                    MenuNav.Visibility = Visibility.Collapsed;
                    ret.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));

                }
                else
                {
                    MenuNav.Visibility = Visibility.Visible;
                    ret.Fill = new SolidColorBrush(Color.FromRgb(200, 200, 200));
                }
            };

            ImgPwrOff.MouseDown += (s, e) =>
            {
                var result = MessageBox.Show("Tem certeza que deseja sair do sistema?", "Sair", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var loginWindow = new MainWindow();
                    loginWindow.Show();
                    this.Close();
                }
            };

            LogoNav.MouseDown += (s, e) =>
            {
                var ret = RetangLogo;
                
                var visibilidadeMenu = MenuNav.Visibility;
                if (visibilidadeMenu == Visibility.Visible)
                {
                    MenuNav.Visibility = Visibility.Collapsed;
                    ret.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));

                }
                else
                {
                    MenuNav.Visibility = Visibility.Visible;
                    ret.Fill = new SolidColorBrush(Color.FromRgb(200, 200, 200));
                }
            };
        }
    }
}

