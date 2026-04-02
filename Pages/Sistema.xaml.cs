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
using Imob.Services.Pdf;

namespace Imob
{
    public partial class Sistema : Window
    {
        private const int TokenSafetyMarginMinutes = 5;

        public static HttpClient HttpClientFixo { get; } = CriarHttpClient();

        public UsuarioDAO UsuarioLogado { get; set; }
        public string TokenJwt { get; private set; }
        public DateTime? TokenExpiration { get; private set; }

        private string _loginSessao;
        private string _senhaSessao;
        private readonly SemaphoreSlim _tokenRefreshSemaphore = new SemaphoreSlim(1, 1);
        private readonly DispatcherTimer _tokenRefreshTimer;

        private class LoginJwtResponse
        {
            [JsonProperty("token")]
            public string Token { get; set; }

            [JsonProperty("expiration")]
            public DateTime? Expiration { get; set; }

            [JsonProperty("tipo")]
            public string Tipo { get; set; }
        }

        private ObservableCollection<ImageSource> _fotosSelecionadasPreview = new ObservableCollection<ImageSource>();

        private List<byte[]> _fotosSelecionadasBinario = new List<byte[]>();

        private readonly Dictionary<ComboBox, string> _comboBoxSearchBuffer = new Dictionary<ComboBox, string>();

        private readonly Dictionary<ComboBox, DateTime> _comboBoxSearchBufferTimestamp = new Dictionary<ComboBox, DateTime>();

        private static readonly TimeSpan ComboBoxSearchBufferReset = TimeSpan.FromSeconds(1.2);

		private List<int?> _fotoIdsPreview = new List<int?>();

		private List<int> _fotosRemovidas = new List<int>();

        private int _idImovelCadastrado;

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
            client.BaseAddress = new Uri("https://localhost:7251/");
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

        // Funções auxilires Inicio

        public void FecharPanelsAtivos()
        {
            if (ProprietariosPanel.Visibility == Visibility.Visible) ProprietariosPanel.Visibility = Visibility.Hidden;
            
            if (LocatariosPanel.Visibility == Visibility.Visible) LocatariosPanel.Visibility = Visibility.Hidden;
            
            if (FiadoresPanel.Visibility == Visibility.Visible) FiadoresPanel.Visibility = Visibility.Hidden;
            
            if (ImoveisPanel.Visibility == Visibility.Visible) ImoveisPanel.Visibility = Visibility.Hidden;
            
            if (ContratosPanel.Visibility == Visibility.Visible) ContratosPanel.Visibility = Visibility.Hidden;
            
            if (VistoriasPanel.Visibility == Visibility.Visible) VistoriasPanel.Visibility = Visibility.Hidden;
        }

        public async Task AdicionarItensGridImoveis()
        {
            try
            {
                var listaImoveis = await ImovelDAO.GetImoveis(HttpClientFixo);
                ImoveisDataGrid.ItemsSource = listaImoveis;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar imóveis: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task AdcionarItensGridProprietarios()
        {
            try
            {
                var listaProprietarios = await ClienteDAO.GetProprietarios(HttpClientFixo);
                ProprietariosDataGrid.ItemsSource = listaProprietarios;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar proprietários: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task AdicionarItensGridLocatários()
        {
            try
            {
                var listaLocatarios = await ClienteDAO.GetLocatários(HttpClientFixo);
                LocatariosDataGrid.ItemsSource = listaLocatarios;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar locatários: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task AdicionarItensGridFiadores()
        {
            try
            {
                var listaFiadores = await ClienteDAO.GetFiadores(HttpClientFixo);
                FiadoresDataGrid.ItemsSource = listaFiadores;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar fiadores: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task AdicionarItensGridContratos()
        {
            try
            {
                var listaContratos = await ContratoDAO.GetContratos(HttpClientFixo);
                ContratosDataGrid.ItemsSource = listaContratos;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar contratos: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task AdicionarItensComboProprietarios()
        {
            List<ClienteDAO> listaClientes = await ClienteDAO.GetProprietarios(HttpClientFixo);

            foreach (ClienteDAO cliente in listaClientes)
            {
                ComboProprietarios.Items.Add(cliente.Nome.ToString());
            }
        }

        public void AdicionarItensComboIntencoes()
        {
            List<IntencaoDAO> ListaIntencoes = IntencaoDAO.GetIntencao(HttpClientFixo);

            foreach (IntencaoDAO intencao in ListaIntencoes)
            {
                ComboIntencao.Items.Add(intencao.Nome.ToString());
            }
        }

        public void AdicionarItensComboTiposImovel()
        {
            List<TipoImovelDAO> ListaTiposImovel = TipoImovelDAO.GetTipoImovel(HttpClientFixo);
            foreach (TipoImovelDAO tipoImovel in ListaTiposImovel)
            {
                ComboTipoImovel.Items.Add(tipoImovel.Nome.ToString());
            }
        }

        private void ComboBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is not ComboBox comboBox || string.IsNullOrWhiteSpace(e.Text))
            {
                return;
            }

            AtualizarBuscaComboBox(comboBox, e.Text);
            e.Handled = true;
        }

        private void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not ComboBox comboBox)
            {
                return;
            }

            if (e.Key == System.Windows.Input.Key.Back)
            {
                if (!_comboBoxSearchBuffer.TryGetValue(comboBox, out var termoAtual) || string.IsNullOrEmpty(termoAtual))
                {
                    return;
                }

                termoAtual = termoAtual[..^1];
                DefinirBuscaComboBox(comboBox, termoAtual);
                e.Handled = true;
                return;
            }

            if (e.Key == System.Windows.Input.Key.Space)
            {
                AtualizarBuscaComboBox(comboBox, " ");
                e.Handled = true;
                return;
            }

            if (e.Key == System.Windows.Input.Key.Escape)
            {
                LimparBuscaComboBox(comboBox);
                RestaurarFiltroComboBox(comboBox);
                comboBox.Text = string.Empty;
                comboBox.SelectedItem = null;
            }
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not ComboBox comboBox)
            {
                return;
            }

            comboBox.DropDownClosed -= ComboBox_DropDownClosed;
            comboBox.DropDownClosed += ComboBox_DropDownClosed;
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (sender is not ComboBox comboBox)
            {
                return;
            }

            RestaurarFiltroComboBox(comboBox);

            if (comboBox.SelectedItem != null)
            {
                comboBox.Text = ObterTextoItemComboBox(comboBox, comboBox.SelectedItem);
            }

            LimparBuscaComboBox(comboBox);
        }

        private void AtualizarBuscaComboBox(ComboBox comboBox, string termoDigitado)
        {
            _comboBoxSearchBuffer.TryGetValue(comboBox, out var termoAtual);
            _comboBoxSearchBufferTimestamp.TryGetValue(comboBox, out var ultimaDigitacao);

            if (DateTime.UtcNow - ultimaDigitacao > ComboBoxSearchBufferReset)
            {
                termoAtual = string.Empty;
            }

            DefinirBuscaComboBox(comboBox, (termoAtual ?? string.Empty) + termoDigitado);
        }

        private void DefinirBuscaComboBox(ComboBox comboBox, string termo)
        {
            _comboBoxSearchBuffer[comboBox] = termo;
            _comboBoxSearchBufferTimestamp[comboBox] = DateTime.UtcNow;

            if (comboBox.IsEditable)
            {
                comboBox.Text = termo ?? string.Empty;
                if (comboBox.Template.FindName("PART_EditableTextBox", comboBox) is TextBox editavel)
                {
                    editavel.SelectionStart = editavel.Text.Length;
                    editavel.SelectionLength = 0;
                }
            }

            if (string.IsNullOrWhiteSpace(termo))
            {
                comboBox.SelectedItem = null;
                RestaurarFiltroComboBox(comboBox);
                comboBox.IsDropDownOpen = true;
                return;
            }

            var termoNormalizado = termo.Trim();
            comboBox.SelectedItem = null;
            AplicarFiltroComboBox(comboBox, termoNormalizado);
        }

        private void AplicarFiltroComboBox(ComboBox comboBox, string termo)
        {
            comboBox.Items.Filter = item =>
            {
                if (item == null)
                {
                    return false;
                }

                var textoItem = ObterTextoItemComboBox(comboBox, item);
                return !string.IsNullOrWhiteSpace(textoItem) &&
                       textoItem.IndexOf(termo, StringComparison.CurrentCultureIgnoreCase) >= 0;
            };

            comboBox.Items.Refresh();
            comboBox.IsDropDownOpen = true;

            comboBox.Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (var item in comboBox.Items)
                {
                    if (comboBox.ItemContainerGenerator.ContainerFromItem(item) is FrameworkElement elemento)
                    {
                        elemento.BringIntoView();
                        break;
                    }
                }

                if (comboBox.Template.FindName("PART_EditableTextBox", comboBox) is TextBox editavel)
                {
                    editavel.Focus();
                    editavel.SelectionStart = editavel.Text.Length;
                    editavel.SelectionLength = 0;
                }
            }), DispatcherPriority.Background);
        }

        private static string ObterTextoItemComboBox(ComboBox comboBox, object item)
        {
            if (item is ComboBoxItem comboBoxItem)
            {
                return comboBoxItem.Content?.ToString() ?? string.Empty;
            }

            var caminhoTexto = TextSearch.GetTextPath(comboBox);
            if (string.IsNullOrWhiteSpace(caminhoTexto))
            {
                caminhoTexto = comboBox.DisplayMemberPath;
            }

            if (!string.IsNullOrWhiteSpace(caminhoTexto))
            {
                var propriedade = item.GetType().GetProperty(caminhoTexto, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (propriedade != null)
                {
                    return propriedade.GetValue(item)?.ToString() ?? string.Empty;
                }
            }

            return item.ToString() ?? string.Empty;
        }

        private void LimparBuscaComboBox(ComboBox comboBox)
        {
            _comboBoxSearchBuffer.Remove(comboBox);
            _comboBoxSearchBufferTimestamp.Remove(comboBox);
        }

        private static void RestaurarFiltroComboBox(ComboBox comboBox)
        {
            comboBox.Items.Filter = null;
            comboBox.Items.Refresh();
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

        private static bool ModalidadeEhFiador(ModalidadeContratoDAO modalidade)
        {
            var nome = NormalizarTextoComparacao(modalidade?.Nome);
            return nome.Contains("fiador");
        }

        private static bool ModalidadeEhSeguroFianca(ModalidadeContratoDAO modalidade)
        {
            var nome = NormalizarTextoComparacao(modalidade?.Nome);
            return nome.Contains("seguro fianca") || nome.Contains("segurofianca");
        }

        private static int? ObterIdEntidade(object entidade)
        {
            if (entidade == null)
            {
                return null;
            }

            var propriedadeId = entidade.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (propriedadeId == null)
            {
                return null;
            }

            var valor = propriedadeId.GetValue(entidade);
            return valor switch
            {
                int idInt => idInt,
                long idLong => (int)idLong,
                _ => null
            };
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

        private void AtualizarPermissoesModalidadeCriar()
        {
            var modalidade = ComboModalidadeContratoCriar.SelectedItem as ModalidadeContratoDAO;
            var habilitaFiador = ModalidadeEhFiador(modalidade);
            var habilitaSeguro = ModalidadeEhSeguroFianca(modalidade);

            ComboContratoFiadorCriar.IsEnabled = habilitaFiador;
            if (!habilitaFiador)
            {
                ComboContratoFiadorCriar.SelectedItem = null;
                ComboContratoFiadorCriar.Text = string.Empty;
            }

            TxtContratoPropostaSegFiancaCriar.IsReadOnly = !habilitaSeguro;
            TxtContratoApoliceSegFiancaCriar.IsReadOnly = !habilitaSeguro;

            if (!habilitaSeguro)
            {
                TxtContratoPropostaSegFiancaCriar.Background = new SolidColorBrush(Color.FromRgb(243, 243, 243));
                TxtContratoPropostaSegFiancaCriar.Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136));
                TxtContratoPropostaSegFiancaCriar.BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                TxtContratoPropostaSegFiancaCriar.ToolTip = "Disponível apenas para modalidade Seguro fiança.";

                TxtContratoApoliceSegFiancaCriar.Background = new SolidColorBrush(Color.FromRgb(243, 243, 243));
                TxtContratoApoliceSegFiancaCriar.Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136));
                TxtContratoApoliceSegFiancaCriar.BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                TxtContratoApoliceSegFiancaCriar.ToolTip = "Disponível apenas para modalidade Seguro fiança.";
            }
            else
            {
                TxtContratoPropostaSegFiancaCriar.ClearValue(TextBox.BackgroundProperty);
                TxtContratoPropostaSegFiancaCriar.ClearValue(TextBox.ForegroundProperty);
                TxtContratoPropostaSegFiancaCriar.ClearValue(TextBox.BorderBrushProperty);
                TxtContratoPropostaSegFiancaCriar.ToolTip = null;

                TxtContratoApoliceSegFiancaCriar.ClearValue(TextBox.BackgroundProperty);
                TxtContratoApoliceSegFiancaCriar.ClearValue(TextBox.ForegroundProperty);
                TxtContratoApoliceSegFiancaCriar.ClearValue(TextBox.BorderBrushProperty);
                TxtContratoApoliceSegFiancaCriar.ToolTip = null;
            }

            if (!habilitaSeguro)
            {
                TxtContratoPropostaSegFiancaCriar.Clear();
                TxtContratoApoliceSegFiancaCriar.Clear();
            }
        }

        private void AtualizarPermissoesModalidadeVisualizar()
        {
            var modalidade = ComboModalidadeContratoVisualizar.SelectedItem as ModalidadeContratoDAO;
            var habilitaFiador = ModalidadeEhFiador(modalidade);
            var habilitaSeguro = ModalidadeEhSeguroFianca(modalidade);

            ComboContratoFiadorVisualizar.IsEnabled = habilitaFiador;
            if (!habilitaFiador)
            {
                ComboContratoFiadorVisualizar.SelectedItem = null;
                ComboContratoFiadorVisualizar.Text = string.Empty;
            }

            TxtContratoPropostaSegFiancaVisualizar.IsReadOnly = !habilitaSeguro;
            TxtContratoApoliceSegFiancaVisualizar.IsReadOnly = !habilitaSeguro;

            if (!habilitaSeguro)
            {
                TxtContratoPropostaSegFiancaVisualizar.Background = new SolidColorBrush(Color.FromRgb(243, 243, 243));
                TxtContratoPropostaSegFiancaVisualizar.Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136));
                TxtContratoPropostaSegFiancaVisualizar.BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                TxtContratoPropostaSegFiancaVisualizar.ToolTip = "Disponível apenas para modalidade Seguro fiança.";

                TxtContratoApoliceSegFiancaVisualizar.Background = new SolidColorBrush(Color.FromRgb(243, 243, 243));
                TxtContratoApoliceSegFiancaVisualizar.Foreground = new SolidColorBrush(Color.FromRgb(136, 136, 136));
                TxtContratoApoliceSegFiancaVisualizar.BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224));
                TxtContratoApoliceSegFiancaVisualizar.ToolTip = "Disponível apenas para modalidade Seguro fiança.";
            }
            else
            {
                TxtContratoPropostaSegFiancaVisualizar.ClearValue(TextBox.BackgroundProperty);
                TxtContratoPropostaSegFiancaVisualizar.ClearValue(TextBox.ForegroundProperty);
                TxtContratoPropostaSegFiancaVisualizar.ClearValue(TextBox.BorderBrushProperty);
                TxtContratoPropostaSegFiancaVisualizar.ToolTip = null;

                TxtContratoApoliceSegFiancaVisualizar.ClearValue(TextBox.BackgroundProperty);
                TxtContratoApoliceSegFiancaVisualizar.ClearValue(TextBox.ForegroundProperty);
                TxtContratoApoliceSegFiancaVisualizar.ClearValue(TextBox.BorderBrushProperty);
                TxtContratoApoliceSegFiancaVisualizar.ToolTip = null;
            }

            if (!habilitaSeguro)
            {
                TxtContratoPropostaSegFiancaVisualizar.Clear();
                TxtContratoApoliceSegFiancaVisualizar.Clear();
            }
        }

        private void AtualizarFiltroProprietarioImovelCriar(bool proprietarioOrigem)
        {
            if (_atualizandoCombosContratoCriar)
            {
                return;
            }

            _atualizandoCombosContratoCriar = true;
            try
            {
                var proprietarioSelecionado = ComboContratoProprietarioCriar.SelectedItem as ClienteDAO;
                var imovelSelecionado = ComboContratoImovelCriar.SelectedItem as ImovelDAO;

                if (proprietarioOrigem)
                {
                    var idProprietario = proprietarioSelecionado?.Id;
                    var imoveisFiltrados = _imoveisContratoCriar
                        .Where(i => !idProprietario.HasValue || i.Proprietario?.Id == idProprietario.Value)
                        .ToList();

                    var imovelIdSelecionado = imovelSelecionado?.Id;

                    ComboContratoProprietarioCriar.ItemsSource = _proprietariosContratoCriar;
                    ComboContratoImovelCriar.ItemsSource = imoveisFiltrados;

                    RestaurarSelecaoPorId<ClienteDAO>(ComboContratoProprietarioCriar, proprietarioSelecionado?.Id);
                    RestaurarSelecaoPorId<ImovelDAO>(ComboContratoImovelCriar, imovelIdSelecionado);

                    if (ComboContratoImovelCriar.SelectedItem == null)
                    {
                        ComboContratoImovelCriar.SelectedItem = null;
                    }
                }
                else
                {
                    var proprietariosFiltrados = _proprietariosContratoCriar
                        .Where(p => imovelSelecionado == null || p.Id == imovelSelecionado.Proprietario?.Id)
                        .ToList();

                    ComboContratoImovelCriar.ItemsSource = _imoveisContratoCriar;
                    ComboContratoProprietarioCriar.ItemsSource = proprietariosFiltrados;

                    RestaurarSelecaoPorId<ImovelDAO>(ComboContratoImovelCriar, imovelSelecionado?.Id);

                    var proprietarioId = imovelSelecionado?.Proprietario?.Id ?? proprietarioSelecionado?.Id;
                    RestaurarSelecaoPorId<ClienteDAO>(ComboContratoProprietarioCriar, proprietarioId);
                }
            }
            finally
            {
                _atualizandoCombosContratoCriar = false;
            }
        }

        private void AtualizarFiltroProprietarioImovelVisualizar(bool proprietarioOrigem)
        {
            if (_atualizandoCombosContratoVisualizar)
            {
                return;
            }

            _atualizandoCombosContratoVisualizar = true;
            try
            {
                var proprietarioSelecionado = ComboContratoProprietarioVisualizar.SelectedItem as ClienteDAO;
                var imovelSelecionado = ComboContratoImovelVisualizar.SelectedItem as ImovelDAO;

                if (proprietarioOrigem)
                {
                    var idProprietario = proprietarioSelecionado?.Id;
                    var imoveisFiltrados = _imoveisContratoVisualizar
                        .Where(i => !idProprietario.HasValue || i.Proprietario?.Id == idProprietario.Value)
                        .ToList();

                    var imovelIdSelecionado = imovelSelecionado?.Id;

                    ComboContratoProprietarioVisualizar.ItemsSource = _proprietariosContratoVisualizar;
                    ComboContratoImovelVisualizar.ItemsSource = imoveisFiltrados;

                    RestaurarSelecaoPorId<ClienteDAO>(ComboContratoProprietarioVisualizar, proprietarioSelecionado?.Id);
                    RestaurarSelecaoPorId<ImovelDAO>(ComboContratoImovelVisualizar, imovelIdSelecionado);
                }
                else
                {
                    var proprietariosFiltrados = _proprietariosContratoVisualizar
                        .Where(p => imovelSelecionado == null || p.Id == imovelSelecionado.Proprietario?.Id)
                        .ToList();

                    ComboContratoImovelVisualizar.ItemsSource = _imoveisContratoVisualizar;
                    ComboContratoProprietarioVisualizar.ItemsSource = proprietariosFiltrados;

                    RestaurarSelecaoPorId<ImovelDAO>(ComboContratoImovelVisualizar, imovelSelecionado?.Id);

                    var proprietarioId = imovelSelecionado?.Proprietario?.Id ?? proprietarioSelecionado?.Id;
                    RestaurarSelecaoPorId<ClienteDAO>(ComboContratoProprietarioVisualizar, proprietarioId);
                }
            }
            finally
            {
                _atualizandoCombosContratoVisualizar = false;
            }
        }

        private void AtualizarFiltroContratantesCriar()
        {
            if (_atualizandoCombosContratoCriar)
            {
                return;
            }

            _atualizandoCombosContratoCriar = true;
            try
            {
                AtualizarItensContratante(
                    _locatariosContratoCriar,
                    ComboContratoContratante1Criar,
                    ComboContratoContratante2Criar,
                    ComboContratoContratante3Criar,
                    ComboContratoContratante4Criar);
            }
            finally
            {
                _atualizandoCombosContratoCriar = false;
            }
        }

        private void AtualizarFiltroContratantesVisualizar()
        {
            if (_atualizandoCombosContratoVisualizar)
            {
                return;
            }

            _atualizandoCombosContratoVisualizar = true;
            try
            {
                AtualizarItensContratante(
                    _locatariosContratoVisualizar,
                    ComboContratoContratante1Visualizar,
                    ComboContratoContratante2Visualizar,
                    ComboContratoContratante3Visualizar,
                    ComboContratoContratante4Visualizar);
            }
            finally
            {
                _atualizandoCombosContratoVisualizar = false;
            }
        }

        private static void AtualizarItensContratante(
            List<ClienteDAO> baseLocatarios,
            ComboBox combo1,
            ComboBox combo2,
            ComboBox combo3,
            ComboBox combo4)
        {
            var combos = new[] { combo1, combo2, combo3, combo4 };
            var selecionados = combos
                .Select(c => c.SelectedItem as ClienteDAO)
                .ToArray();

            for (var i = 0; i < combos.Length; i++)
            {
                var idAtual = selecionados[i]?.Id;
                var idsOutros = selecionados
                    .Where((s, indice) => indice != i && s != null)
                    .Select(s => s.Id)
                    .ToHashSet();

                var itens = baseLocatarios
                    .Where(l => !idsOutros.Contains(l.Id) || (idAtual.HasValue && l.Id == idAtual.Value))
                    .ToList();

                combos[i].ItemsSource = itens;
                RestaurarSelecaoPorId<ClienteDAO>(combos[i], idAtual);
            }
        }

        private bool PossuiContratantesRepetidos(params ClienteDAO[] contratantes)
        {
            var ids = contratantes
                .Where(c => c != null)
                .Select(c => c.Id)
                .ToList();

            return ids.Count != ids.Distinct().Count();
        }

        private void ComboModalidadeContratoCriar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AtualizarPermissoesModalidadeCriar();
        }

        private void ComboModalidadeContratoVisualizar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AtualizarPermissoesModalidadeVisualizar();
        }

        private void ComboContratoProprietarioCriar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AtualizarFiltroProprietarioImovelCriar(true);
        }

        private void ComboContratoImovelCriar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AtualizarFiltroProprietarioImovelCriar(false);
        }

        private void ComboContratoProprietarioVisualizar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AtualizarFiltroProprietarioImovelVisualizar(true);
        }

        private void ComboContratoImovelVisualizar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AtualizarFiltroProprietarioImovelVisualizar(false);
        }

        private void ComboContratanteCriar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AtualizarFiltroContratantesCriar();
        }

        private void ComboContratanteVisualizar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AtualizarFiltroContratantesVisualizar();
        }

        // Funções auxilires Fim

        public Sistema()
        {

            InitializeComponent();
            WindowState = WindowState.Maximized;

            _tokenRefreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _tokenRefreshTimer.Tick += async (_, __) => await RenovarTokenSeNecessarioAsync();
            _tokenRefreshTimer.Start();
            Closed += (_, __) => _tokenRefreshTimer.Stop();

            FotosSelecionadasList.ItemsSource = _fotosSelecionadasPreview;
			FotosSelecionadasListEditar.ItemsSource = _fotosSelecionadasPreview;

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

            // Mouse Enter and Leave Events Fim

            //Click Events Inicio

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

        private async void ImoveisTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();

            await AdicionarItensGridImoveis();
            ImoveisPanel.Visibility = Visibility.Visible;
        }

        private async void ContratosTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();

            await AdicionarItensGridContratos();
            ContratosPanel.Visibility = Visibility.Visible;
        }

        private async void ProprietariosTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();

            await AdcionarItensGridProprietarios();

            ProprietariosPanel.Visibility = Visibility.Visible;
        }

        private async void LocatariosTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();

            await AdicionarItensGridLocatários();

            LocatariosPanel.Visibility = Visibility.Visible;
        }

        private async void FiadoresTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();

            await AdicionarItensGridFiadores();

            FiadoresPanel.Visibility = Visibility.Visible;
        }

        private void VistoriaTreeListar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            VistoriasPanel.Visibility = Visibility.Visible;
        }

        private void BtnAdicionarImovel_Click(object sender, RoutedEventArgs e)
        {
            Task task = AdicionarItensComboProprietarios();
            AdicionarItensComboIntencoes();
            AdicionarItensComboTiposImovel();
            ImovelModalOverlayCriar.Visibility = Visibility.Visible;
        }

        private void BtnFecharModalImovel_Click(object sender, RoutedEventArgs e)
        {
            ImovelModalOverlayCriar.Visibility = Visibility.Hidden;
            ComboProprietarios.Items.Clear();
            ComboIntencao.Items.Clear();
            ComboTipoImovel.Items.Clear();
            TxtBoxCep.Clear();
            TxtBoxLogradouro.Clear();
            TxtBoxNumero.Clear();
            TxtBoxPais.Clear();
            TxtBoxEstado.Clear();
            TxtBoxCidade.Clear();
            TxtBoxBairro.Clear();
            TxtBoxComplemento.Clear();
            TxtBoxCondominio.Clear();
            TxtBoxObservacoes.Clear();
            TxtBoxDescricao.Clear();
            TxtBoxMetragem.Clear();
            TxtBoxValor.Clear();
            TxtBoxIptu.Clear();
            TxtBoxTaxaIncendio.Clear();
            TxtBoxForo.Clear();
        }

        private async void CadastrarImovelBanco(object sender, RoutedEventArgs e)
        {
            var clienteSelecionado = ComboProprietarios.SelectedItem as string;
            var intencaoSelecionada = ComboIntencao.SelectedItem as string;
            var tipoImovelSelecionado = ComboTipoImovel.SelectedItem as string;

            if (string.IsNullOrWhiteSpace(clienteSelecionado) ||
                string.IsNullOrWhiteSpace(intencaoSelecionada) ||
                string.IsNullOrWhiteSpace(tipoImovelSelecionado) ||
                string.IsNullOrWhiteSpace(TxtBoxCep.Text) ||
                string.IsNullOrWhiteSpace(TxtBoxLogradouro.Text) ||
                string.IsNullOrWhiteSpace(TxtBoxNumero.Text) ||
                string.IsNullOrWhiteSpace(TxtBoxPais.Text) ||
                string.IsNullOrWhiteSpace(TxtBoxEstado.Text) ||
                string.IsNullOrWhiteSpace(TxtBoxCidade.Text) ||
                string.IsNullOrWhiteSpace(TxtBoxBairro.Text))
            {
                MessageBox.Show("Por favor, preencha todos os campos obrigatórios. (*)", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(TxtBoxMetragem.Text, out int metragem))
            {
                MessageBox.Show("Metragem inválida.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!double.TryParse(TxtBoxValor.Text, out double valor)) {
                MessageBox.Show("Valor inválido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!double.TryParse(TxtBoxIptu.Text, out double iptu)) {
                MessageBox.Show("IPTU inválido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!double.TryParse(TxtBoxTaxaIncendio.Text, out double taxaIncendio))
            {
                MessageBox.Show("Taxa de Incêndio inválida.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!double.TryParse(TxtBoxForo.Text, out double foro)) {
                MessageBox.Show("Foro inválido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(TxtBoxNumero.Text, out int numero))
            {
                MessageBox.Show("Número inválido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var complemento = TxtBoxComplemento.Text;
            var condominio = TxtBoxCondominio.Text;
            var observacao = TxtBoxObservacoes.Text;
            var descricao = TxtBoxDescricao.Text;
            var inscricaoIptu = TxtBoxInscricaoIptu.Text;
            var numeroCbmerj = TxtBoxNumeroCbmerj.Text;

            ImovelDTO imovel = new ImovelDTO();

            if (observacao != null)
            {
                imovel.Observacao = observacao;
            }

            if (descricao != null)
            {
                imovel.Descricao = descricao;
            }

            if (complemento != null)
            {
                imovel.Complemento = complemento;
            }

            imovel.Proprietario = ClienteDAO.GetIdPorNome(clienteSelecionado, HttpClientFixo);
            imovel.TipoImovel = TipoImovelDAO.GetIdPorNome(tipoImovelSelecionado, HttpClientFixo);
            imovel.Intencao = IntencaoDAO.GetIdPorNome(intencaoSelecionada, HttpClientFixo);
            imovel.Cep = TxtBoxCep.Text;
            imovel.Logradouro = TxtBoxLogradouro.Text;
            imovel.Numero = int.Parse(TxtBoxNumero.Text);
            imovel.Bairro = TxtBoxBairro.Text;
            imovel.Cidade = TxtBoxCidade.Text;
            imovel.Estado = TxtBoxEstado.Text;
            imovel.Pais = TxtBoxPais.Text;
            imovel.Metragem = metragem;
            imovel.Valor = (decimal)valor;
            imovel.Condominio = string.IsNullOrWhiteSpace(condominio) ? null : (decimal?)Convert.ToDecimal(condominio);
            imovel.Iptu = (decimal?)iptu;
            imovel.TaxaIncendio = (decimal?)taxaIncendio;
            imovel.Foro = (decimal?)foro;
            imovel.InscricaoIptu = inscricaoIptu;
            imovel.NumeroCbmerj = numeroCbmerj;
            imovel.Cadastrador = UsuarioLogado.Id;



            int idImovel = await imovel.CadastrarImovel(HttpClientFixo);

            _idImovelCadastrado = idImovel;

            MessageBox.Show("Imóvel cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            ComboProprietarios.Items.Clear();
            ComboIntencao.Items.Clear();
            ComboTipoImovel.Items.Clear();
            TxtBoxCep.Clear();
            TxtBoxLogradouro.Clear();
            TxtBoxNumero.Clear();
            TxtBoxPais.Clear();
            TxtBoxEstado.Clear();
            TxtBoxCidade.Clear();
            TxtBoxBairro.Clear();
            TxtBoxComplemento.Clear();
            TxtBoxCondominio.Clear();
            TxtBoxObservacoes.Clear();
            TxtBoxDescricao.Clear();
            TxtBoxMetragem.Clear();
            TxtBoxValor.Clear();
            TxtBoxIptu.Clear();
            TxtBoxTaxaIncendio.Clear();
            TxtBoxForo.Clear();

            ImovelModalOverlayCriar.Visibility = Visibility.Hidden;
            await AdicionarItensGridImoveis();

            ImovelFotosModalOverlayCriar.Visibility = Visibility.Visible;

        }

        private async void BtnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            await AdicionarItensGridImoveis();  
        }

        private async void BtnAtualizarProprietarios_Click(object sender, RoutedEventArgs e)
        {
            await AdcionarItensGridProprietarios();
        }

        private async void BtnAtualizarLocatarios_Click(object sender, RoutedEventArgs e)
        {
            await AdicionarItensGridLocatários();
        }

        private async void BtnAtualizarFiadores_Click(object sender, RoutedEventArgs e)
        {
            await AdicionarItensGridFiadores();
        }

        private async void BtnAtualizarContratos_Click(object sender, RoutedEventArgs e)
        {
            await AdicionarItensGridContratos();
        }

        private async void BtnAdicionarContrato_Click(object sender, RoutedEventArgs e)
        {
            await CarregarCombosContratoCriarAsync();
            ContratoModalOverlayCriar.Visibility = Visibility.Visible;
        }

        private async Task CarregarCombosContratoCriarAsync()
        {
            try
            {
                var tiposContratoTask = TipoContratoDAO.GetTiposContrato(HttpClientFixo);
                var modalidadesContratoTask = ModalidadeContratoDAO.GetModalidadesContrato(HttpClientFixo);
                var objetosContratoTask = ObjetoContratoDAO.GetObjetosContrato(HttpClientFixo);
                var proprietariosTask = ClienteDAO.GetProprietarios(HttpClientFixo);
                var locatariosTask = ClienteDAO.GetLocatários(HttpClientFixo);
                var fiadoresTask = ClienteDAO.GetFiadores(HttpClientFixo);
                var imoveisTask = ImovelDAO.GetImoveis(HttpClientFixo);

                await Task.WhenAll(
                    tiposContratoTask,
                    modalidadesContratoTask,
                    objetosContratoTask,
                    proprietariosTask,
                    locatariosTask,
                    fiadoresTask,
                    imoveisTask);

                ComboTipoContratoCriar.DisplayMemberPath = "Nome";
                ComboTipoContratoCriar.SelectedValuePath = "Id";
                ComboTipoContratoCriar.ItemsSource = tiposContratoTask.Result;

                ComboModalidadeContratoCriar.DisplayMemberPath = "Nome";
                ComboModalidadeContratoCriar.SelectedValuePath = "Id";
                ComboModalidadeContratoCriar.ItemsSource = modalidadesContratoTask.Result;

                ComboObjetoContratoCriar.DisplayMemberPath = "Nome";
                ComboObjetoContratoCriar.SelectedValuePath = "Id";
                ComboObjetoContratoCriar.ItemsSource = objetosContratoTask.Result;

                _proprietariosContratoCriar = proprietariosTask.Result?.ToList() ?? new List<ClienteDAO>();
                _imoveisContratoCriar = imoveisTask.Result?.ToList() ?? new List<ImovelDAO>();
                _locatariosContratoCriar = locatariosTask.Result?.ToList() ?? new List<ClienteDAO>();

                ComboContratoProprietarioCriar.DisplayMemberPath = "Nome";
                ComboContratoProprietarioCriar.SelectedValuePath = "Id";
                ComboContratoProprietarioCriar.ItemsSource = _proprietariosContratoCriar;

                ComboContratoImovelCriar.DisplayMemberPath = "Logradouro";
                ComboContratoImovelCriar.SelectedValuePath = "Id";
                ComboContratoImovelCriar.ItemsSource = _imoveisContratoCriar;

                ComboContratoContratante1Criar.DisplayMemberPath = "Nome";
                ComboContratoContratante1Criar.SelectedValuePath = "Id";
                ComboContratoContratante1Criar.ItemsSource = _locatariosContratoCriar;

                ComboContratoContratante2Criar.DisplayMemberPath = "Nome";
                ComboContratoContratante2Criar.SelectedValuePath = "Id";
                ComboContratoContratante2Criar.ItemsSource = _locatariosContratoCriar;

                ComboContratoContratante3Criar.DisplayMemberPath = "Nome";
                ComboContratoContratante3Criar.SelectedValuePath = "Id";
                ComboContratoContratante3Criar.ItemsSource = _locatariosContratoCriar;

                ComboContratoContratante4Criar.DisplayMemberPath = "Nome";
                ComboContratoContratante4Criar.SelectedValuePath = "Id";
                ComboContratoContratante4Criar.ItemsSource = _locatariosContratoCriar;

                ComboContratoFiadorCriar.DisplayMemberPath = "Nome";
                ComboContratoFiadorCriar.SelectedValuePath = "Id";
                ComboContratoFiadorCriar.ItemsSource = fiadoresTask.Result;

                AtualizarPermissoesModalidadeCriar();
                AtualizarFiltroProprietarioImovelCriar(true);
                AtualizarFiltroContratantesCriar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados do contrato: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task CarregarCombosContratoEditarAsync()
        {
            try
            {
                var tiposContratoTask = TipoContratoDAO.GetTiposContrato(HttpClientFixo);
                var modalidadesContratoTask = ModalidadeContratoDAO.GetModalidadesContrato(HttpClientFixo);
                var objetosContratoTask = ObjetoContratoDAO.GetObjetosContrato(HttpClientFixo);
                var proprietariosTask = ClienteDAO.GetProprietarios(HttpClientFixo);
                var locatariosTask = ClienteDAO.GetLocatários(HttpClientFixo);
                var fiadoresTask = ClienteDAO.GetFiadores(HttpClientFixo);
                var imoveisTask = ImovelDAO.GetImoveis(HttpClientFixo);

                await Task.WhenAll(
                    tiposContratoTask,
                    modalidadesContratoTask,
                    objetosContratoTask,
                    proprietariosTask,
                    locatariosTask,
                    fiadoresTask,
                    imoveisTask);

                ComboTipoContratoVisualizar.DisplayMemberPath = "Nome";
                ComboTipoContratoVisualizar.SelectedValuePath = "Id";
                ComboTipoContratoVisualizar.ItemsSource = tiposContratoTask.Result;

                ComboModalidadeContratoVisualizar.DisplayMemberPath = "Nome";
                ComboModalidadeContratoVisualizar.SelectedValuePath = "Id";
                ComboModalidadeContratoVisualizar.ItemsSource = modalidadesContratoTask.Result;

                ComboObjetoContratoVisualizar.DisplayMemberPath = "Nome";
                ComboObjetoContratoVisualizar.SelectedValuePath = "Id";
                ComboObjetoContratoVisualizar.ItemsSource = objetosContratoTask.Result;

                _proprietariosContratoVisualizar = proprietariosTask.Result?.ToList() ?? new List<ClienteDAO>();
                _imoveisContratoVisualizar = imoveisTask.Result?.ToList() ?? new List<ImovelDAO>();
                _locatariosContratoVisualizar = locatariosTask.Result?.ToList() ?? new List<ClienteDAO>();

                ComboContratoProprietarioVisualizar.DisplayMemberPath = "Nome";
                ComboContratoProprietarioVisualizar.SelectedValuePath = "Id";
                ComboContratoProprietarioVisualizar.ItemsSource = _proprietariosContratoVisualizar;

                ComboContratoImovelVisualizar.DisplayMemberPath = "Logradouro";
                ComboContratoImovelVisualizar.SelectedValuePath = "Id";
                ComboContratoImovelVisualizar.ItemsSource = _imoveisContratoVisualizar;

                ComboContratoContratante1Visualizar.DisplayMemberPath = "Nome";
                ComboContratoContratante1Visualizar.SelectedValuePath = "Id";
                ComboContratoContratante1Visualizar.ItemsSource = _locatariosContratoVisualizar;

                ComboContratoContratante2Visualizar.DisplayMemberPath = "Nome";
                ComboContratoContratante2Visualizar.SelectedValuePath = "Id";
                ComboContratoContratante2Visualizar.ItemsSource = _locatariosContratoVisualizar;

                ComboContratoContratante3Visualizar.DisplayMemberPath = "Nome";
                ComboContratoContratante3Visualizar.SelectedValuePath = "Id";
                ComboContratoContratante3Visualizar.ItemsSource = _locatariosContratoVisualizar;

                ComboContratoContratante4Visualizar.DisplayMemberPath = "Nome";
                ComboContratoContratante4Visualizar.SelectedValuePath = "Id";
                ComboContratoContratante4Visualizar.ItemsSource = _locatariosContratoVisualizar;

                ComboContratoFiadorVisualizar.DisplayMemberPath = "Nome";
                ComboContratoFiadorVisualizar.SelectedValuePath = "Id";
                ComboContratoFiadorVisualizar.ItemsSource = fiadoresTask.Result;

                AtualizarPermissoesModalidadeVisualizar();
                AtualizarFiltroProprietarioImovelVisualizar(true);
                AtualizarFiltroContratantesVisualizar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados do contrato: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnFecharModalContratosCriar_Click(object sender, RoutedEventArgs e)
        {
            ContratoModalOverlayCriar.Visibility = Visibility.Hidden;

            TxtContratoNomeCriar.Clear();
            TxtContratoPrazoMesesCriar.Clear();
            TxtContratoPropostaSegFiancaCriar.Clear();
            TxtContratoApoliceSegFiancaCriar.Clear();

            DpContratoDataInicioCriar.SelectedDate = null;
            DpContratoVencimentoCriar.Text = string.Empty;

            ComboTipoContratoCriar.SelectedItem = null;
            ComboModalidadeContratoCriar.SelectedItem = null;
            ComboObjetoContratoCriar.SelectedItem = null;
            ComboContratoProprietarioCriar.SelectedItem = null;
            ComboContratoImovelCriar.SelectedItem = null;
            ComboContratoContratante1Criar.SelectedItem = null;
            ComboContratoContratante2Criar.SelectedItem = null;
            ComboContratoContratante3Criar.SelectedItem = null;
            ComboContratoContratante4Criar.SelectedItem = null;
            ComboContratoFiadorCriar.SelectedItem = null;

            AtualizarPermissoesModalidadeCriar();
            AtualizarFiltroProprietarioImovelCriar(true);
            AtualizarFiltroContratantesCriar();
        }

        private void SearchBarContratos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter || ContratosDataGrid.ItemsSource == null)
            {
                return;
            }

            var texto = SearchBarContratos.Text?.ToLower() ?? string.Empty;

            foreach (var it in ContratosDataGrid.ItemsSource)
            {
                if (it is ContratoDAO contrato)
                {
                    var corresponde = string.IsNullOrWhiteSpace(texto) ||
                                     (contrato.Nome?.ToLower().Contains(texto) ?? false) ||
                                     (contrato.NomeTipoContrato?.ToLower().Contains(texto) ?? false) ||
                                     (contrato.NomeProprietario?.ToLower().Contains(texto) ?? false) ||
                                     (contrato.NomeImovel?.ToLower().Contains(texto) ?? false);

                    var row = ContratosDataGrid.ItemContainerGenerator.ContainerFromItem(it) as DataGridRow;
                    if (row != null)
                    {
                        row.Visibility = corresponde ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }
        }

        private async Task CarregarCombosContratoVisualizarAsync()
        {
            try
            {
                var tiposContratoTask = TipoContratoDAO.GetTiposContrato(HttpClientFixo);
                var modalidadesContratoTask = ModalidadeContratoDAO.GetModalidadesContrato(HttpClientFixo);
                var objetosContratoTask = ObjetoContratoDAO.GetObjetosContrato(HttpClientFixo);
                var proprietariosTask = ClienteDAO.GetProprietarios(HttpClientFixo);
                var locatariosTask = ClienteDAO.GetLocatários(HttpClientFixo);
                var fiadoresTask = ClienteDAO.GetFiadores(HttpClientFixo);
                var imoveisTask = ImovelDAO.GetImoveis(HttpClientFixo);

                await Task.WhenAll(
                    tiposContratoTask,
                    modalidadesContratoTask,
                    objetosContratoTask,
                    proprietariosTask,
                    locatariosTask,
                    fiadoresTask,
                    imoveisTask);

                ComboTipoContratoVisualizar.DisplayMemberPath = "Nome";
                ComboTipoContratoVisualizar.SelectedValuePath = "Id";
                ComboTipoContratoVisualizar.ItemsSource = tiposContratoTask.Result;

                ComboModalidadeContratoVisualizar.DisplayMemberPath = "Nome";
                ComboModalidadeContratoVisualizar.SelectedValuePath = "Id";
                ComboModalidadeContratoVisualizar.ItemsSource = modalidadesContratoTask.Result;

                ComboObjetoContratoVisualizar.DisplayMemberPath = "Nome";
                ComboObjetoContratoVisualizar.SelectedValuePath = "Id";
                ComboObjetoContratoVisualizar.ItemsSource = objetosContratoTask.Result;

                _proprietariosContratoVisualizar = proprietariosTask.Result?.ToList() ?? new List<ClienteDAO>();
                _imoveisContratoVisualizar = imoveisTask.Result?.ToList() ?? new List<ImovelDAO>();
                _locatariosContratoVisualizar = locatariosTask.Result?.ToList() ?? new List<ClienteDAO>();

                ComboContratoProprietarioVisualizar.DisplayMemberPath = "Nome";
                ComboContratoProprietarioVisualizar.SelectedValuePath = "Id";
                ComboContratoProprietarioVisualizar.ItemsSource = _proprietariosContratoVisualizar;

                ComboContratoImovelVisualizar.DisplayMemberPath = "Logradouro";
                ComboContratoImovelVisualizar.SelectedValuePath = "Id";
                ComboContratoImovelVisualizar.ItemsSource = _imoveisContratoVisualizar;

                ComboContratoContratante1Visualizar.DisplayMemberPath = "Nome";
                ComboContratoContratante1Visualizar.SelectedValuePath = "Id";
                ComboContratoContratante1Visualizar.ItemsSource = _locatariosContratoVisualizar;

                ComboContratoContratante2Visualizar.DisplayMemberPath = "Nome";
                ComboContratoContratante2Visualizar.SelectedValuePath = "Id";
                ComboContratoContratante2Visualizar.ItemsSource = _locatariosContratoVisualizar;

                ComboContratoContratante3Visualizar.DisplayMemberPath = "Nome";
                ComboContratoContratante3Visualizar.SelectedValuePath = "Id";
                ComboContratoContratante3Visualizar.ItemsSource = _locatariosContratoVisualizar;

                ComboContratoContratante4Visualizar.DisplayMemberPath = "Nome";
                ComboContratoContratante4Visualizar.SelectedValuePath = "Id";
                ComboContratoContratante4Visualizar.ItemsSource = _locatariosContratoVisualizar;

                ComboContratoFiadorVisualizar.DisplayMemberPath = "Nome";
                ComboContratoFiadorVisualizar.SelectedValuePath = "Id";
                ComboContratoFiadorVisualizar.ItemsSource = fiadoresTask.Result;

                AtualizarPermissoesModalidadeVisualizar();
                AtualizarFiltroProprietarioImovelVisualizar(true);
                AtualizarFiltroContratantesVisualizar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados do contrato para visualização: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnFecharModalContratosVisualizar_Click(object sender, RoutedEventArgs e)
        {
            ContratoModalOverlayVisualizar.Visibility = Visibility.Hidden;
            AtualizarPermissoesModalidadeVisualizar();
            AtualizarFiltroProprietarioImovelVisualizar(true);
            AtualizarFiltroContratantesVisualizar();
        }

        private async void BtnInativarContrato_Click(object sender, RoutedEventArgs e)
        {
            var confirm = MessageBox.Show("Tem certeza que deseja inativar?", "Inativar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes)
            {
                return;
            }

            var id = 0;
            if (sender is Button button && button.CommandParameter is int idParam)
            {
                id = idParam;
            }
            else if (ContratosDataGrid.SelectedItem is ContratoDAO contrato)
            {
                id = contrato.Id;
            }

            if (id == 0)
            {
                MessageBox.Show("Selecione um contrato para inativar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var contratoDto = new ContratoDTO();
                await contratoDto.InativarContrato(id, HttpClientFixo);
                MessageBox.Show("Contrato inativado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                await AdicionarItensGridContratos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inativar contrato: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnVisualizar_Click(object sender, RoutedEventArgs e)
        {
           ImovelDAO imovelSelecionado = ((ImovelDAO)ImoveisDataGrid.SelectedItem);
            
            List<ClienteDAO> listaClientes = ClienteDAO.GetClientes(HttpClientFixo);

            foreach (ClienteDAO cliente in listaClientes)
            {
                ComboProprietariosEditar.Items.Add(cliente.Nome.ToString());
            }

            List<IntencaoDAO> ListaIntencoes = IntencaoDAO.GetIntencao(HttpClientFixo);

            foreach (IntencaoDAO intencao in ListaIntencoes)
            {
                ComboIntencaoEditar.Items.Add(intencao.Nome.ToString());
            }

            List<TipoImovelDAO> ListaTiposImovel = TipoImovelDAO.GetTipoImovel(HttpClientFixo);
            foreach (TipoImovelDAO tipoImovel in ListaTiposImovel)
            {
                ComboTipoImovelEditar.Items.Add(tipoImovel.Nome.ToString());
            }

            ComboProprietariosEditar.Text = imovelSelecionado.NomeProprietario;
            ComboIntencaoEditar.Text = imovelSelecionado.NomeIntencao;
            ComboTipoImovelEditar.Text = imovelSelecionado.NomeTipoImovel;
            TxtBoxCepEditar.Text = imovelSelecionado.Cep;
            TxtBoxLogradouroEditar.Text = imovelSelecionado.Logradouro;
            TxtBoxNumeroEditar.Text = Convert.ToInt32(imovelSelecionado.Numero).ToString();
            TxtBoxPaisEditar.Text = imovelSelecionado.Pais;
            TxtBoxEstadoEditar.Text = imovelSelecionado.Estado;
            TxtBoxCidadeEditar.Text = imovelSelecionado.Cidade;
            TxtBoxBairroEditar.Text = imovelSelecionado.Bairro;
            TxtBoxMetragemEditar.Text = imovelSelecionado.Metragem.ToString();
            TxtBoxComplementoEditar.Text = imovelSelecionado.Complemento;
            TxtBoxValorEditar.Text = imovelSelecionado.Valor.ToString();
            TxtBoxCondominioEditar.Text = imovelSelecionado.Condominio.ToString();
            TxtBoxIptuEditar.Text = imovelSelecionado.Iptu.ToString();
            TxtBoxTaxaIncendioEditar.Text = imovelSelecionado.TaxaIncendio.ToString();
            TxtBoxForoEditar.Text = imovelSelecionado.Foro.ToString();
            TxtBoxObservacoesEditar.Text = imovelSelecionado.Observacao;
            TxtBoxDescricaoEditar.Text = imovelSelecionado.Descricao;
            TxtBoxInscricaoIptuEditar.Text = imovelSelecionado.InscricaoIptu;
            TxtBoxNumeroCbmerjEditar.Text = imovelSelecionado.NumeroCbmerj;
            ImovelModalOverlayEditar.Visibility = Visibility.Visible;

        }

        private async void BtnEditarModalImovel_Click(object sender, RoutedEventArgs e)
        {
            ImovelDAO imovelSelecionado = ((ImovelDAO)ImoveisDataGrid.SelectedItem);
            ImovelDTO imovelAtualizado = new ImovelDTO();

            if (!double.TryParse(TxtBoxMetragemEditar.Text, out double metragem))
            {
                MessageBox.Show("Metragem inválida.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!double.TryParse(TxtBoxValorEditar.Text, out double valor))
            {
                MessageBox.Show("Valor inválido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!double.TryParse(TxtBoxIptuEditar.Text, out double iptu))
            {
                MessageBox.Show("IPTU inválido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!double.TryParse(TxtBoxTaxaIncendioEditar.Text, out double taxaIncendio))
            {
                MessageBox.Show("Taxa de Incêndio inválida.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!double.TryParse(TxtBoxForoEditar.Text, out double foro))
            {
                MessageBox.Show("Foro inválido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(TxtBoxNumeroEditar.Text, out int numero))
            {
                MessageBox.Show("Número inválido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!double.TryParse(TxtBoxCondominioEditar.Text, out double condominio))
            {
                MessageBox.Show("Valor de Condomínio inválido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            imovelAtualizado.TaxaIncendio = Convert.ToDecimal(TxtBoxTaxaIncendioEditar.Text);
            imovelAtualizado.Foro = Convert.ToDecimal(TxtBoxForoEditar.Text);
            imovelAtualizado.Iptu = Convert.ToDecimal(TxtBoxIptuEditar.Text);
            imovelAtualizado.Valor = Convert.ToDecimal(TxtBoxValorEditar.Text);
            imovelAtualizado.Metragem = Convert.ToDecimal(TxtBoxMetragemEditar.Text);
            imovelAtualizado.Descricao = TxtBoxDescricaoEditar.Text;
            imovelAtualizado.Observacao = TxtBoxObservacoesEditar.Text;
            imovelAtualizado.Condominio = Convert.ToDecimal(TxtBoxCondominioEditar.Text);
            imovelAtualizado.Complemento = TxtBoxComplementoEditar.Text;
            imovelAtualizado.Bairro = TxtBoxBairroEditar.Text;
            imovelAtualizado.Cidade = TxtBoxCidadeEditar.Text;
            imovelAtualizado.Estado = TxtBoxEstadoEditar.Text;
            imovelAtualizado.Pais = TxtBoxPaisEditar.Text;
            imovelAtualizado.Numero = Convert.ToInt32(TxtBoxNumeroEditar.Text);
            imovelAtualizado.Logradouro = TxtBoxLogradouroEditar.Text;
            imovelAtualizado.Cep = TxtBoxCepEditar.Text;
            imovelAtualizado.InscricaoIptu = TxtBoxInscricaoIptuEditar.Text;
            imovelAtualizado.NumeroCbmerj = TxtBoxNumeroCbmerjEditar.Text;
            imovelAtualizado.Intencao = IntencaoDAO.GetIdPorNome(ComboIntencaoEditar.SelectedItem as string, HttpClientFixo);
            imovelAtualizado.TipoImovel = TipoImovelDAO.GetIdPorNome(ComboTipoImovelEditar.SelectedItem as string, HttpClientFixo);
            imovelAtualizado.Proprietario = ClienteDAO.GetIdPorNome(ComboProprietariosEditar.SelectedItem as string, HttpClientFixo);


            try
            {
                await imovelAtualizado.AtualizarImovel(imovelSelecionado.Id, HttpClientFixo);
                MessageBox.Show("Imóvel atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                ImovelModalOverlayEditar.Visibility = Visibility.Hidden;
                ComboProprietariosEditar.Items.Clear();
                ComboIntencaoEditar.Items.Clear();
                ComboTipoImovelEditar.Items.Clear();
                await AdicionarItensGridImoveis();

            } catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar imóvel: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void SearchBarFiadores_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var texto = SearchBarFiadores.Text?.ToLower() ?? string.Empty;
                var itens = FiadoresDataGrid.ItemsSource;

                foreach (var it in itens)
                {
                    var cliente = it as ClienteDAO;
                    if (cliente != null)
                    {
                        var corresponde = string.IsNullOrWhiteSpace(texto) ||
                                         (cliente.Nome?.ToLower().Contains(texto) ?? false) ||
                                         (cliente.CpfCnpj?.ToLower().Contains(texto) ?? false) ||
                                         (cliente.Email?.ToLower().Contains(texto) ?? false) ||
                                         (cliente.Telefone?.ToLower().Contains(texto) ?? false) ||
                                         (cliente.Endereco?.ToLower().Contains(texto) ?? false);
                        var row = FiadoresDataGrid.ItemContainerGenerator.ContainerFromItem(it) as DataGridRow;
                        if (row != null)
                        {
                            row.Visibility = corresponde ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        private void SearchBarLocatarios_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var texto = SearchBarLocatarios.Text?.ToLower() ?? string.Empty;
                var itens = LocatariosDataGrid.ItemsSource;

                foreach (var it in itens)
                {
                    var cliente = it as ClienteDAO;
                    if (cliente != null)
                    {
                        var corresponde = string.IsNullOrWhiteSpace(texto) ||
                                         (cliente.Nome?.ToLower().Contains(texto) ?? false) ||
                                         (cliente.CpfCnpj?.ToLower().Contains(texto) ?? false) ||
                                         (cliente.Email?.ToLower().Contains(texto) ?? false) ||
                                         (cliente.Telefone?.ToLower().Contains(texto) ?? false) ||
                                         (cliente.Endereco?.ToLower().Contains(texto) ?? false);
                        var row = LocatariosDataGrid.ItemContainerGenerator.ContainerFromItem(it) as DataGridRow;
                        if (row != null)
                        {
                            row.Visibility = corresponde ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        private void BtnAdicionarFiador_Click(object sender, RoutedEventArgs e)
        {
            FiadorModalOverlayCriar.Visibility = Visibility.Visible;
        }

        private void BtnFecharModalFiadorCriar_Click(object sender, RoutedEventArgs e)
        {
            FiadorModalOverlayCriar.Visibility = Visibility.Hidden;
            TxtFiadorNomeCriar.Clear();
            TxtFiadorCpfCnpjCriar.Clear();
            TxtFiadorIdentidadeCriar.Clear();
            TxtFiadorOrgaoExpedidorCriar.Clear();
            TxtFiadorNacionalidadeCriar.Clear();
            TxtFiadorNaturalidadeCriar.Clear();
            TxtFiadorEstadoCivilCriar.Clear();
            TxtFiadorProfissaoCriar.Clear();
            TxtFiadorEnderecoCriar.Clear();
            TxtFiadorBancoCriar.Clear();
            TxtFiadorAgenciaCriar.Clear();
            TxtFiadorContaCriar.Clear();
            TxtFiadorCodBancoCriar.Clear();
            TxtFiadorEmailCriar.Clear();
            TxtFiadorTelefoneCriar.Clear();
            DpFiadorNascimentoCriar.Text = "";
        }

        private async void BtnSalvarFiadorCriar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFiadorNomeCriar.Text) ||
                string.IsNullOrWhiteSpace(TxtFiadorCpfCnpjCriar.Text))
            {
                MessageBox.Show("Por favor, preencha os campos obrigatórios. (*)", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime? dataNascimento = null;
            if (!string.IsNullOrWhiteSpace(DpFiadorNascimentoCriar.Text))
            {
                if (!DateTime.TryParse(DpFiadorNascimentoCriar.Text, out var data))
                {
                    MessageBox.Show("Data de nascimento inválida.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                dataNascimento = data;
            }

            int tipoClienteId = TipoClienteDAO.GetIdPorNome("Fiador", HttpClientFixo);
            if (tipoClienteId == 0)
            {
                MessageBox.Show("Tipo de cliente fiador não encontrado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ClienteDTO cliente = new ClienteDTO
            {
                Nome = TxtFiadorNomeCriar.Text,
                CpfCnpj = TxtFiadorCpfCnpjCriar.Text,
                Identidade = TxtFiadorIdentidadeCriar.Text,
                OrgaoExpedidor = TxtFiadorOrgaoExpedidorCriar.Text,
                Nacionalidade = TxtFiadorNacionalidadeCriar.Text,
                Naturalidade = TxtFiadorNaturalidadeCriar.Text,
                EstadoCivil = TxtFiadorEstadoCivilCriar.Text,
                Profissao = TxtFiadorProfissaoCriar.Text,
                Endereco = TxtFiadorEnderecoCriar.Text,
                Banco = TxtFiadorBancoCriar.Text,
                Agencia = TxtFiadorAgenciaCriar.Text,
                Conta = TxtFiadorContaCriar.Text,
                CodBanco = TxtFiadorCodBancoCriar.Text,
                Email = TxtFiadorEmailCriar.Text,
                Telefone = TxtFiadorTelefoneCriar.Text,
                DataNascimento = dataNascimento,
                TipoCliente = new TipoClienteDAO { Id = tipoClienteId },
                Cadastrador = new UsuarioDAO { Id = UsuarioLogado.Id }
            };

            try
            {
                await cliente.CadastrarCliente(HttpClientFixo);
                MessageBox.Show("Fiador cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                BtnFecharModalFiadorCriar_Click(sender, e);
                await AdicionarItensGridFiadores();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cadastrar fiador: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnVisualizarFiador_Click(object sender, RoutedEventArgs e)
        {
            if (FiadoresDataGrid.SelectedItem is not ClienteDAO fiadorSelecionado)
            {
                MessageBox.Show("Selecione um fiador para visualizar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var listaClientes = await Task.Run(() => ClienteDAO.GetClientes(HttpClientFixo));
            var fiadorCompleto = listaClientes?.Find(cliente => cliente.Id == fiadorSelecionado.Id) ?? fiadorSelecionado;

            TxtFiadorNomeEditar.Text = fiadorCompleto.Nome;
            TxtFiadorCpfCnpjEditar.Text = fiadorCompleto.CpfCnpj;
            TxtFiadorIdentidadeEditar.Text = fiadorCompleto.Identidade;
            TxtFiadorOrgaoExpedidorEditar.Text = fiadorCompleto.OrgaoExpedidor;
            TxtFiadorNacionalidadeEditar.Text = fiadorCompleto.Nacionalidade;
            TxtFiadorNaturalidadeEditar.Text = fiadorCompleto.Naturalidade;
            TxtFiadorEstadoCivilEditar.Text = fiadorCompleto.EstadoCivil;
            TxtFiadorProfissaoEditar.Text = fiadorCompleto.Profissao;
            TxtFiadorEnderecoEditar.Text = fiadorCompleto.Endereco;
            TxtFiadorBancoEditar.Text = fiadorCompleto.Banco;
            TxtFiadorAgenciaEditar.Text = fiadorCompleto.Agencia;
            TxtFiadorContaEditar.Text = fiadorCompleto.Conta;
            TxtFiadorCodBancoEditar.Text = fiadorCompleto.CodBanco;
            TxtFiadorEmailEditar.Text = fiadorCompleto.Email;
            TxtFiadorTelefoneEditar.Text = fiadorCompleto.Telefone;
            DpFiadorNascimentoEditar.SelectedDate = fiadorCompleto.DataNascimento;

            FiadorModalOverlayEditar.Visibility = Visibility.Visible;
        }

        private void BtnFecharModalFiadorEditar_Click(object sender, RoutedEventArgs e)
        {
            FiadorModalOverlayEditar.Visibility = Visibility.Hidden;
        }

        private async void BtnSalvarFiadorEditar_Click(object sender, RoutedEventArgs e)
        {
            if (FiadoresDataGrid.SelectedItem is not ClienteDAO fiadorSelecionado)
            {
                MessageBox.Show("Selecione um fiador para editar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtFiadorNomeEditar.Text) ||
                string.IsNullOrWhiteSpace(TxtFiadorCpfCnpjEditar.Text))
            {
                MessageBox.Show("Por favor, preencha os campos obrigatórios. (*)", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime? dataNascimento = null;
            if (!string.IsNullOrWhiteSpace(DpFiadorNascimentoEditar.Text))
            {
                if (!DateTime.TryParse(DpFiadorNascimentoEditar.Text, out var data))
                {
                    MessageBox.Show("Data de nascimento inválida.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                dataNascimento = data;
            }

            var tipoClienteId = TipoClienteDAO.GetIdPorNome("Fiador", HttpClientFixo);
            if (tipoClienteId == 0)
            {
                MessageBox.Show("Tipo de cliente fiador não encontrado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var clienteAtualizado = new ClienteDTO
            {
                Id = fiadorSelecionado.Id,
                Nome = TxtFiadorNomeEditar.Text,
                CpfCnpj = TxtFiadorCpfCnpjEditar.Text,
                Identidade = TxtFiadorIdentidadeEditar.Text,
                OrgaoExpedidor = TxtFiadorOrgaoExpedidorEditar.Text,
                Nacionalidade = TxtFiadorNacionalidadeEditar.Text,
                Naturalidade = TxtFiadorNaturalidadeEditar.Text,
                EstadoCivil = TxtFiadorEstadoCivilEditar.Text,
                Profissao = TxtFiadorProfissaoEditar.Text,
                Endereco = TxtFiadorEnderecoEditar.Text,
                Banco = TxtFiadorBancoEditar.Text,
                Agencia = TxtFiadorAgenciaEditar.Text,
                Conta = TxtFiadorContaEditar.Text,
                CodBanco = TxtFiadorCodBancoEditar.Text,
                Email = TxtFiadorEmailEditar.Text,
                Telefone = TxtFiadorTelefoneEditar.Text,
                DataNascimento = dataNascimento,
                TipoCliente = new TipoClienteDAO { Id = tipoClienteId },
                Cadastrador = new UsuarioDAO { Id = UsuarioLogado.Id }
            };

            try
            {
                await clienteAtualizado.AtualizarCliente(fiadorSelecionado.Id, HttpClientFixo);
                MessageBox.Show("Fiador atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                FiadorModalOverlayEditar.Visibility = Visibility.Hidden;
                await AdicionarItensGridFiadores();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar fiador: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnInativarFiador_Click(object sender, RoutedEventArgs e)
        {
            var confirm = MessageBox.Show("Tem certeza que deseja inativar?", "Inativar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes)
            {
                return;
            }

            var id = 0;
            if (sender is Button button && button.CommandParameter is int idParam)
            {
                id = idParam;
            }
            else if (FiadoresDataGrid.SelectedItem is ClienteDAO cliente)
            {
                id = cliente.Id;
            }

            if (id == 0)
            {
                MessageBox.Show("Selecione um fiador para inativar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var clienteDto = new ClienteDTO();
                await clienteDto.InativarCliente(id, HttpClientFixo);
                MessageBox.Show("Fiador inativado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                await AdicionarItensGridFiadores();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inativar fiador: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnFecharModalImovelEditar_Click(object sender, RoutedEventArgs e)
        {
            ImovelModalOverlayEditar.Visibility = Visibility.Hidden;
            ComboProprietariosEditar.Items.Clear();
            ComboIntencaoEditar.Items.Clear();
            ComboTipoImovelEditar.Items.Clear();
            TxtBoxCepEditar.Clear();
            TxtBoxLogradouroEditar.Clear();
            TxtBoxNumeroEditar.Clear();
            TxtBoxPaisEditar.Clear();
            TxtBoxEstadoEditar.Clear();
            TxtBoxCidadeEditar.Clear();
            TxtBoxBairroEditar.Clear();
            TxtBoxComplementoEditar.Clear();
            TxtBoxCondominioEditar.Clear();
            TxtBoxObservacoesEditar.Clear();
            TxtBoxDescricaoEditar.Clear();
            TxtBoxMetragemEditar.Clear();
            TxtBoxValorEditar.Clear();
            TxtBoxIptuEditar.Clear();
            TxtBoxTaxaIncendioEditar.Clear();
            TxtBoxForoEditar.Clear();
        }

        private async void BtnExcluir_Click(object sender, RoutedEventArgs e)
        {
            var confirm = MessageBox.Show("Tem certeza que deseja excluir?", "Excluir", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm == MessageBoxResult.Yes)
            {
                ImovelDAO imovelSelecionado = ((ImovelDAO)ImoveisDataGrid.SelectedItem);
                ImovelDTO imovel = new ImovelDTO();
                try
                {
                    await imovel.InativarImovel(imovelSelecionado.Id, HttpClientFixo);
                    MessageBox.Show("Imóvel excluído com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    await AdicionarItensGridImoveis();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao excluir imóvel: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        private void BtnAdicionarLocatario_Click(object sender, RoutedEventArgs e)
        {
            LocatarioModalOverlayCriar.Visibility = Visibility.Visible;
        }

        private void BtnFecharModalLocatarioCriar_Click(object sender, RoutedEventArgs e)
        {
            LocatarioModalOverlayCriar.Visibility = Visibility.Hidden;
            TxtLocatarioNomeCriar.Clear();
            TxtLocatarioCpfCnpjCriar.Clear();
            TxtLocatarioIdentidadeCriar.Clear();
            TxtLocatarioOrgaoExpedidorCriar.Clear();
            TxtLocatarioNacionalidadeCriar.Clear();
            TxtLocatarioNaturalidadeCriar.Clear();
            TxtLocatarioEstadoCivilCriar.Clear();
            TxtLocatarioProfissaoCriar.Clear();
            TxtLocatarioEnderecoCriar.Clear();
            TxtLocatarioBancoCriar.Clear();
            TxtLocatarioAgenciaCriar.Clear();
            TxtLocatarioContaCriar.Clear();
            TxtLocatarioCodBancoCriar.Clear();
            TxtLocatarioEmailCriar.Clear();
            TxtLocatarioTelefoneCriar.Clear();
            DpLocatarioNascimentoCriar.Text = "";
        }

        private async void BtnSalvarLocatarioCriar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtLocatarioNomeCriar.Text) ||
                string.IsNullOrWhiteSpace(TxtLocatarioCpfCnpjCriar.Text))
            {
                MessageBox.Show("Por favor, preencha os campos obrigatórios. (*)", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime? dataNascimento = null;
            if (!string.IsNullOrWhiteSpace(DpLocatarioNascimentoCriar.Text))
            {
                if (!DateTime.TryParse(DpLocatarioNascimentoCriar.Text, out var data))
                {
                    MessageBox.Show("Data de nascimento inválida.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                dataNascimento = data;
            }

            int tipoClienteId = TipoClienteDAO.GetIdPorNome("Locatário", HttpClientFixo);
            if (tipoClienteId == 0)
            {
                MessageBox.Show("Tipo de cliente locatário não encontrado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ClienteDTO cliente = new ClienteDTO
            {
                Nome = TxtLocatarioNomeCriar.Text,
                CpfCnpj = TxtLocatarioCpfCnpjCriar.Text,
                Identidade = TxtLocatarioIdentidadeCriar.Text,
                OrgaoExpedidor = TxtLocatarioOrgaoExpedidorCriar.Text,
                Nacionalidade = TxtLocatarioNacionalidadeCriar.Text,
                Naturalidade = TxtLocatarioNaturalidadeCriar.Text,
                EstadoCivil = TxtLocatarioEstadoCivilCriar.Text,
                Profissao = TxtLocatarioProfissaoCriar.Text,
                Endereco = TxtLocatarioEnderecoCriar.Text,
                Banco = TxtLocatarioBancoCriar.Text,
                Agencia = TxtLocatarioAgenciaCriar.Text,
                Conta = TxtLocatarioContaCriar.Text,
                CodBanco = TxtLocatarioCodBancoCriar.Text,
                Email = TxtLocatarioEmailCriar.Text,
                Telefone = TxtLocatarioTelefoneCriar.Text,
                DataNascimento = dataNascimento,
                TipoCliente = new TipoClienteDAO { Id = tipoClienteId },
                Cadastrador = new UsuarioDAO { Id = UsuarioLogado.Id }
            };

            try
            {
                await cliente.CadastrarCliente(HttpClientFixo);
                MessageBox.Show("Locatário cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                BtnFecharModalLocatarioCriar_Click(sender, e);
                await AdicionarItensGridLocatários();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cadastrar locatário: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnVisualizarLocatario_Click(object sender, RoutedEventArgs e)
        {
            if (LocatariosDataGrid.SelectedItem is not ClienteDAO locatarioSelecionado)
            {
                MessageBox.Show("Selecione um locatário para visualizar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var listaClientes = await Task.Run(() => ClienteDAO.GetClientes(HttpClientFixo));
            var locatarioCompleto = listaClientes?.Find(cliente => cliente.Id == locatarioSelecionado.Id) ?? locatarioSelecionado;

            TxtLocatarioNomeEditar.Text = locatarioCompleto.Nome;
            TxtLocatarioCpfCnpjEditar.Text = locatarioCompleto.CpfCnpj;
            TxtLocatarioIdentidadeEditar.Text = locatarioCompleto.Identidade;
            TxtLocatarioOrgaoExpedidorEditar.Text = locatarioCompleto.OrgaoExpedidor;
            TxtLocatarioNacionalidadeEditar.Text = locatarioCompleto.Nacionalidade;
            TxtLocatarioNaturalidadeEditar.Text = locatarioCompleto.Naturalidade;
            TxtLocatarioEstadoCivilEditar.Text = locatarioCompleto.EstadoCivil;
            TxtLocatarioProfissaoEditar.Text = locatarioCompleto.Profissao;
            TxtLocatarioEnderecoEditar.Text = locatarioCompleto.Endereco;
            TxtLocatarioBancoEditar.Text = locatarioCompleto.Banco;
            TxtLocatarioAgenciaEditar.Text = locatarioCompleto.Agencia;
            TxtLocatarioContaEditar.Text = locatarioCompleto.Conta;
            TxtLocatarioCodBancoEditar.Text = locatarioCompleto.CodBanco;
            TxtLocatarioEmailEditar.Text = locatarioCompleto.Email;
            TxtLocatarioTelefoneEditar.Text = locatarioCompleto.Telefone;
            DpLocatarioNascimentoEditar.SelectedDate = locatarioCompleto.DataNascimento;

            LocatarioModalOverlayEditar.Visibility = Visibility.Visible;
        }

        private void BtnFecharModalLocatarioEditar_Click(object sender, RoutedEventArgs e)
        {
            LocatarioModalOverlayEditar.Visibility = Visibility.Hidden;
        }

        private async void BtnSalvarLocatarioEditar_Click(object sender, RoutedEventArgs e)
        {
            if (LocatariosDataGrid.SelectedItem is not ClienteDAO locatarioSelecionado)
            {
                MessageBox.Show("Selecione um locatário para editar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtLocatarioNomeEditar.Text) ||
                string.IsNullOrWhiteSpace(TxtLocatarioCpfCnpjEditar.Text))
            {
                MessageBox.Show("Por favor, preencha os campos obrigatórios. (*)", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime? dataNascimento = null;
            if (!string.IsNullOrWhiteSpace(DpLocatarioNascimentoEditar.Text))
            {
                if (!DateTime.TryParse(DpLocatarioNascimentoEditar.Text, out var data))
                {
                    MessageBox.Show("Data de nascimento inválida.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                dataNascimento = data;
            }

            var tipoClienteId = TipoClienteDAO.GetIdPorNome("Locatário", HttpClientFixo);
            if (tipoClienteId == 0)
            {
                MessageBox.Show("Tipo de cliente locatário não encontrado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var clienteAtualizado = new ClienteDTO
            {
                Id = locatarioSelecionado.Id,
                Nome = TxtLocatarioNomeEditar.Text,
                CpfCnpj = TxtLocatarioCpfCnpjEditar.Text,
                Identidade = TxtLocatarioIdentidadeEditar.Text,
                OrgaoExpedidor = TxtLocatarioOrgaoExpedidorEditar.Text,
                Nacionalidade = TxtLocatarioNacionalidadeEditar.Text,
                Naturalidade = TxtLocatarioNaturalidadeEditar.Text,
                EstadoCivil = TxtLocatarioEstadoCivilEditar.Text,
                Profissao = TxtLocatarioProfissaoEditar.Text,
                Endereco = TxtLocatarioEnderecoEditar.Text,
                Banco = TxtLocatarioBancoEditar.Text,
                Agencia = TxtLocatarioAgenciaEditar.Text,
                Conta = TxtLocatarioContaEditar.Text,
                CodBanco = TxtLocatarioCodBancoEditar.Text,
                Email = TxtLocatarioEmailEditar.Text,
                Telefone = TxtLocatarioTelefoneEditar.Text,
                DataNascimento = dataNascimento,
                TipoCliente = new TipoClienteDAO { Id = tipoClienteId },
                Cadastrador = new UsuarioDAO { Id = UsuarioLogado.Id }
            };

            try
            {
                await clienteAtualizado.AtualizarCliente(locatarioSelecionado.Id, HttpClientFixo);
                MessageBox.Show("Locatário atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                LocatarioModalOverlayEditar.Visibility = Visibility.Hidden;
                await AdicionarItensGridLocatários();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar locatário: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnInativarLocatario_Click(object sender, RoutedEventArgs e)
        {
            var confirm = MessageBox.Show("Tem certeza que deseja inativar?", "Inativar", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm != MessageBoxResult.Yes)
            {
                return;
            }

            var id = 0;
            if (sender is Button button && button.CommandParameter is int idParam)
            {
                id = idParam;
            }
            else if (LocatariosDataGrid.SelectedItem is ClienteDAO cliente)
            {
                id = cliente.Id;
            }

            if (id == 0)
            {
                MessageBox.Show("Selecione um locatário para inativar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var clienteDto = new ClienteDTO();
                await clienteDto.InativarCliente(id, HttpClientFixo);
                MessageBox.Show("Locatário inativado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                await AdicionarItensGridLocatários();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inativar locatário: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchBarProprietarios_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var texto = SearchBarProprietarios.Text?.ToLower() ?? string.Empty;
                var itens = ProprietariosDataGrid.ItemsSource;

                foreach (var it in itens)
                {
                    var cliente = it as ClienteDAO;
                    if (cliente != null)
                    {
                        var corresponde = string.IsNullOrWhiteSpace(texto) ||
                                         (cliente.Nome?.ToLower().Contains(texto) ?? false) ||
                                         (cliente.CpfCnpj?.ToLower().Contains(texto) ?? false) ||
                                         (cliente.Email?.ToLower().Contains(texto) ?? false) ||
                                         (cliente.Telefone?.ToLower().Contains(texto) ?? false) ||
                                         (cliente.Endereco?.ToLower().Contains(texto) ?? false);
                        var row = ProprietariosDataGrid.ItemContainerGenerator.ContainerFromItem(it) as DataGridRow;
                        if (row != null)
                        {
                            row.Visibility = corresponde ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        private async void BtnInativarProprietario_Click(object sender, RoutedEventArgs e)
        {
            var confirm = MessageBox.Show("Tem certeza que deseja inativar?", "Inativar", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
            {
                return;
            }

            var id = 0;
            if (sender is Button button && button.CommandParameter is int idParam)
            {
                id = idParam;
            }
            else if (sender is Button btn && btn.CommandParameter is string idText && int.TryParse(idText, out var parsedId))
            {
                id = parsedId;
            }
            else if (ProprietariosDataGrid.SelectedItem is ClienteDAO cliente)
            {
                id = cliente.Id;
            }

            if (id == 0)
            {
                MessageBox.Show("Selecione um proprietário para inativar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var clienteDto = new ClienteDTO();
                await clienteDto.InativarCliente(id, HttpClientFixo);
                MessageBox.Show("Proprietário inativado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                await AdcionarItensGridProprietarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inativar proprietário: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAdicionarFotos_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.Multiselect = true;
            Nullable<bool> result = openFileDlg.ShowDialog();

            if (result == true)
            {
                foreach (string fileName in openFileDlg.FileNames)
                {
                    if (fileName == null) continue;
                    if (!File.Exists(fileName)) continue;
                    if (!fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) &&
                        !fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) &&
                        !fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) &&
                        !fileName.EndsWith(".webp", StringComparison.OrdinalIgnoreCase) &&
                        !fileName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show($"Arquivo '{fileName}' não é um formato de imagem suportado.", "Formato Não Suportado", MessageBoxButton.OK, MessageBoxImage.Warning);
                        continue;
                    }
                    string filePath = fileName;
                    byte[] binFoto = File.ReadAllBytes(filePath);
                    _fotosSelecionadasBinario.Add(binFoto);
					_fotoIdsPreview.Add(null);
                    var bitmap = new BitmapImage();
                    using (var stream = new MemoryStream(binFoto))
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        bitmap.Freeze();
                    }
                    _fotosSelecionadasPreview.Add(bitmap);
                }
            }
        }

        private void UploadDropArea_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void UploadDropArea_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }

        private void UploadDropArea_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var filePath in files)
                {
                    try
                    {
                        var ext = System.IO.Path.GetExtension(filePath)?.ToLowerInvariant();
                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".webp" || ext == ".bmp")
                        {
                            byte[] binFoto = File.ReadAllBytes(filePath);
                            _fotosSelecionadasBinario.Add(binFoto);
							_fotoIdsPreview.Add(null);
                            var bitmap = new BitmapImage();
                            using (var stream = new MemoryStream(binFoto))
                            {
                                bitmap.BeginInit();
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.StreamSource = stream;
                                bitmap.EndInit();
                                bitmap.Freeze();
                            }
                            _fotosSelecionadasPreview.Add(bitmap);
                        }
                    } 
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao carregar imagem: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnRemoverFoto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is ImageSource img)
            {
				int index = _fotosSelecionadasPreview.IndexOf(img);
				if (index >= 0)
				{
					_fotosSelecionadasPreview.RemoveAt(index);
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
            }
        }

        private void BtnFecharModalImovelFotoCriar_Click(object sender, RoutedEventArgs e)
        {
			ImovelFotosModalOverlayCriar.Visibility = Visibility.Hidden;
			ImovelFotosModalOverlayEditar.Visibility = Visibility.Hidden;
            _fotosSelecionadasPreview.Clear();
            _fotosSelecionadasBinario.Clear();
			_fotoIdsPreview.Clear();
			_fotosRemovidas.Clear();
            _idImovelCadastrado = 0;
        }

        private async void BtnCadastrarModalImovelFotoCriar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < _fotosSelecionadasBinario.Count; i++)
                {
                    FotoDTO foto = new FotoDTO
                    {
                        ImovelId = _idImovelCadastrado,
                        Bin = _fotosSelecionadasBinario[i],
                        NomeArquivo = $"ImovelId[{_idImovelCadastrado}] - Foto[{i}]",
                        CadastradorId = UsuarioLogado.Id,
                        TipoFoto = 1,
                        Principal = false
                    };

                    await foto.CadastrarFoto(HttpClientFixo);
                }

                MessageBox.Show("Fotos cadastradas com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                ImovelFotosModalOverlayCriar.Visibility = Visibility.Hidden;
                _fotosSelecionadasPreview.Clear();
                _fotosSelecionadasBinario.Clear();
				_fotoIdsPreview.Clear();
				_fotosRemovidas.Clear();
                _idImovelCadastrado = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cadastrar fotos: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

		private async void BtnEditarFotos_Click(object sender, RoutedEventArgs e)
		{
			if (ImoveisDataGrid.SelectedItem is not ImovelDAO imovelSelecionado)
			{
				MessageBox.Show("Selecione um imóvel para editar as fotos.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			ImovelModalOverlayEditar.Visibility = Visibility.Hidden;
			_idImovelCadastrado = imovelSelecionado.Id;
			ImovelFotosModalOverlayEditar.Visibility = Visibility.Visible;

			_fotosSelecionadasPreview.Clear();
			_fotosSelecionadasBinario.Clear();
			_fotoIdsPreview.Clear();
			_fotosRemovidas.Clear();

			try
			{
                var fotosImovel = await FotoDAO.GetFotosPorImovel(_idImovelCadastrado, HttpClientFixo);

				foreach (var foto in fotosImovel)
				{
					if (foto.Bin == null || foto.Bin.Length == 0) continue;

					_fotosSelecionadasBinario.Add(foto.Bin);
					_fotoIdsPreview.Add(foto.Id);
					var bitmap = new BitmapImage();
					using (var stream = new MemoryStream(foto.Bin))
					{
						bitmap.BeginInit();
						bitmap.CacheOption = BitmapCacheOption.OnLoad;
						bitmap.StreamSource = stream;
						bitmap.EndInit();
						bitmap.Freeze();
					}
					_fotosSelecionadasPreview.Add(bitmap);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Erro ao carregar fotos do imóvel: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
			}

		}
		private void BtnFecharModalImovelFotoEditar_Click(object sender, RoutedEventArgs e)
		{
			_fotosSelecionadasBinario.Clear();
			_fotosSelecionadasPreview.Clear();
			_fotoIdsPreview.Clear();
			_fotosRemovidas.Clear();

			ImovelFotosModalOverlayEditar.Visibility = Visibility.Hidden;

			ImovelModalOverlayEditar.Visibility = Visibility.Visible;
		}

		private async void BtnCriarModalImovelFotoEditar_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				foreach (var fotoId in _fotosRemovidas)
				{
					var fotoDto = new FotoDTO();
					await fotoDto.InativarFoto(fotoId, HttpClientFixo);
				}

				for (int i = 0; i < _fotosSelecionadasBinario.Count; i++)
				{
					if (_fotoIdsPreview.Count > i && _fotoIdsPreview[i].HasValue)
					{
						continue;
					}

					FotoDTO foto = new FotoDTO
					{
						ImovelId = _idImovelCadastrado,
						Bin = _fotosSelecionadasBinario[i],
						NomeArquivo = $"ImovelId[{_idImovelCadastrado}] - Foto[{i}]",
						CadastradorId = UsuarioLogado.Id,
						TipoFoto = 1,
						Principal = false
					};

                    await foto.CadastrarFoto(HttpClientFixo);
				}

				MessageBox.Show("Fotos atualizadas com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
				ImovelFotosModalOverlayEditar.Visibility = Visibility.Hidden;
				_fotosSelecionadasBinario.Clear();
				_fotosSelecionadasPreview.Clear();
				_fotoIdsPreview.Clear();
				_fotosRemovidas.Clear();
				_idImovelCadastrado = 0;
				ImovelModalOverlayEditar.Visibility = Visibility.Visible;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Erro ao atualizar fotos: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

        private void SearchBarImoveis_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var texto = SearchBarImoveis.Text.ToLower();
                var itens = ImoveisDataGrid.ItemsSource;

                foreach (var it in itens)
                {
                    var imovel = it as ImovelDAO;
                    if (imovel != null)
                    {
                        var corresponde = imovel.NomeProprietario.ToLower().Contains(texto) ||
                                         imovel.Logradouro.ToLower().Contains(texto) ||
                                         imovel.Cidade.ToLower().Contains(texto) ||
                                         imovel.Bairro.ToLower().Contains(texto) ||
                                         imovel.Cep.ToLower().Contains(texto) ||
                                         imovel.Intencao.Nome.ToLower().Contains(texto) ||
                                         imovel.Valor.ToString().ToLower().Contains(texto) ||
                                         imovel.Metragem.ToString().ToLower().Contains(texto);
                        var row = ImoveisDataGrid.ItemContainerGenerator.ContainerFromItem(it) as DataGridRow;
                        if (row != null)
                        {
                            row.Visibility = corresponde ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                }
            }
        }

        private void BtnAdicionarProprietario_Click(object sender, RoutedEventArgs e)
        {
            ProprietarioModalOverlayCriar.Visibility = Visibility.Visible;
        }

        private void BtnFecharModalProprietarioCriar_Click(object sender, RoutedEventArgs e)
        {
            ProprietarioModalOverlayCriar.Visibility = Visibility.Hidden;
            TxtClienteNomeCriar.Clear();
            TxtClienteCpfCnpjCriar.Clear();
            TxtClienteIdentidadeCriar.Clear();
            TxtClienteOrgaoExpedidorCriar.Clear();
            TxtClienteNacionalidadeCriar.Clear();
            TxtClienteNaturalidadeCriar.Clear();
            TxtClienteEstadoCivilCriar.Clear();
            TxtClienteProfissaoCriar.Clear();
            TxtClienteEnderecoCriar.Clear();
            TxtClienteBancoCriar.Clear();
            TxtClienteAgenciaCriar.Clear();
            TxtClienteContaCriar.Clear();
            TxtClienteCodBancoCriar.Clear();
            TxtClienteEmailCriar.Clear();
            TxtClienteTelefoneCriar.Clear();
            DpClienteNascimentoCriar.Text = "";
        }

        private async void BtnSalvarProprietarioCriar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtClienteNomeCriar.Text) ||
                string.IsNullOrWhiteSpace(TxtClienteCpfCnpjCriar.Text))
            {
                MessageBox.Show("Por favor, preencha os campos obrigatórios. (*)", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime? dataNascimento = null;
            if (!string.IsNullOrWhiteSpace(DpClienteNascimentoCriar.Text))
            {
                if (!DateTime.TryParse(DpClienteNascimentoCriar.Text, out var data))
                {
                    MessageBox.Show("Data de nascimento inválida.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                dataNascimento = data;
            }

            int tipoClienteId = TipoClienteDAO.GetIdPorNome("Proprietário", HttpClientFixo);
            if (tipoClienteId == 0)
            {
                MessageBox.Show("Tipo de cliente proprietário não encontrado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ClienteDTO cliente = new ClienteDTO
            {
                Nome = TxtClienteNomeCriar.Text,
                CpfCnpj = TxtClienteCpfCnpjCriar.Text,
                Identidade = TxtClienteIdentidadeCriar.Text,
                OrgaoExpedidor = TxtClienteOrgaoExpedidorCriar.Text,
                Nacionalidade = TxtClienteNacionalidadeCriar.Text,
                Naturalidade = TxtClienteNaturalidadeCriar.Text,
                EstadoCivil = TxtClienteEstadoCivilCriar.Text,
                Profissao = TxtClienteProfissaoCriar.Text,
                Endereco = TxtClienteEnderecoCriar.Text,
                Banco = TxtClienteBancoCriar.Text,
                Agencia = TxtClienteAgenciaCriar.Text,
                Conta = TxtClienteContaCriar.Text,
                CodBanco = TxtClienteCodBancoCriar.Text,
                Email = TxtClienteEmailCriar.Text,
                Telefone = TxtClienteTelefoneCriar.Text,
                DataNascimento = dataNascimento,
                TipoCliente = new TipoClienteDAO { Id = tipoClienteId },
                Cadastrador = new UsuarioDAO { Id = UsuarioLogado.Id }
            };

            try
            {
                await cliente.CadastrarCliente(HttpClientFixo);
                MessageBox.Show("Proprietário cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                ProprietarioModalOverlayCriar.Visibility = Visibility.Hidden;
                TxtClienteNomeCriar.Clear();
                TxtClienteCpfCnpjCriar.Clear();
                TxtClienteIdentidadeCriar.Clear();
                TxtClienteOrgaoExpedidorCriar.Clear();
                TxtClienteNacionalidadeCriar.Clear();
                TxtClienteNaturalidadeCriar.Clear();
                TxtClienteEstadoCivilCriar.Clear();
                TxtClienteProfissaoCriar.Clear();
                TxtClienteEnderecoCriar.Clear();
                TxtClienteBancoCriar.Clear();
                TxtClienteAgenciaCriar.Clear();
                TxtClienteContaCriar.Clear();
                TxtClienteCodBancoCriar.Clear();
                TxtClienteEmailCriar.Clear();
                TxtClienteTelefoneCriar.Clear();
                DpClienteNascimentoCriar.Text = "";

                await AdcionarItensGridProprietarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cadastrar proprietário: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // TODO: Refatorar para funcionar corretamente.
        private void BtnVisualizarProprietario_Click(object sender, RoutedEventArgs e)
        {
            if (ProprietariosDataGrid.SelectedItem is not ClienteDAO proprietarioSelecionado)
            {
                MessageBox.Show("Selecione um proprietário para visualizar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            TxtClienteNomeEditar.Text = proprietarioSelecionado.Nome;
            TxtClienteCpfCnpjEditar.Text = proprietarioSelecionado.CpfCnpj;
            TxtClienteIdentidadeEditar.Text = proprietarioSelecionado.Identidade;
            TxtClienteOrgaoExpedidorEditar.Text = proprietarioSelecionado.OrgaoExpedidor;
            TxtClienteNacionalidadeEditar.Text = proprietarioSelecionado.Nacionalidade;
            TxtClienteNaturalidadeEditar.Text = proprietarioSelecionado.Naturalidade;
            TxtClienteEstadoCivilEditar.Text = proprietarioSelecionado.EstadoCivil;
            TxtClienteProfissaoEditar.Text = proprietarioSelecionado.Profissao;
            TxtClienteEnderecoEditar.Text = proprietarioSelecionado.Endereco;
            TxtClienteBancoEditar.Text = proprietarioSelecionado.Banco;
            TxtClienteAgenciaEditar.Text = proprietarioSelecionado.Agencia;
            TxtClienteContaEditar.Text = proprietarioSelecionado.Conta;
            TxtClienteCodBancoEditar.Text = proprietarioSelecionado.CodBanco;
            TxtClienteEmailEditar.Text = proprietarioSelecionado.Email;
            TxtClienteTelefoneEditar.Text = proprietarioSelecionado.Telefone;
            DpClienteNascimentoEditar.SelectedDate = proprietarioSelecionado.DataNascimento;

            ProprietarioModalOverlayEditar.Visibility = Visibility.Visible;
        }

        private void BtnFecharModalProprietarioEditar_Click(object sender, RoutedEventArgs e)
        {
            ProprietarioModalOverlayEditar.Visibility = Visibility.Hidden;
        }

        private async void BtnSalvarProprietarioEditar_Click(object sender, RoutedEventArgs e)
        {
            if (ProprietariosDataGrid.SelectedItem is not ClienteDAO proprietarioSelecionado)
            {
                MessageBox.Show("Selecione um proprietário para editar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtClienteNomeEditar.Text) ||
                string.IsNullOrWhiteSpace(TxtClienteCpfCnpjEditar.Text))
            {
                MessageBox.Show("Por favor, preencha os campos obrigatórios. (*)", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DateTime? dataNascimento = null;
            if (!string.IsNullOrWhiteSpace(DpClienteNascimentoEditar.Text))
            {
                if (!DateTime.TryParse(DpClienteNascimentoEditar.Text, out var data))
                {
                    MessageBox.Show("Data de nascimento inválida.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                dataNascimento = data;
            }

            var tipoClienteId = TipoClienteDAO.GetIdPorNome("Proprietário", HttpClientFixo);
            if (tipoClienteId == 0)
            {
                MessageBox.Show("Tipo de cliente proprietário não encontrado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var clienteAtualizado = new ClienteDTO
            {
                Nome = TxtClienteNomeEditar.Text,
                CpfCnpj = TxtClienteCpfCnpjEditar.Text,
                Identidade = TxtClienteIdentidadeEditar.Text,
                OrgaoExpedidor = TxtClienteOrgaoExpedidorEditar.Text,
                Nacionalidade = TxtClienteNacionalidadeEditar.Text,
                Naturalidade = TxtClienteNaturalidadeEditar.Text,
                EstadoCivil = TxtClienteEstadoCivilEditar.Text,
                Profissao = TxtClienteProfissaoEditar.Text,
                Endereco = TxtClienteEnderecoEditar.Text,
                Banco = TxtClienteBancoEditar.Text,
                Agencia = TxtClienteAgenciaEditar.Text,
                Conta = TxtClienteContaEditar.Text,
                CodBanco = TxtClienteCodBancoEditar.Text,
                Email = TxtClienteEmailEditar.Text,
                Telefone = TxtClienteTelefoneEditar.Text,
                DataNascimento = dataNascimento,
                TipoCliente = new TipoClienteDAO { Id = tipoClienteId }
            };

            try
            {
                await clienteAtualizado.AtualizarCliente(proprietarioSelecionado.Id, HttpClientFixo);
                MessageBox.Show("Proprietário atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                ProprietarioModalOverlayEditar.Visibility = Visibility.Hidden;
                await AdcionarItensGridProprietarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar proprietário: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnSalvarContratoCriar_Click(object sender, RoutedEventArgs e)
        {
            string nomeContrato = TxtContratoNomeCriar.Text;
            TipoContratoDAO tipoContratoSelecionado = ComboTipoContratoCriar.SelectedItem as TipoContratoDAO;
            ModalidadeContratoDAO modalidadeSelecionada = ComboModalidadeContratoCriar.SelectedItem as ModalidadeContratoDAO;
            ObjetoContratoDAO objetoSelecionado = ComboObjetoContratoCriar.SelectedItem as ObjetoContratoDAO;
            ClienteDAO proprietarioSelecionado = ComboContratoProprietarioCriar.SelectedItem as ClienteDAO;
            ImovelDAO imovelSelecionado = ComboContratoImovelCriar.SelectedItem as ImovelDAO;
            ClienteDAO locatario1Selecionado = ComboContratoContratante1Criar.SelectedItem as ClienteDAO;
            ClienteDAO locatario2Selecionado = ComboContratoContratante2Criar.SelectedItem as ClienteDAO;
            ClienteDAO locatario3Selecionado = ComboContratoContratante3Criar.SelectedItem as ClienteDAO;
            ClienteDAO locatario4Selecionado = ComboContratoContratante4Criar.SelectedItem as ClienteDAO;
            ClienteDAO fiadorSelecionado = ComboContratoFiadorCriar.SelectedItem as ClienteDAO;
            DateTime? inicio = DpContratoDataInicioCriar.SelectedDate;
            string proposta = TxtContratoPropostaSegFiancaCriar.Text;
            string apolice = TxtContratoApoliceSegFiancaCriar.Text;


            if (!int.TryParse(TxtContratoPrazoMesesCriar.Text, out int prazo))
            {
                MessageBox.Show("Prazo inválido Utilize apenas números.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(DpContratoVencimentoCriar.Text, out int vencimento) || (vencimento < 1 || vencimento > 31))
            {
                MessageBox.Show("Vencimento inválido Utilize apenas números de 1 a 31.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (PossuiContratantesRepetidos(locatario1Selecionado, locatario2Selecionado, locatario3Selecionado, locatario4Selecionado))
            {
                MessageBox.Show("Não é permitido repetir contratantes.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(nomeContrato) ||
                modalidadeSelecionada == null ||
                objetoSelecionado == null ||
                proprietarioSelecionado == null ||
                imovelSelecionado == null ||
                locatario1Selecionado == null ||
                inicio == null)
            {
                MessageBox.Show("Por favor, preencha todos os campos obrigatórios.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ContratoDTO contrato = new ContratoDTO
            {
                Nome = nomeContrato,
                Cadastrador = new UsuarioDAO { Id = UsuarioLogado.Id },
                TipoContrato = tipoContratoSelecionado,
                ModalidadeContrato = modalidadeSelecionada,
                ObjetoContrato = objetoSelecionado,
                Proprietario = proprietarioSelecionado,
                Imovel = imovelSelecionado,
                Contratante1 = locatario1Selecionado,
                Contratante2 = locatario2Selecionado,
                Contratante3 = locatario3Selecionado,
                Contratante4 = locatario4Selecionado,
                Fiador = fiadorSelecionado,
                DataInicioVigencia = inicio.Value,
                PrazoMeses = prazo,
                Vencimento = vencimento,
                PropostaSegFianca = proposta,
                ApoliceSegFianca = apolice

            };

            await contrato.CadastrarContrato(HttpClientFixo);

            MessageBox.Show("Imóvel Contrato com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            TxtContratoNomeCriar.Clear();
            ComboModalidadeContratoCriar.SelectedItem = null;
            ComboObjetoContratoCriar.SelectedItem = null;
            ComboContratoProprietarioCriar.SelectedItem = null;
            ComboContratoImovelCriar.SelectedItem = null;
            ComboContratoContratante1Criar.SelectedItem = null;
            ComboContratoContratante2Criar.SelectedItem = null;
            ComboContratoContratante3Criar.SelectedItem = null;
            ComboContratoContratante4Criar.SelectedItem = null;
            ComboContratoFiadorCriar.SelectedItem = null;
            DpContratoDataInicioCriar.SelectedDate = null;
            TxtContratoPrazoMesesCriar.Clear();
            DpContratoVencimentoCriar.Clear();
            TxtContratoPropostaSegFiancaCriar.Clear();
            TxtContratoApoliceSegFiancaCriar.Clear();

            AtualizarPermissoesModalidadeCriar();
            AtualizarFiltroProprietarioImovelCriar(true);
            AtualizarFiltroContratantesCriar();

            ContratoModalOverlayCriar.Visibility = Visibility.Hidden;

            await AdicionarItensGridContratos();
        }

        private async void BtnSalvarContratoVisualizar_Click(object sender, RoutedEventArgs e)
        {
            if (ContratosDataGrid.SelectedItem is not ContratoDAO contratoSelecionado)
            {
                MessageBox.Show("Nenhum contrato selecionado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            static int? ObterIdSelecionado(ComboBox comboBox)
            {
                if (comboBox.SelectedItem == null)
                {
                    return null;
                }

                return comboBox.SelectedValue switch
                {
                    int idInt => idInt,
                    long idLong => (int)idLong,
                    string idTexto when int.TryParse(idTexto, out var idParseado) => idParseado,
                    _ => null
                };
            }

            string nomeContrato = TxtContratoNomeVisualizar.Text;
            int? tipoContratoId = ObterIdSelecionado(ComboTipoContratoVisualizar);
            int? modalidadeId = ObterIdSelecionado(ComboModalidadeContratoVisualizar);
            var modalidadeSelecionada = ComboModalidadeContratoVisualizar.SelectedItem as ModalidadeContratoDAO;
            int? objetoId = ObterIdSelecionado(ComboObjetoContratoVisualizar);
            int? proprietarioId = ObterIdSelecionado(ComboContratoProprietarioVisualizar);
            int? imovelId = ObterIdSelecionado(ComboContratoImovelVisualizar);
            int? locatario1Id = ObterIdSelecionado(ComboContratoContratante1Visualizar);
            int? locatario2Id = ObterIdSelecionado(ComboContratoContratante2Visualizar);
            int? locatario3Id = ObterIdSelecionado(ComboContratoContratante3Visualizar);
            int? locatario4Id = ObterIdSelecionado(ComboContratoContratante4Visualizar);
            int? fiadorId = ObterIdSelecionado(ComboContratoFiadorVisualizar);
            DateTime? inicio = DpContratoDataInicioVisualizar.SelectedDate;
            string proposta = TxtContratoPropostaSegFiancaVisualizar.Text;
            string apolice = TxtContratoApoliceSegFiancaVisualizar.Text;

            if (!ModalidadeEhFiador(modalidadeSelecionada))
            {
                fiadorId = null;
            }

            if (!ModalidadeEhSeguroFianca(modalidadeSelecionada))
            {
                proposta = null;
                apolice = null;
            }
            else
            {
                proposta = string.IsNullOrWhiteSpace(proposta) ? null : proposta;
                apolice = string.IsNullOrWhiteSpace(apolice) ? null : apolice;
            }

            if (!int.TryParse(TxtContratoPrazoMesesVisualizar.Text, out int prazo))
            {
                MessageBox.Show("Prazo inválido. Utilize apenas números.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(TxtContratoVencimentoVisualizar.Text, out int vencimento) || (vencimento < 1 || vencimento > 31))
            {
                MessageBox.Show("Vencimento inválido. Utilize apenas números de 1 a 31.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrEmpty(nomeContrato) ||
                !tipoContratoId.HasValue ||
                !modalidadeId.HasValue ||
                !objetoId.HasValue ||
                !proprietarioId.HasValue ||
                !imovelId.HasValue ||
                !locatario1Id.HasValue ||
                inicio == null)
            {
                MessageBox.Show("Por favor, preencha todos os campos obrigatórios.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                ContratoDTO contratoAtualizado = new ContratoDTO
                {
                    Id = contratoSelecionado.Id,
                    Nome = nomeContrato,
                    Cadastrador = new UsuarioDAO { Id = UsuarioLogado.Id },
                    TipoContrato = new TipoContratoDAO { Id = tipoContratoId.Value },
                    ModalidadeContrato = new ModalidadeContratoDAO { Id = modalidadeId.Value },
                    ObjetoContrato = new ObjetoContratoDAO { Id = objetoId.Value },
                    Proprietario = new ClienteDAO { Id = proprietarioId.Value },
                    Imovel = new ImovelDAO { Id = imovelId.Value },
                    Contratante1 = new ClienteDAO { Id = locatario1Id.Value },
                    Contratante2 = locatario2Id.HasValue ? new ClienteDAO { Id = locatario2Id.Value } : null,
                    Contratante3 = locatario3Id.HasValue ? new ClienteDAO { Id = locatario3Id.Value } : null,
                    Contratante4 = locatario4Id.HasValue ? new ClienteDAO { Id = locatario4Id.Value } : null,
                    Fiador = fiadorId.HasValue ? new ClienteDAO { Id = fiadorId.Value } : null,
                    DataInicioVigencia = inicio.Value,
                    PrazoMeses = prazo,
                    Vencimento = vencimento,
                    PropostaSegFianca = proposta,
                    ApoliceSegFianca = apolice
                };

                await contratoAtualizado.AtualizarContrato(contratoSelecionado.Id, HttpClientFixo);

                MessageBox.Show("Contrato atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                ContratoModalOverlayVisualizar.Visibility = Visibility.Hidden;

                await AdicionarItensGridContratos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar contrato: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnVisualizarContrato_Click(object sender, RoutedEventArgs e)
        {
            if (ContratosDataGrid.SelectedItem is not ContratoDAO contratoSelecionadoGrid)
            {
                MessageBox.Show("Selecione um contrato para visualizar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var contratoSelecionado = await ContratoDAO.GetContratoPorId(contratoSelecionadoGrid.Id, HttpClientFixo);
                if (contratoSelecionado == null)
                {
                    MessageBox.Show("Contrato não encontrado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await CarregarCombosContratoVisualizarAsync();

                TxtContratoNomeVisualizar.Text = contratoSelecionado.Nome;

                ComboTipoContratoVisualizar.SelectedValue = contratoSelecionado.TipoContrato?.Id ?? contratoSelecionado.TipoContratoId;
                ComboModalidadeContratoVisualizar.SelectedValue = contratoSelecionado.ModalidadeContrato?.Id ?? contratoSelecionado.ModalidadeContratoId;
                ComboObjetoContratoVisualizar.SelectedValue = contratoSelecionado.ObjetoContrato?.Id ?? contratoSelecionado.ObjetoContratoId;
                ComboContratoProprietarioVisualizar.SelectedValue = contratoSelecionado.Proprietario?.Id ?? contratoSelecionado.ProprietarioId;
                ComboContratoImovelVisualizar.SelectedValue = contratoSelecionado.Imovel?.Id ?? contratoSelecionado.ImovelId;
                ComboContratoContratante1Visualizar.SelectedValue = contratoSelecionado.Contratante1?.Id ?? contratoSelecionado.Contratante1Id;
                ComboContratoContratante2Visualizar.SelectedValue = contratoSelecionado.Contratante2?.Id ?? contratoSelecionado.Contratante2Id;
                ComboContratoContratante3Visualizar.SelectedValue = contratoSelecionado.Contratante3?.Id ?? contratoSelecionado.Contratante3Id;
                ComboContratoContratante4Visualizar.SelectedValue = contratoSelecionado.Contratante4?.Id ?? contratoSelecionado.Contratante4Id;
                ComboContratoFiadorVisualizar.SelectedValue = contratoSelecionado.Fiador?.Id ?? contratoSelecionado.FiadorId;

                if (ComboTipoContratoVisualizar.SelectedItem == null) ComboTipoContratoVisualizar.Text = contratoSelecionado.TipoContrato?.Nome;
                if (ComboModalidadeContratoVisualizar.SelectedItem == null) ComboModalidadeContratoVisualizar.Text = contratoSelecionado.ModalidadeContrato?.Nome;
                if (ComboObjetoContratoVisualizar.SelectedItem == null) ComboObjetoContratoVisualizar.Text = contratoSelecionado.ObjetoContrato?.Nome;
                if (ComboContratoProprietarioVisualizar.SelectedItem == null) ComboContratoProprietarioVisualizar.Text = contratoSelecionado.Proprietario?.Nome;
                if (ComboContratoImovelVisualizar.SelectedItem == null) ComboContratoImovelVisualizar.Text = contratoSelecionado.Imovel?.Logradouro;
                if (ComboContratoContratante1Visualizar.SelectedItem == null) ComboContratoContratante1Visualizar.Text = contratoSelecionado.Contratante1?.Nome;
                if (ComboContratoContratante2Visualizar.SelectedItem == null) ComboContratoContratante2Visualizar.Text = contratoSelecionado.Contratante2?.Nome;
                if (ComboContratoContratante3Visualizar.SelectedItem == null) ComboContratoContratante3Visualizar.Text = contratoSelecionado.Contratante3?.Nome;
                if (ComboContratoContratante4Visualizar.SelectedItem == null) ComboContratoContratante4Visualizar.Text = contratoSelecionado.Contratante4?.Nome;

                DpContratoDataInicioVisualizar.SelectedDate = contratoSelecionado.DataInicioVigencia;
                TxtContratoPrazoMesesVisualizar.Text = contratoSelecionado.PrazoMeses.ToString();
                TxtContratoVencimentoVisualizar.Text = contratoSelecionado.Vencimento.ToString();
                TxtContratoPropostaSegFiancaVisualizar.Text = contratoSelecionado.PropostaSegFianca;
                TxtContratoApoliceSegFiancaVisualizar.Text = contratoSelecionado.ApoliceSegFianca;

                AtualizarFiltroProprietarioImovelVisualizar(true);
                AtualizarFiltroContratantesVisualizar();
                AtualizarPermissoesModalidadeVisualizar();

                ContratoModalOverlayVisualizar.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar contrato: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnGerarContratoVisualizar_Click(object sender, RoutedEventArgs e)
        {
            GeradorContratoPdf gerador = new GeradorContratoPdf();

            gerador.CriarContrato();
        }
    }
}
