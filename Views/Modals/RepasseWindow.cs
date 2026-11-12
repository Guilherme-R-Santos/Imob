using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Imob.Models;
using Imob.Services;

namespace Imob.Views.Modals
{
    public sealed class RepasseWindow : Window
    {
        private readonly CobrancaDAO _cobranca;
        private readonly HttpClient _client;
        private readonly RepasseService _service;
        private readonly Func<CobrancaDAO, HttpClient, Task> _abrirRepasseManual;
        private readonly DataGrid _grid;
        private readonly TextBox _transferencia;
        private readonly TextBlock _mensagem;
        private readonly Button _atualizar;
        private readonly Button _manual;
        private readonly Button _conciliar;
        private readonly Button _retomar;
        private readonly Button _copiarComprovante;
        private bool _ocupado;
        private bool _consultaValida;
        private IReadOnlyList<RegistroRepasse> _registros = Array.Empty<RegistroRepasse>();

        public RepasseWindow(CobrancaDAO cobranca, HttpClient client, bool administradorConfirmadoPelaApi,
            Func<CobrancaDAO, HttpClient, Task> abrirRepasseManual = null)
        {
            if (!administradorConfirmadoPelaApi) throw new UnauthorizedAccessException("A permissão de administrador deve ser confirmada pela API antes de abrir a janela de repasses.");
            _cobranca = cobranca ?? throw new ArgumentNullException(nameof(cobranca));
            if (cobranca.Id <= 0) throw new ArgumentException("A cobrança deve estar cadastrada.", nameof(cobranca));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _service = new RepasseService(client);
            _abrirRepasseManual = abrirRepasseManual;

            Title = "Acompanhar repasses";
            Width = 1050;
            Height = 700;
            MinWidth = 760;
            MinHeight = 520;
            Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(248, 249, 251));
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var painel = new DockPanel { Margin = new Thickness(18) };
            var topo = new StackPanel();
            var cabecalho = new Border { Background = System.Windows.Media.Brushes.White, CornerRadius = new CornerRadius(10), Padding = new Thickness(16), Margin = new Thickness(0, 0, 0, 10), BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(228, 231, 236)), BorderThickness = new Thickness(1) };
            var textoCabecalho = new StackPanel();
            textoCabecalho.Children.Add(new TextBlock { Text = cobranca.Nome, FontSize = 20, FontWeight = FontWeights.SemiBold, Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(242, 127, 29)) });
            textoCabecalho.Children.Add(new TextBlock { Text = $"Acompanhamento dos valores destinados a esta cobrança (nº {cobranca.Id}).", TextWrapping = TextWrapping.Wrap, Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(92, 99, 112)), Margin = new Thickness(0, 4, 0, 0) });
            cabecalho.Child = textoCabecalho;
            topo.Children.Add(cabecalho);
            topo.Children.Add(new TextBlock { Text = "A transferência só é considerada concluída quando aparecer como “Concluído”. Cada conta de destino é processada separadamente; se houver dúvida ou falha, consulte os detalhes antes de tentar qualquer ação.", TextWrapping = TextWrapping.Wrap, Margin = new Thickness(2, 0, 2, 8), Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(72, 78, 88)) });
            DockPanel.SetDock(topo, Dock.Top);
            painel.Children.Add(topo);

            var rodape = new StackPanel { Margin = new Thickness(0, 8, 0, 0) };
            var botoes = new WrapPanel();
            _atualizar = CriarBotao("Atualizar");
            _manual = CriarBotao("Repasse manual");
            _conciliar = CriarBotao("Resolver envio pendente");
            _retomar = CriarBotao("Tentar novamente os não enviados");
            _copiarComprovante = CriarBotao("Copiar comprovante");
            _transferencia = new TextBox { Width = 220, MaxLength = 128, Margin = new Thickness(4), ToolTip = "Informe o código da transferência após confirmar com o suporte." };
            botoes.Children.Add(_atualizar);
            botoes.Children.Add(_manual);
            botoes.Children.Add(_copiarComprovante);
            rodape.Children.Add(botoes);
            var avancado = new Expander { Header = "Precisa de ajuda com um repasse?", Margin = new Thickness(4, 4, 4, 0), Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(76, 82, 92)) };
            var painelAvancado = new StackPanel { Margin = new Thickness(8) };
            painelAvancado.Children.Add(new TextBlock { Text = "Use estas opções somente após confirmar com o suporte se o valor já foi enviado ou se ainda pode ser tentado. Não repita uma solicitação enquanto houver dúvida.", TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 0, 0, 6), Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(92, 99, 112)) });
            var linhaAvancada = new WrapPanel();
            linhaAvancada.Children.Add(new TextBlock { Text = "Código informado pelo suporte:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(4, 0, 4, 0) });
            linhaAvancada.Children.Add(_transferencia);
            linhaAvancada.Children.Add(_conciliar);
            linhaAvancada.Children.Add(_retomar);
            painelAvancado.Children.Add(linhaAvancada);
            avancado.Content = painelAvancado;
            rodape.Children.Add(avancado);

            _mensagem = new TextBlock { TextWrapping = TextWrapping.Wrap, Margin = new Thickness(4, 8, 4, 0) };
            rodape.Children.Add(_mensagem);
            DockPanel.SetDock(rodape, Dock.Bottom);
            painel.Children.Add(rodape);

            _grid = new DataGrid { IsReadOnly = true, AutoGenerateColumns = false, CanUserAddRows = false, SelectionMode = DataGridSelectionMode.Single, SelectionUnit = DataGridSelectionUnit.FullRow, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
            AdicionarColuna("Destino", "DestinatarioDescricao");
            AdicionarColuna("Solicitação", "OrigemDescricao");
            AdicionarColuna("Valor", "Valor", "C2");
            AdicionarColuna("Situação", "SituacaoDescricao");
            AdicionarColuna("Observação", "ObservacaoAmigavel");
            AdicionarColuna("Comprovante", "ComprovanteDescricao");
            AdicionarColuna("Solicitado em", "CriadoEmLocal");
            painel.Children.Add(_grid);
            Content = painel;

            _grid.SelectionChanged += (_, _) => AtualizarBotoes();
            _atualizar.Click += async (_, _) => await ExecutarAsync(null);
            _manual.Click += async (_, _) => await AbrirManualAsync();
            _conciliar.Click += async (_, _) => await ConciliarAsync();
            _retomar.Click += async (_, _) => await RetomarAsync();
            _copiarComprovante.Click += (_, _) => CopiarComprovante();
            Loaded += async (_, _) => await ExecutarAsync(null);
            AtualizarBotoes();
        }

        private static Button CriarBotao(string texto) => new Button { Content = texto, Padding = new Thickness(10, 5, 10, 5), Margin = new Thickness(4) };

        private void AdicionarColuna(string titulo, string propriedade, string formato = null)
        {
            _grid.Columns.Add(new DataGridTextColumn { Header = titulo, Binding = new Binding(propriedade) { StringFormat = formato } });
        }

        private void AtualizarBotoes()
        {
            var selecionado = _grid.SelectedItem as RegistroRepasse;
            _atualizar.IsEnabled = !_ocupado;
            _manual.IsEnabled = !_ocupado && _consultaValida && _abrirRepasseManual != null && _registros.Count == 0 && _cobranca.Ativo && _cobranca.Pago && _cobranca.Status == "RECEIVED" && !string.IsNullOrWhiteSpace(_cobranca.IdCobrancaAsaas);
            _conciliar.IsEnabled = !_ocupado && _consultaValida && (selecionado?.Status == "INCERTO" || selecionado?.Status == "ENVIANDO");
            _transferencia.IsEnabled = _conciliar.IsEnabled;
            _retomar.IsEnabled = !_ocupado && _consultaValida && _registros.Any(r => r.Status == "BLOQUEADO");
            _copiarComprovante.IsEnabled = !_ocupado && !string.IsNullOrWhiteSpace(selecionado?.ComprovanteUrl);
        }

        private void CopiarComprovante()
        {
            if (_grid.SelectedItem is not RegistroRepasse registro || string.IsNullOrWhiteSpace(registro.ComprovanteUrl)) return;
            try
            {
                Clipboard.SetText(registro.ComprovanteUrl);
                _mensagem.Text = "Link do comprovante copiado.";
            }
            catch (System.Runtime.InteropServices.ExternalException ex)
            {
                _mensagem.Text = "Não foi possível copiar o link do comprovante: " + ex.Message;
            }
        }

        private async Task AbrirManualAsync()
        {
            if (!_manual.IsEnabled) return;
            if (MessageBox.Show(this, "A API permite uma única solicitação por cobrança Asaas, com até dois destinatários. Consulte os registros e confirme que não houve solicitação anterior antes de continuar. O aceite apenas reserva o processamento. Continuar?", "Repasse manual", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            await ExecutarAsync(() => _abrirRepasseManual(_cobranca, _client));
        }

        private async Task ConciliarAsync()
        {
            if (!_conciliar.IsEnabled || _grid.SelectedItem is not RegistroRepasse registro) return;
            var id = _transferencia.Text.Trim();
            if (string.IsNullOrWhiteSpace(id) || id.Length > 128)
            {
                _mensagem.Text = "Informe o ID da transferência no Asaas, com até 128 caracteres.";
                return;
            }
            if (MessageBox.Show(this, $"Confirme que investigou a transferência {id} no Asaas para o repasse {registro.Id}. A API verificará referência e valor. Continuar?", "Conciliar", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            await ExecutarAsync(async () => { var resposta = await _service.ConciliarAsync(registro.Id, id); _mensagem.Text = $"Conciliação: {resposta}"; });
        }

        private async Task RetomarAsync()
        {
            if (!_retomar.IsEnabled) return;
            if (MessageBox.Show(this, "A retomada coloca TODOS os registros BLOQUEADO desta cobrança na fila. Confirme que não foram enviados e que saldo/configuração foram corrigidos. Estornos, contestações ou problemas em um repasse anterior podem impedir o envio. Continuar?", "Retomar repasses", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
            await ExecutarAsync(async () => { var resposta = await _service.RetomarNaoEnviadosAsync(_cobranca.Id); _mensagem.Text = $"Retomada aceita para processamento, não concluída: {resposta}"; });
        }

        private static string MensagemAmigavel(Exception ex)
        {
            if (ex is RepasseApiException api)
                return api.StatusCode switch
                {
                    System.Net.HttpStatusCode.Forbidden => "Seu usuário não tem permissão para esta ação.",
                    System.Net.HttpStatusCode.Unauthorized => "Sua sessão expirou. Entre novamente no sistema.",
                    System.Net.HttpStatusCode.ServiceUnavailable => "O serviço de repasses está temporariamente indisponível. Tente mais tarde.",
                    System.Net.HttpStatusCode.Conflict => "Esta ação não pode ser realizada agora. Atualize os dados e verifique se já existe uma solicitação.",
                    System.Net.HttpStatusCode.NotFound => "Não encontramos o registro solicitado.",
                    _ => "Não foi possível concluir a solicitação. Consulte os detalhes do repasse ou peça ajuda ao suporte."
                };
            if (ex is HttpRequestException || ex is TaskCanceledException)
                return "Não foi possível confirmar a comunicação com o serviço. O pedido pode ter sido recebido; confira os registros antes de tentar novamente.";
            return "Não foi possível concluir a solicitação. Confira os dados e tente novamente mais tarde.";
        }

        private async Task ExecutarAsync(Func<Task> operacao)
        {
            if (_ocupado) return;
            _ocupado = true;
            _consultaValida = false;
            _mensagem.Text = "";
            AtualizarBotoes();
            try
            {
                if (operacao != null) await operacao();
            }
            catch (Exception ex)
            {
                _mensagem.Text = MensagemAmigavel(ex) + " Consulte os registros antes de tentar novamente.";
            }
            try
            {
                _registros = await _service.PorCobrancaAsync(_cobranca.Id);
                _grid.ItemsSource = _registros;
                _consultaValida = true;
                if (string.IsNullOrEmpty(_mensagem.Text))
                    _mensagem.Text = _registros.Count == 0 ? "Nenhum registro local para esta cobrança. A API ainda verifica se existe solicitação para a mesma cobrança Asaas." : "Registros locais atualizados.";
            }
            catch (Exception ex)
            {
                _grid.ItemsSource = null;
                _registros = Array.Empty<RegistroRepasse>();
                _mensagem.Text += $"\nNão foi possível atualizar os registros: {ex.Message}. Atualize antes de realizar operações.";
            }
            finally
            {
                _ocupado = false;
                AtualizarBotoes();
            }
        }
    }
}
