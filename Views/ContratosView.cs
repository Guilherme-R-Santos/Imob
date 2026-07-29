using Imob.Models;
using Imob.Services.Pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace Imob
{
	public partial class Sistema
	{
        public async Task AdicionarItensGridContratos()
        {
            try
            {
                await _viewModel.CarregarContratosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar contratos: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

private ComboBox? ObterComboContratoFiador2Visualizar()
        {
            return FindName("ComboContratoFiador2Visualizar") as ComboBox;
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

private async void BtnAdicionarContrato_Click(object sender, RoutedEventArgs e)
        {
            await CarregarCombosContratoCriarAsync();
            _viewModel.ContratoModalCriarVisibility = Visibility.Visible;
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

                ComboContratoFiador2Criar.DisplayMemberPath = "Nome";
                ComboContratoFiador2Criar.SelectedValuePath = "Id";
                ComboContratoFiador2Criar.ItemsSource = fiadoresTask.Result;

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

                var comboContratoFiador2Visualizar = ObterComboContratoFiador2Visualizar();
                if (comboContratoFiador2Visualizar != null)
                {
                    comboContratoFiador2Visualizar.DisplayMemberPath = "Nome";
                    comboContratoFiador2Visualizar.SelectedValuePath = "Id";
                    comboContratoFiador2Visualizar.ItemsSource = fiadoresTask.Result;
                }

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
            _viewModel.ContratoModalCriarVisibility = Visibility.Hidden;

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
            ComboContratoFiador2Criar.SelectedItem = null;

            AtualizarPermissoesModalidadeCriar();
            AtualizarFiltroProprietarioImovelCriar(true);
            AtualizarFiltroContratantesCriar();
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
            _viewModel.ContratoModalVisualizarVisibility = Visibility.Hidden;
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
            ClienteDAO fiador2Selecionado = ComboContratoFiador2Criar.SelectedItem as ClienteDAO;
            DateTime? inicio = DpContratoDataInicioCriar.SelectedDate;
            string proposta = TxtContratoPropostaSegFiancaCriar.Text;
            string apolice = TxtContratoApoliceSegFiancaCriar.Text;
            var valorContratoTexto = TxtContratoValorCriar.Text;


            if (!int.TryParse(TxtContratoPrazoMesesCriar.Text, out int prazo))
            {
                MessageBox.Show("Prazo inválido Utilize apenas números.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(valorContratoTexto, NumberStyles.Number, CultureInfo.CurrentCulture, out var valorContrato))
            {
                MessageBox.Show("Valor do contrato inválido. Utilize apenas números.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                Fiador2 = fiador2Selecionado,
                ValorContrato = valorContrato,
                DataInicioVigencia = inicio.Value,
                PrazoMeses = prazo,
                Vencimento = vencimento,
                PropostaSegFianca = proposta,
                ApoliceSegFianca = apolice

            };

            await contrato.CadastrarContrato(HttpClientFixo);

            MessageBox.Show("Contrato cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

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
            ComboContratoFiador2Criar.SelectedItem = null;
            DpContratoDataInicioCriar.SelectedDate = null;
            TxtContratoPrazoMesesCriar.Clear();
            TxtContratoValorCriar.Clear();
            DpContratoVencimentoCriar.Clear();
            TxtContratoPropostaSegFiancaCriar.Clear();
            TxtContratoApoliceSegFiancaCriar.Clear();

            AtualizarPermissoesModalidadeCriar();
            AtualizarFiltroProprietarioImovelCriar(true);
            AtualizarFiltroContratantesCriar();

            _viewModel.ContratoModalCriarVisibility = Visibility.Hidden;

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
            int? fiador2Id = ObterComboContratoFiador2Visualizar() is ComboBox comboContratoFiador2Visualizar
                ? ObterIdSelecionado(comboContratoFiador2Visualizar)
                : null;
            DateTime? inicio = DpContratoDataInicioVisualizar.SelectedDate;
            string proposta = TxtContratoPropostaSegFiancaVisualizar.Text;
            string apolice = TxtContratoApoliceSegFiancaVisualizar.Text;
            var valorContratoTexto = TxtContratoValorVisualizar.Text;

            if (!ModalidadeEhFiador(modalidadeSelecionada))
            {
                fiadorId = null;
                fiador2Id = null;
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

            if (!decimal.TryParse(valorContratoTexto, NumberStyles.Number, CultureInfo.CurrentCulture, out var valorContrato))
            {
                MessageBox.Show("Valor do contrato inválido. Utilize apenas números.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    Fiador2 = fiador2Id.HasValue ? new ClienteDAO { Id = fiador2Id.Value } : null,
                    ValorContrato = valorContrato,
                    DataInicioVigencia = inicio.Value,
                    PrazoMeses = prazo,
                    Vencimento = vencimento,
                    PropostaSegFianca = proposta,
                    ApoliceSegFianca = apolice
                };

                await contratoAtualizado.AtualizarContrato(contratoSelecionado.Id, HttpClientFixo);

                MessageBox.Show("Contrato atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                _viewModel.ContratoModalVisualizarVisibility = Visibility.Hidden;

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
                var comboContratoFiador2Visualizar = ObterComboContratoFiador2Visualizar();
                if (comboContratoFiador2Visualizar != null)
                {
                    comboContratoFiador2Visualizar.SelectedValue = contratoSelecionado.Fiador2?.Id ?? contratoSelecionado.Fiador2Id;
                }

                if (ComboTipoContratoVisualizar.SelectedItem == null) ComboTipoContratoVisualizar.Text = contratoSelecionado.TipoContrato?.Nome;
                if (ComboModalidadeContratoVisualizar.SelectedItem == null) ComboModalidadeContratoVisualizar.Text = contratoSelecionado.ModalidadeContrato?.Nome;
                if (ComboObjetoContratoVisualizar.SelectedItem == null) ComboObjetoContratoVisualizar.Text = contratoSelecionado.ObjetoContrato?.Nome;
                if (ComboContratoProprietarioVisualizar.SelectedItem == null) ComboContratoProprietarioVisualizar.Text = contratoSelecionado.Proprietario?.Nome;
                if (ComboContratoImovelVisualizar.SelectedItem == null) ComboContratoImovelVisualizar.Text = contratoSelecionado.Imovel?.Logradouro;
                if (ComboContratoContratante1Visualizar.SelectedItem == null) ComboContratoContratante1Visualizar.Text = contratoSelecionado.Contratante1?.Nome;
                if (ComboContratoContratante2Visualizar.SelectedItem == null) ComboContratoContratante2Visualizar.Text = contratoSelecionado.Contratante2?.Nome;
                if (ComboContratoContratante3Visualizar.SelectedItem == null) ComboContratoContratante3Visualizar.Text = contratoSelecionado.Contratante3?.Nome;
                if (ComboContratoContratante4Visualizar.SelectedItem == null) ComboContratoContratante4Visualizar.Text = contratoSelecionado.Contratante4?.Nome;
                if (ComboContratoFiadorVisualizar.SelectedItem == null) ComboContratoFiadorVisualizar.Text = contratoSelecionado.Fiador?.Nome;
                if (comboContratoFiador2Visualizar != null && comboContratoFiador2Visualizar.SelectedItem == null) comboContratoFiador2Visualizar.Text = contratoSelecionado.Fiador2?.Nome;

                DpContratoDataInicioVisualizar.SelectedDate = contratoSelecionado.DataInicioVigencia;
                TxtContratoPrazoMesesVisualizar.Text = contratoSelecionado.PrazoMeses.ToString();
                TxtContratoValorVisualizar.Text = contratoSelecionado.ValorContrato.ToString(CultureInfo.CurrentCulture);
                TxtContratoVencimentoVisualizar.Text = contratoSelecionado.Vencimento.ToString();
                TxtContratoPropostaSegFiancaVisualizar.Text = contratoSelecionado.PropostaSegFianca;
                TxtContratoApoliceSegFiancaVisualizar.Text = contratoSelecionado.ApoliceSegFianca;

                AtualizarFiltroProprietarioImovelVisualizar(true);
                AtualizarFiltroContratantesVisualizar();
                AtualizarPermissoesModalidadeVisualizar();

                _viewModel.ContratoModalVisualizarVisibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar contrato: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

private void BtnGerarContratoVisualizar_Click(object sender, RoutedEventArgs e)
        {
            GeradorContratoPdf gerador = new GeradorContratoPdf();
            ContratoDAO contratoSelecionado = ContratosDataGrid.SelectedItem as ContratoDAO;

            try
            {
                MessageBox.Show("Em andamento", "Aguarde", MessageBoxButton.OK, MessageBoxImage.Information);
                gerador.CriarContratoLocacao(contratoSelecionado);

            } catch (Exception ex)
            {
                MessageBox.Show("Erro ao gerar contrato: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
	}
}
