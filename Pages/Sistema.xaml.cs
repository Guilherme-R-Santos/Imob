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

        // Funções auxilires Fim

        public Sistema()
        {

            InitializeComponent();
            WindowState = WindowState.Maximized;

            // Bind preview collection to ItemsControl
            FotosSelecionadasList.ItemsSource = _fotosSelecionadasPreview;

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

        private void ProprietariosTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            ProprietariosPanel.Visibility = Visibility.Visible;
        }

        private void LocatariosTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            LocatariosPanel.Visibility = Visibility.Visible;
        }

        private void FiadoresTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
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
            ImoveilModalOverlayCriar.Visibility = Visibility.Visible;
        }

        private void BtnFecharModalImovel_Click(object sender, RoutedEventArgs e)
        {
            ImoveilModalOverlayCriar.Visibility = Visibility.Hidden;
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

            ImoveilModalOverlayCriar.Visibility = Visibility.Hidden;
            await AdicionarItensGridImoveis();

            ImoveilFotosModalOverlayCriar.Visibility = Visibility.Visible;

        }

        private async void BtnAtualizar_Click(object sender, RoutedEventArgs e)
        {
            await AdicionarItensGridImoveis();  
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
            ImoveilModalOverlayEditar.Visibility = Visibility.Visible;

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
            imovelAtualizado.Intencao = IntencaoDAO.GetIdPorNome(ComboIntencaoEditar.SelectedItem as string);
            imovelAtualizado.TipoImovel = TipoImovelDAO.GetIdPorNome(ComboTipoImovelEditar.SelectedItem as string);
            imovelAtualizado.Proprietario = ClienteDAO.GetIdPorNome(ComboProprietariosEditar.SelectedItem as string);


            try
            {
                await imovelAtualizado.AtualizarImovel(imovelSelecionado.Id);
                MessageBox.Show("Imóvel atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                ImoveilModalOverlayEditar.Visibility = Visibility.Hidden;
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
            ImoveilModalOverlayEditar.Visibility = Visibility.Hidden;
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

        private void BtnLimparSelecao_Click(object sender, RoutedEventArgs e)
        {
            _fotosSelecionadasPreview.Clear();
            _fotosSelecionadasBinario.Clear();
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
                _fotosSelecionadasPreview.Remove(img);
                int index = FotosSelecionadasList.Items.IndexOf(img);
                if (index >= 0 && index < _fotosSelecionadasBinario.Count)
                {
                    _fotosSelecionadasBinario.RemoveAt(index);
                }
            }
        }

        private void BtnFecharModalImovelFotoCriar_Click(object sender, RoutedEventArgs e)
        {
            ImoveilFotosModalOverlayCriar.Visibility = Visibility.Hidden;
            _fotosSelecionadasPreview.Clear();
            _fotosSelecionadasBinario.Clear();
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
                ImoveilFotosModalOverlayCriar.Visibility = Visibility.Hidden;
                _fotosSelecionadasPreview.Clear();
                _fotosSelecionadasBinario.Clear();
                _idImovelCadastrado = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cadastrar fotos: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void BtnEditarFotos_Click(object sender, RoutedEventArgs e)
        {
            ImoveilModalOverlayEditar.Visibility = Visibility.Hidden;
            //TODO: Implementar aqui

        }
    }
}
