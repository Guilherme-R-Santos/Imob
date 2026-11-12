using System.Windows;
using System.Windows.Controls;
using Imob.Models;

namespace Imob.Views.Modals
{
    public sealed class ConfiguracaoRepasseWindow : Window
    {
        private readonly RepasseContasEditor _editor = new();
        private readonly CheckBox _automatico = new() { Content = "Ativar repasse automático nas cobranças deste contrato", Margin = new Thickness(0, 0, 0, 6), FontWeight = FontWeights.SemiBold, Foreground = System.Windows.Media.Brushes.Black };
        private readonly bool _manual;
        public bool Automatico => _automatico.IsChecked == true;
        public ContaRepasse Conta1 { get; private set; }
        public ContaRepasse Conta2 { get; private set; }

        public ConfiguracaoRepasseWindow(bool manual, bool automatico, ContaRepasse conta1, ContaRepasse conta2, ContaRepasse dadosProprietario)
        {
            _manual = manual;
            Title = manual ? "Revisar repasse" : "Configurar repasses";
            Width = 780;
            Height = 800;
            MinWidth = 620;
            MinHeight = 560;
            Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(248, 249, 251));
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            AplicarEstilos();
            _automatico.IsChecked = automatico;
            _editor.CarregarContas(conta1, conta2);
            if (conta1 == null && dadosProprietario != null)
                _editor.PreencherDadosCliente(1, dadosProprietario);

            var painel = new DockPanel { Margin = new Thickness(20) };
            var cabecalho = new Border { Background = System.Windows.Media.Brushes.White, CornerRadius = new CornerRadius(10), Padding = new Thickness(18), Margin = new Thickness(0, 0, 0, 12), BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(238, 232, 226)), BorderThickness = new Thickness(1) };
            var textoCabecalho = new StackPanel();
            textoCabecalho.Children.Add(new TextBlock { Text = manual ? "Revise os destinatários" : "Como deseja dividir os valores?", FontSize = 21, FontWeight = FontWeights.SemiBold, Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(242, 127, 29)) });
            textoCabecalho.Children.Add(new TextBlock { Text = "Você pode cadastrar até dois destinatários. Confira os dados com atenção antes de continuar.", TextWrapping = TextWrapping.Wrap, Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(92, 99, 112)), Margin = new Thickness(0, 5, 0, 0) });
            cabecalho.Child = textoCabecalho;
            DockPanel.SetDock(cabecalho, Dock.Top);
            painel.Children.Add(cabecalho);

            var rodape = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 12, 0, 0) };
            var cancelar = new Button { Content = "Cancelar", Width = 110, Height = 38, Margin = new Thickness(0, 0, 10, 0), IsCancel = true, Background = System.Windows.Media.Brushes.White, Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(76, 82, 92)), BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(210, 214, 220)), BorderThickness = new Thickness(1), Cursor = System.Windows.Input.Cursors.Hand };
            var confirmar = new Button { Content = manual ? "Continuar" : "Salvar configuração", MinWidth = manual ? 120 : 170, Height = 38, Padding = new Thickness(16, 0, 16, 0), IsDefault = true, Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(234, 133, 47)), Foreground = System.Windows.Media.Brushes.White, BorderThickness = new Thickness(0), FontWeight = FontWeights.SemiBold, Cursor = System.Windows.Input.Cursors.Hand };
            confirmar.Click += Confirmar;
            rodape.Children.Add(cancelar);
            rodape.Children.Add(confirmar);
            DockPanel.SetDock(rodape, Dock.Bottom);
            painel.Children.Add(rodape);

            var conteudo = new StackPanel();
            if (!manual)
            {
                var automaticoCard = new Border { Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 247, 239)), BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(248, 213, 184)), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Padding = new Thickness(14), Margin = new Thickness(0, 0, 0, 12) };
                var automaticoPainel = new StackPanel();
                automaticoPainel.Children.Add(_automatico);
                automaticoPainel.Children.Add(new TextBlock { Text = "Quando uma cobrança for recebida, o sistema iniciará o repasse automaticamente, se houver pelo menos um destinatário válido.", TextWrapping = TextWrapping.Wrap, Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(92, 99, 112)), Margin = new Thickness(24, 0, 0, 0) });
                automaticoCard.Child = automaticoPainel;
                conteudo.Children.Add(automaticoCard);
            }
            var ajuda = new Border { Background = System.Windows.Media.Brushes.White, BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(228, 231, 236)), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Padding = new Thickness(14), Margin = new Thickness(0, 0, 0, 12) };
            ajuda.Child = new TextBlock { Text = (dadosProprietario != null && !manual ? "Alguns dados do proprietário foram preenchidos automaticamente; revise e complete o que faltar.\n\n" : string.Empty) + "Valor fixo: informe em reais (50 = R$ 50,00). Percentual: informe a porcentagem do valor líquido (50 = 50%). Valores percentuais são arredondados para centavos; o total a repassar não pode ultrapassar o valor líquido recebido.", TextWrapping = TextWrapping.Wrap, Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(72, 78, 88)) };
            conteudo.Children.Add(ajuda);
            conteudo.Children.Add(_editor);
            painel.Children.Add(new ScrollViewer { Content = conteudo, VerticalScrollBarVisibility = ScrollBarVisibility.Auto, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled });
            Content = painel;
        }

        private void AplicarEstilos()
        {
            Resources.Add(typeof(TextBox), new Style(typeof(TextBox))
            {
                Setters = { new Setter(Control.FontSizeProperty, 14d), new Setter(Control.PaddingProperty, new Thickness(10, 8, 10, 8)), new Setter(Control.MinHeightProperty, 38d), new Setter(Control.BorderBrushProperty, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(210, 214, 220))), new Setter(Control.BorderThicknessProperty, new Thickness(1)), new Setter(Control.BackgroundProperty, System.Windows.Media.Brushes.White), new Setter(Control.ForegroundProperty, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 50, 58))) }
            });
            Resources.Add(typeof(ComboBox), new Style(typeof(ComboBox))
            {
                Setters = { new Setter(Control.FontSizeProperty, 14d), new Setter(Control.PaddingProperty, new Thickness(10, 7, 10, 7)), new Setter(Control.MinHeightProperty, 38d), new Setter(Control.BorderBrushProperty, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(210, 214, 220))), new Setter(Control.BorderThicknessProperty, new Thickness(1)), new Setter(Control.BackgroundProperty, System.Windows.Media.Brushes.White), new Setter(Control.ForegroundProperty, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 50, 58))) }
            });
            Resources.Add(typeof(GroupBox), new Style(typeof(GroupBox))
            {
                Setters = { new Setter(Control.PaddingProperty, new Thickness(12)), new Setter(Control.MarginProperty, new Thickness(0, 0, 0, 12)), new Setter(Control.BorderBrushProperty, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(228, 231, 236))), new Setter(Control.BackgroundProperty, System.Windows.Media.Brushes.White), new Setter(Control.ForegroundProperty, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(242, 127, 29))), new Setter(Control.FontSizeProperty, 15d), new Setter(Control.FontWeightProperty, FontWeights.SemiBold) }
            });
            Resources.Add(typeof(Label), new Style(typeof(Label))
            {
                Setters = { new Setter(Control.FontSizeProperty, 13d), new Setter(Control.ForegroundProperty, new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(83, 89, 99))) }
            });
        }

        private void Confirmar(object sender, RoutedEventArgs e)
        {
            if (!_editor.TryObterContas(out var primeira, out var segunda, out var erro, _manual || Automatico))
            {
                MessageBox.Show(this, erro, "Dados de repasse", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Conta1 = primeira;
            Conta2 = segunda;
            DialogResult = true;
        }
    }
}
