using Imob.Components;
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

namespace Imob
{
    public partial class Sistema : Window
    {
        public UsuarioDAO UsuarioLogado { get; set; }

        public void SetUsuarioLogado(UsuarioDAO usuarioLogado)
        {
            UsuarioLogado = usuarioLogado;
            UsuarioAtivo.Content = UsuarioLogado.Login;
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
                var listaImoveis = await ImovelDAO.GetImoveis();
                ImoveisDataGrid.ItemsSource = listaImoveis;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar imóveis: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void AdicionarItensComboProprietarios()
        {
            List<ClienteDAO> listaClientes = ClienteDAO.GetClientes();

            foreach (ClienteDAO cliente in listaClientes)
            {
                ComboProprietarios.Items.Add(cliente.Nome.ToString());
            }
        }

        public void AdicionarItensComboIntencoes()
        {
            List<IntencaoDAO> ListaIntencoes = IntencaoDAO.GetIntencao();

            foreach (IntencaoDAO intencao in ListaIntencoes)
            {
                ComboIntencao.Items.Add(intencao.Nome.ToString());
            }
        }

        public void AdicionarItensComboTiposImovel()
        {
            List<TipoImovelDAO> ListaTiposImovel = TipoImovelDAO.GetTipoImovel();
            foreach (TipoImovelDAO tipoImovel in ListaTiposImovel)
            {
                ComboTipoImovel.Items.Add(tipoImovel.Nome.ToString());
            }
        }

        // TODO: Implementar resetar filtros e valores dos panels
        public void ResetarPanels()
        {

        }

        // TODO: Implementar obtenção de informações da API
        public void GetInformacoesApi(string panelAtivo)
        {

        }

        // Funções auxilires Fim

        public Sistema()
        {

            InitializeComponent();
            WindowState = WindowState.Maximized;

            //Loaded += async (s, e) =>
            //{
            //    await AdicionarItensGridImoveis();
            //};

            // Mouse Enter and Leave Events Inicio

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
            // TODO: Resetar valores e trazer todos os imóveis
            ResetarPanels();
            await AdicionarItensGridImoveis();
            ImoveisPanel.Visibility = Visibility.Visible;

            // recarrega a grid
            await AdicionarItensGridImoveis();
        }

        private void ContratosTreeCompraVenda_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            // TODO: Resetar valores e trazer contratos com filtro de tipo Compra e Venda
            ResetarPanels();
            ContratosPanel.Visibility = Visibility.Visible;
        }

        private void ContratosTreeLocacao_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            // TODO: Resetar valores e trazer contratos com filtro de tipo locação
            ResetarPanels();
            ContratosPanel.Visibility = Visibility.Visible;
        }

        private void ProprietariosTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            // TODO: Resetar valores e trazer todos os proprietários
            ResetarPanels();
            ProprietariosPanel.Visibility = Visibility.Visible;
        }

        private void LocatariosTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            // TODO: Resetar valores e trazer todos os locatários
            ResetarPanels();
            LocatariosPanel.Visibility = Visibility.Visible;
        }

        private void FiadoresTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            // TODO: Resetar valores e trazer todos os fiadores
            ResetarPanels();
            FiadoresPanel.Visibility = Visibility.Visible;
        }

        private void VistoriaTreeListar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            // TODO: Resetar valores e trazer todas as vistorias
            ResetarPanels();
            VistoriasPanel.Visibility = Visibility.Visible;
        }

        private void VistoriaTreeCriar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            // TODO: Resetar valores e trazer painel de criação de vistoria
            ResetarPanels();
            VistoriasPanel.Visibility = Visibility.Visible;
        }

        private void BtnAdicionarImovel_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Finalizar
            AdicionarItensComboProprietarios();
            AdicionarItensComboIntencoes();
            AdicionarItensComboTiposImovel();
            ImoveilModalOverlayCriar.Visibility = Visibility.Visible;
        }

        private void BtnFecharModalImovel_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Resetar valores do modal
            ImoveilModalOverlayCriar.Visibility = Visibility.Hidden;
            ComboProprietarios.Items.Clear();
            ComboIntencao.Items.Clear();
            ComboTipoImovel.Items.Clear();
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

            var complemento = TxtBoxComplemento.Text;
            var condominio = TxtBoxCondominio.Text;
            var observacao = TxtBoxObservacoes.Text;
            var descricao = TxtBoxDescricao.Text;

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

            //TODO: Fix
            imovel.Proprietario = ClienteDAO.GetIdPorNome(clienteSelecionado);
            imovel.TipoImovel = TipoImovelDAO.GetIdPorNome(tipoImovelSelecionado);
            imovel.Intencao = IntencaoDAO.GetIdPorNome(intencaoSelecionada);
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
            imovel.Cadastrador = UsuarioLogado.Id;

            await imovel.CadastrarImovel();

            MessageBox.Show("Imóvel cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

            ComboProprietarios.SelectedIndex = -1;
            ComboIntencao.SelectedIndex = -1;
            ComboTipoImovel.SelectedIndex = -1;
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


            ImoveilModalOverlayCriar.Visibility = Visibility.Hidden;

        }

        //Click Events Fim
    }
}
