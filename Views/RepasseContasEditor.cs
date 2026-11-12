using Imob.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Imob.Views
{
    /// <summary>Editor independente para até dois destinatários de repasse.</summary>
    public class RepasseContasEditor : UserControl
    {
        private readonly EditorConta[] editores = { new EditorConta(1), new EditorConta(2) };
        private readonly TextBlock mensagem = new TextBlock { Foreground = Brushes.DarkRed, TextWrapping = TextWrapping.Wrap, Margin = new Thickness(0, 8, 0, 0) };

        public RepasseContasEditor()
        {
            var painel = new StackPanel();
            painel.Children.Add(editores[0].Raiz);
            painel.Children.Add(editores[1].Raiz);
            painel.Children.Add(mensagem);
            Content = painel;
        }

        /// <summary>Substitui o conteúdo do editor. Uma conta nula desativa o respectivo destinatário.</summary>
        public void CarregarContas(ContaRepasse primeira, ContaRepasse segunda)
        {
            editores[0].Carregar(primeira);
            editores[1].Carregar(segunda);
            mensagem.Text = string.Empty;
        }

        /// <summary>Preenche somente campos vazios do destinatário 1 ou 2, sem escolher modalidade, tipo de chave, tipo ou dígito da conta.</summary>
        public void PreencherDadosCliente(int destinatario, ContaRepasse cliente)
        {
            if (destinatario < 1 || destinatario > 2) throw new ArgumentOutOfRangeException(nameof(destinatario));
            if (cliente == null) throw new ArgumentNullException(nameof(cliente));
            editores[destinatario - 1].Preencher(cliente);
        }

        /// <summary>Valida e retorna cópias das contas; destinatários desativados são retornados como null.</summary>
        public bool TryObterContas(out ContaRepasse primeira, out ContaRepasse segunda, out string erro, bool exigirAoMenosUma = false)
        {
            primeira = segunda = null;
            erro = null;
            if (!editores[0].TryObter(out primeira, out erro) || !editores[1].TryObter(out segunda, out erro))
            {
                primeira = segunda = null;
                mensagem.Text = erro;
                return false;
            }
            if (exigirAoMenosUma && primeira == null && segunda == null)
                erro = "Informe ao menos um destinatário.";
            else if ((primeira?.TipoValor == "PERCENTUAL" ? primeira.Valor : 0m) +
                     (segunda?.TipoValor == "PERCENTUAL" ? segunda.Valor : 0m) > 100m)
                erro = "A soma dos percentuais de repasse não pode ultrapassar 100%.";
            mensagem.Text = erro ?? string.Empty;
            if (erro == null) return true;
            primeira = segunda = null;
            return false;
        }

        private sealed class EditorConta
        {
            private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("pt-BR");
            private readonly int numero;
            private readonly CheckBox ativo = new CheckBox { Content = "Incluir este destinatário", Margin = new Thickness(0, 0, 0, 8) };
            private readonly StackPanel campos = new StackPanel();
            private readonly StackPanel pix = new StackPanel();
            private readonly StackPanel banco = new StackPanel();
            private readonly ComboBox modalidade = Combo(("Pix por chave", "PIX_CHAVE"), ("Pix por conta", "PIX_CONTA"), ("TED", "TED"));
            private readonly ComboBox tipoValor = Combo(("Valor fixo (R$)", "FIXO"), ("Percentual (%)", "PERCENTUAL"));
            private readonly ComboBox tipoChave = Combo(("CPF", "CPF"), ("CNPJ", "CNPJ"), ("E-mail", "EMAIL"), ("Telefone", "PHONE"), ("Chave aleatória", "EVP"));
            private readonly ComboBox tipoConta = Combo(("Conta corrente", "CONTA_CORRENTE"), ("Conta poupança", "CONTA_POUPANCA"));
            private readonly TextBox valor = new TextBox();
            private readonly TextBox chave = new TextBox();
            private readonly TextBox codigoBanco = new TextBox();
            private readonly TextBox titular = new TextBox();
            private readonly TextBox documento = new TextBox();
            private readonly TextBox nascimento = new TextBox();
            private readonly TextBox agencia = new TextBox();
            private readonly TextBox conta = new TextBox();
            private readonly TextBox digito = new TextBox();
            public GroupBox Raiz { get; }

            public EditorConta(int numero)
            {
                this.numero = numero;
                var conteudo = new StackPanel { Margin = new Thickness(8) };
                conteudo.Children.Add(ativo);
                conteudo.Children.Add(campos);
                Campo(campos, "Modalidade", modalidade);
                Campo(campos, "Tipo do valor", tipoValor);
                Campo(campos, "Valor (use vírgula decimal)", valor);
                Campo(pix, "Tipo da chave Pix", tipoChave);
                Campo(pix, "Chave Pix", chave);
                campos.Children.Add(pix);
                Campo(banco, "Código do banco", codigoBanco);
                Campo(banco, "Nome do titular", titular);
                Campo(banco, "CPF/CNPJ do titular (somente dígitos)", documento);
                Campo(banco, "Nascimento do titular (opcional, dd/MM/aaaa)", nascimento);
                Campo(banco, "Agência", agencia);
                Campo(banco, "Conta", conta);
                Campo(banco, "Dígito da conta", digito);
                Campo(banco, "Tipo de conta", tipoConta);
                campos.Children.Add(banco);
                Raiz = new GroupBox { Header = $"Conta de destino {numero}", Content = conteudo, Margin = new Thickness(0, 0, 0, 8) };
                ativo.Checked += (_, _) => Atualizar();
                ativo.Unchecked += (_, _) => Atualizar();
                modalidade.SelectionChanged += (_, _) => Atualizar();
                Atualizar();
            }

            private static ComboBox Combo(params (string Texto, string Valor)[] opcoes)
            {
                var combo = new ComboBox { SelectedValuePath = "Tag" };
                foreach (var opcao in opcoes) combo.Items.Add(new ComboBoxItem { Content = opcao.Texto, Tag = opcao.Valor });
                return combo;
            }

            private static void Selecionar(ComboBox combo, string valor)
            {
                combo.SelectedValue = valor;
                if (!string.IsNullOrEmpty(valor) && combo.SelectedIndex < 0)
                {
                    var item = new ComboBoxItem { Content = valor + " (inválido)", Tag = valor };
                    combo.Items.Add(item);
                    combo.SelectedItem = item;
                }
            }

            private static string Selecionado(ComboBox combo) => combo.SelectedValue as string;
            private static void Campo(Panel painel, string legenda, Control controle)
            {
                painel.Children.Add(new Label { Content = legenda, Target = controle, Padding = new Thickness(0), Margin = new Thickness(0, 5, 0, 2) });
                painel.Children.Add(controle);
            }

            private void Atualizar()
            {
                campos.IsEnabled = ativo.IsChecked == true;
                pix.Visibility = Selecionado(modalidade) == "PIX_CHAVE" ? Visibility.Visible : Visibility.Collapsed;
                banco.Visibility = Selecionado(modalidade) == "PIX_CONTA" || Selecionado(modalidade) == "TED" ? Visibility.Visible : Visibility.Collapsed;
            }

            public void Carregar(ContaRepasse modelo)
            {
                ativo.IsChecked = modelo != null;
                Selecionar(modalidade, modelo?.Modalidade);
                Selecionar(tipoValor, modelo?.TipoValor);
                valor.Text = modelo == null ? "" : modelo.Valor.ToString("0.############################", Cultura);
                Selecionar(tipoChave, modelo?.TipoChavePix);
                chave.Text = modelo?.ChavePix ?? "";
                codigoBanco.Text = modelo?.CodigoBanco ?? "";
                titular.Text = modelo?.NomeTitular ?? "";
                documento.Text = modelo?.CpfCnpjTitular ?? "";
                nascimento.Text = modelo?.DataNascimentoTitular?.ToString("dd/MM/yyyy", Cultura) ?? "";
                agencia.Text = modelo?.Agencia ?? "";
                conta.Text = modelo?.Conta ?? "";
                digito.Text = modelo?.DigitoConta ?? "";
                Selecionar(tipoConta, modelo?.TipoConta);
                Atualizar();
            }

            public void Preencher(ContaRepasse cliente)
            {
                ativo.IsChecked = true;
                PreencherVazio(titular, cliente.NomeTitular);
                PreencherVazio(documento, cliente.CpfCnpjTitular);
                PreencherVazio(nascimento, cliente.DataNascimentoTitular?.ToString("dd/MM/yyyy", Cultura));
                PreencherVazio(codigoBanco, cliente.CodigoBanco);
                PreencherVazio(agencia, cliente.Agencia);
                PreencherVazio(conta, cliente.Conta);
                PreencherVazio(chave, cliente.ChavePix);
            }

            private static void PreencherVazio(TextBox caixa, string valor)
            {
                if (string.IsNullOrWhiteSpace(caixa.Text) && !string.IsNullOrWhiteSpace(valor)) caixa.Text = valor;
            }

            public bool TryObter(out ContaRepasse resultado, out string erro)
            {
                resultado = null;
                erro = null;
                if (ativo.IsChecked != true) return true;
                string modalidadeSelecionada = Selecionado(modalidade);
                string tipoSelecionado = Selecionado(tipoValor);
                string falha = null;
                decimal quantia = 0;
                DateTime data;
                DateTime? dataNascimento = null;
                if (modalidadeSelecionada != "PIX_CHAVE" && modalidadeSelecionada != "PIX_CONTA" && modalidadeSelecionada != "TED")
                    falha = "Selecione a modalidade de repasse.";
                else if (tipoSelecionado != "FIXO" && tipoSelecionado != "PERCENTUAL")
                    falha = "Selecione valor fixo ou percentual.";
                else if (!decimal.TryParse(valor.Text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, Cultura, out quantia) ||
                         quantia <= 0 || quantia > 99999999999999.9999m || decimal.Round(quantia, 4) != quantia ||
                         (tipoSelecionado == "FIXO" && decimal.Round(quantia, 2) != quantia) || (tipoSelecionado == "PERCENTUAL" && quantia > 100))
                    falha = "Informe um valor maior que zero. Valores em reais aceitam até 2 casas decimais; percentuais até 4 casas e no máximo 100%.";
                else if (modalidadeSelecionada == "PIX_CHAVE")
                {
                    string tipo = Selecionado(tipoChave);
                    string texto = chave.Text;
                    if (string.IsNullOrWhiteSpace(texto) || !new[] { "CPF", "CNPJ", "EMAIL", "PHONE", "EVP" }.Contains(tipo))
                        falha = "Informe a chave Pix e seu tipo.";
                    else if ((tipo == "CPF" && !Digitos(texto, 11)) || (tipo == "CNPJ" && !Digitos(texto, 14)) ||
                             (tipo == "PHONE" && !Digitos(texto, 11)) ||
                             (tipo == "EMAIL" && (texto.Length > 254 || !new EmailAddressAttribute().IsValid(texto))) ||
                             (tipo == "EVP" && !Guid.TryParseExact(texto, "D", out _)))
                        falha = "Confira o tipo e o formato da chave Pix. CPF, CNPJ e telefone devem conter apenas números; e-mail ou chave aleatória precisam estar em formato válido.";
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(codigoBanco.Text) || string.IsNullOrWhiteSpace(titular.Text) ||
                        string.IsNullOrWhiteSpace(documento.Text) || string.IsNullOrWhiteSpace(agencia.Text) ||
                        string.IsNullOrWhiteSpace(conta.Text) || string.IsNullOrWhiteSpace(digito.Text) ||
                        (Selecionado(tipoConta) != "CONTA_CORRENTE" && Selecionado(tipoConta) != "CONTA_POUPANCA"))
                        falha = "Preencha banco, titular, CPF/CNPJ, agência, conta, dígito e tipo de conta.";
                    else if (!Digitos(documento.Text, 11) && !Digitos(documento.Text, 14))
                        falha = "O CPF/CNPJ do titular deve conter somente 11 ou 14 dígitos.";
                    else if (!string.IsNullOrWhiteSpace(nascimento.Text))
                    {
                        if (DateTime.TryParseExact(nascimento.Text.Trim(), "dd/MM/yyyy", Cultura, DateTimeStyles.None, out data)) dataNascimento = data;
                        else falha = "Data de nascimento inválida; use dd/MM/aaaa ou deixe vazia.";
                    }
                }
                if (falha != null)
                {
                    erro = $"Destinatário {numero}: {falha}";
                    return false;
                }
                resultado = new ContaRepasse
                {
                    Modalidade = modalidadeSelecionada, TipoValor = tipoSelecionado, Valor = quantia,
                    ChavePix = modalidadeSelecionada == "PIX_CHAVE" ? chave.Text : null,
                    TipoChavePix = modalidadeSelecionada == "PIX_CHAVE" ? Selecionado(tipoChave) : null,
                    CodigoBanco = modalidadeSelecionada == "PIX_CHAVE" ? null : codigoBanco.Text,
                    NomeTitular = modalidadeSelecionada == "PIX_CHAVE" ? null : titular.Text,
                    CpfCnpjTitular = modalidadeSelecionada == "PIX_CHAVE" ? null : documento.Text,
                    DataNascimentoTitular = dataNascimento,
                    Agencia = modalidadeSelecionada == "PIX_CHAVE" ? null : agencia.Text,
                    Conta = modalidadeSelecionada == "PIX_CHAVE" ? null : conta.Text,
                    DigitoConta = modalidadeSelecionada == "PIX_CHAVE" ? null : digito.Text,
                    TipoConta = modalidadeSelecionada == "PIX_CHAVE" ? null : Selecionado(tipoConta)
                };
                return true;
            }

            private static bool Digitos(string texto, int tamanho) => texto != null && texto.Length == tamanho && texto.All(c => c >= '0' && c <= '9');
        }
    }
}
