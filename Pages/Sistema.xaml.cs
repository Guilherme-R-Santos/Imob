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
using System.IO;
using System.IO.Enumeration;
using System.Collections.ObjectModel;

namespace Imob
{
    public partial class Sistema : Window
    {
        public UsuarioDAO UsuarioLogado { get; set; }

        private ObservableCollection<ImageSource> _fotosSelecionadasPreview = new ObservableCollection<ImageSource>();

        private List<byte[]> _fotosSelecionadasBinario = new List<byte[]>();

		private List<int?> _fotoIdsPreview = new List<int?>();

		private List<int> _fotosRemovidas = new List<int>();

        private int _idImovelCadastrado;

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

        public async Task AdcionarItensGridProprietarios()
        {
            try
            {
                var listaProprietarios = await ClienteDAO.GetProprietarios();
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
                var listaLocatarios = await ClienteDAO.GetLocatários();
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
                var listaFiadores = await ClienteDAO.GetFiadores();
                FiadoresDataGrid.ItemsSource = listaFiadores;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar fiadores: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task AdicionarItensComboProprietarios()
        {
            List<ClienteDAO> listaClientes = await ClienteDAO.GetProprietarios();

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

        // Funções auxilires Fim

        public Sistema()
        {

            InitializeComponent();
            WindowState = WindowState.Maximized;

            FotosSelecionadasList.ItemsSource = _fotosSelecionadasPreview;
			FotosSelecionadasListEditar.ItemsSource = _fotosSelecionadasPreview;

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

        private void ContratosTreeCompraVenda_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            ContratosPanel.Visibility = Visibility.Visible;
        }

        private void ContratosTreeLocacao_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
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

        private void VistoriaTreeCriar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            VistoriasPanel.Visibility = Visibility.Visible;
        }

        private void BtnAdicionarImovel_Click(object sender, RoutedEventArgs e)
        {
            AdicionarItensComboProprietarios();
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
            imovel.InscricaoIptu = inscricaoIptu;
            imovel.NumeroCbmerj = numeroCbmerj;
            imovel.Cadastrador = UsuarioLogado.Id;



            int idImovel = await imovel.CadastrarImovel();

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

        private void BtnVisualizar_Click(object sender, RoutedEventArgs e)
        {
           ImovelDAO imovelSelecionado = ((ImovelDAO)ImoveisDataGrid.SelectedItem);
            
            List<ClienteDAO> listaClientes = ClienteDAO.GetClientes();

            foreach (ClienteDAO cliente in listaClientes)
            {
                ComboProprietariosEditar.Items.Add(cliente.Nome.ToString());
            }

            List<IntencaoDAO> ListaIntencoes = IntencaoDAO.GetIntencao();

            foreach (IntencaoDAO intencao in ListaIntencoes)
            {
                ComboIntencaoEditar.Items.Add(intencao.Nome.ToString());
            }

            List<TipoImovelDAO> ListaTiposImovel = TipoImovelDAO.GetTipoImovel();
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
            TxtBoxComplementoEditar.Text = imovelSelecionado.Complemento;
            TxtBoxCondominioEditar.Text = imovelSelecionado.Condominio.HasValue ? imovelSelecionado.Condominio.Value.ToString() : string.Empty;
            TxtBoxObservacoesEditar.Text = imovelSelecionado.Observacao;
            TxtBoxDescricaoEditar.Text = imovelSelecionado.Descricao;
            TxtBoxMetragemEditar.Text = Convert.ToInt32(imovelSelecionado.Metragem).ToString();
            TxtBoxValorEditar.Text = Convert.ToInt32(imovelSelecionado.Valor).ToString();
            TxtBoxIptuEditar.Text = imovelSelecionado.Iptu.HasValue ? imovelSelecionado.Iptu.Value.ToString() : string.Empty;
            TxtBoxTaxaIncendioEditar.Text = imovelSelecionado.TaxaIncendio.HasValue ? imovelSelecionado.TaxaIncendio.Value.ToString() : string.Empty;
            TxtBoxForoEditar.Text = imovelSelecionado.Foro.HasValue ? imovelSelecionado.Foro.Value.ToString() : string.Empty;
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
            imovelAtualizado.Intencao = IntencaoDAO.GetIdPorNome(ComboIntencaoEditar.SelectedItem as string);
            imovelAtualizado.TipoImovel = TipoImovelDAO.GetIdPorNome(ComboTipoImovelEditar.SelectedItem as string);
            imovelAtualizado.Proprietario = ClienteDAO.GetIdPorNome(ComboProprietariosEditar.SelectedItem as string);


            try
            {
                await imovelAtualizado.AtualizarImovel(imovelSelecionado.Id);
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
                    await imovel.InativarImovel(imovelSelecionado.Id);
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
                await clienteDto.InativarCliente(id);
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

                    await foto.CadastrarFoto();
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
				var fotosImovel = await FotoDAO.GetFotosPorImovel(_idImovelCadastrado);

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
					await fotoDto.InativarFoto(fotoId);
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

					await foto.CadastrarFoto();
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

            int tipoClienteId = TipoClienteDAO.GetIdPorNome("Proprietário");
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
                await cliente.CadastrarCliente();
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

            var tipoClienteId = TipoClienteDAO.GetIdPorNome("Proprietário");
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
                await clienteAtualizado.AtualizarCliente(proprietarioSelecionado.Id);
                MessageBox.Show("Proprietário atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                ProprietarioModalOverlayEditar.Visibility = Visibility.Hidden;
                await AdcionarItensGridProprietarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar proprietário: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
