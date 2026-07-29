using Imob.Models;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Imob
{
    public partial class Sistema
    {
        public async Task AdicionarItensGridImoveis()
        {
            try
            {
                await _viewModel.CarregarImoveisAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar imóveis: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CadastrarImovelBanco(object sender, RoutedEventArgs e)
        {
            var clienteSelecionado = ComboProprietarios.SelectedItem as string;
            var intencaoSelecionada = ComboIntencao.SelectedItem as string;
            var tipoImovelSelecionado = ComboTipoImovel.SelectedItem as string;
            var finalidadeSelecionada = ComboFinalidade.SelectedItem as string;

            if (string.IsNullOrWhiteSpace(clienteSelecionado) ||
                string.IsNullOrWhiteSpace(intencaoSelecionada) ||
                string.IsNullOrWhiteSpace(tipoImovelSelecionado) ||
                string.IsNullOrWhiteSpace(finalidadeSelecionada) ||
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

            if (string.IsNullOrWhiteSpace(TxtBoxValorLocacao.Text) && string.IsNullOrWhiteSpace(TxtBoxValorVenda.Text))
            {
                MessageBox.Show("Por favor, preencha pelo menos um dos campos de valor: Valor de Venda ou Valor de Locação.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(TxtBoxMetragem.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var metragem))
            {
                MessageBox.Show("Metragem inválida.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(TxtBoxNumero.Text, out var numero))
            {
                MessageBox.Show("Número inválido.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!TryParseDecimal(TxtBoxValorVenda.Text, out var valorVenda) ||
                !TryParseDecimal(TxtBoxValorLocacao.Text, out var valorLocacao) ||
                !TryParseDecimal(TxtBoxCondominio.Text, out var condominio) ||
                !TryParseDecimal(TxtBoxIptu.Text, out var iptu) ||
                !TryParseDecimal(TxtBoxTaxaIncendio.Text, out var taxaIncendio) ||
                !TryParseDecimal(TxtBoxForo.Text, out var foro))
            {
                MessageBox.Show("Um ou mais valores numéricos estão inválidos.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var imovel = new ImovelDTO
            {
                Proprietario = ClienteDAO.GetIdPorNome(clienteSelecionado, HttpClientFixo),
                TipoImovel = TipoImovelDAO.GetIdPorNome(tipoImovelSelecionado, HttpClientFixo),
                Intencao = IntencaoDAO.GetIdPorNome(intencaoSelecionada, HttpClientFixo),
                Finalidade = Imob.Models.DAOs.FinalidadeDAO.GetIdPorNome(finalidadeSelecionada, HttpClientFixo),
                Cep = TxtBoxCep.Text,
                Logradouro = TxtBoxLogradouro.Text,
                Numero = numero,
                Bairro = TxtBoxBairro.Text,
                Cidade = TxtBoxCidade.Text,
                Estado = TxtBoxEstado.Text,
                Pais = TxtBoxPais.Text,
                Complemento = TxtBoxComplemento.Text,
                Metragem = metragem,
                ValorVenda = valorVenda,
                ValorLocacao = valorLocacao,
                Condominio = condominio,
                Iptu = iptu,
                TaxaIncendio = taxaIncendio,
                Foro = foro,
                Observacao = TxtBoxObservacoes.Text,
                Descricao = TxtBoxDescricao.Text,
                InscricaoIptu = TxtBoxInscricaoIptu.Text,
                NumeroCbmerj = TxtBoxNumeroCbmerj.Text,
                Cadastrador = UsuarioLogado.Id
            };

            try
            {
                var idImovel = await imovel.CadastrarImovel(HttpClientFixo);
                _idImovelCadastrado = idImovel;
                _viewModel.DefinirImovelCadastrado(idImovel);

                MessageBox.Show("Imóvel cadastrado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                _viewModel.ImovelModalCriarVisibility = Visibility.Hidden;
                await AdicionarItensGridImoveis();
                _viewModel.ImovelFotosModalCriarVisibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao cadastrar imóvel: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnEditarModalImovel_Click(object sender, RoutedEventArgs e)
        {
            if (ImoveisDataGrid.SelectedItem is not ImovelDAO imovelSelecionado)
            {
                MessageBox.Show("Selecione um imóvel para editar.", "Atenção", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!decimal.TryParse(TxtBoxMetragemEditar.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var metragem) ||
                !decimal.TryParse(TxtBoxValorVendaEditar.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var valorVenda) ||
                !decimal.TryParse(TxtBoxValorLocacaoEditar.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var valorLocacao) ||
                !decimal.TryParse(TxtBoxCondominioEditar.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var condominio) ||
                !decimal.TryParse(TxtBoxIptuEditar.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var iptu) ||
                !decimal.TryParse(TxtBoxTaxaIncendioEditar.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var taxaIncendio) ||
                !decimal.TryParse(TxtBoxForoEditar.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out var foro) ||
                !int.TryParse(TxtBoxNumeroEditar.Text, out var numero))
            {
                MessageBox.Show("Existem valores inválidos no formulário.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var imovelAtualizado = new ImovelDTO
            {
                TaxaIncendio = taxaIncendio,
                Foro = foro,
                Iptu = iptu,
                ValorVenda = valorVenda,
                ValorLocacao = valorLocacao,
                Metragem = metragem,
                Descricao = TxtBoxDescricaoEditar.Text,
                Observacao = TxtBoxObservacoesEditar.Text,
                Condominio = condominio,
                Complemento = TxtBoxComplementoEditar.Text,
                Bairro = TxtBoxBairroEditar.Text,
                Cidade = TxtBoxCidadeEditar.Text,
                Estado = TxtBoxEstadoEditar.Text,
                Pais = TxtBoxPaisEditar.Text,
                Numero = numero,
                Logradouro = TxtBoxLogradouroEditar.Text,
                Cep = TxtBoxCepEditar.Text,
                InscricaoIptu = TxtBoxInscricaoIptuEditar.Text,
                NumeroCbmerj = TxtBoxNumeroCbmerjEditar.Text,
                Intencao = IntencaoDAO.GetIdPorNome(ComboIntencaoEditar.SelectedItem as string, HttpClientFixo),
                TipoImovel = TipoImovelDAO.GetIdPorNome(ComboTipoImovelEditar.SelectedItem as string, HttpClientFixo),
                Finalidade = Imob.Models.DAOs.FinalidadeDAO.GetIdPorNome(ComboFinalidadeEditar.SelectedItem as string, HttpClientFixo),
                Proprietario = ClienteDAO.GetIdPorNome(ComboProprietariosEditar.SelectedItem as string, HttpClientFixo)
            };

            try
            {
                await imovelAtualizado.AtualizarImovel(imovelSelecionado.Id, HttpClientFixo);
                MessageBox.Show("Imóvel atualizado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                _viewModel.ImovelModalEditarVisibility = Visibility.Hidden;
                await AdicionarItensGridImoveis();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar imóvel: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
    }
}
