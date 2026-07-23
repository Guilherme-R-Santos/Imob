using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Humanizer;

namespace Imob.Models.Documentos.Contratos
{
    public class ContratoLocacaoPdfModel
    {
        public string Titulo { get; init; } = string.Empty;
        public IReadOnlyList<ContratoPdfBlocoModel> Blocos { get; init; } = Array.Empty<ContratoPdfBlocoModel>();

        public static ContratoLocacaoPdfModel FromContrato(ContratoDAO contrato)
        {
            ArgumentNullException.ThrowIfNull(contrato);

            var blocos = new List<ContratoPdfBlocoModel>();
            var locatarios = ObterLocatarios(contrato).ToList();

            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo($"LOCADOR: {FormatarLocador(contrato.Proprietario)}"));
            blocos.Add(ContratoPdfBlocoModel.CriarEspaco(8));

            blocos.Add(ContratoPdfBlocoModel.CriarTitulo("LOCATÁRIOS:", recuo: 0));

            for (var indice = 0; indice < locatarios.Count; indice++)
            {
                blocos.Add(ContratoPdfBlocoModel.CriarParagrafo($"{indice + 1}. {FormatarLocatario(locatarios[indice])}", recuo: 25));
            }

            blocos.Add(ContratoPdfBlocoModel.CriarEspaco(8));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("As partes acima identificadas têm entre si justo e contratado o presente Contrato de Locação Residencial, que se regerá pelas cláusulas e condições seguintes:"));
            blocos.Add(ContratoPdfBlocoModel.CriarDivisoria());

            blocos.Add(ContratoPdfBlocoModel.CriarTitulo("CLÁUSULA 1 - DO IMÓVEL:"));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("O LOCADOR dá em locação aos LOCATÁRIOS o imóvel situado à:"));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo(FormatarEnderecoImovel(contrato.Imovel)));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo(FormatarFinalidadeImovel(contrato.Imovel?.TipoImovel?.Nome)));
            blocos.Add(ContratoPdfBlocoModel.CriarDivisoria());

            blocos.Add(ContratoPdfBlocoModel.CriarTitulo("CLÁUSULA 2 - DO PRAZO:"));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo($"O prazo da locação é de {contrato.PrazoMeses} ({contrato.PrazoMeses.ToWords(new CultureInfo("pt-BR"))}) meses, iniciando-se em {FormatarData(contrato.DataInicioVigencia)} e terminando em {FormatarData(contrato.DataFimVigencia)}."));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("Ao término do prazo, caso não haja manifestação contrária, a locação prorrogar-se-á por prazo indeterminado."));
            blocos.Add(ContratoPdfBlocoModel.CriarDivisoria());

            blocos.Add(ContratoPdfBlocoModel.CriarTitulo("CLÁUSULA 3 - DO VALOR DO ALUGUEL:"));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo($"O valor mensal do aluguel será de R$ {contrato.ValorContrato:N2} ({FormatarMoedaPorExtenso(contrato.ValorContrato)}), a ser pago até o dia {contrato.Vencimento} de cada mês, sob pena de multa de 10%, acrescida de juros de 1% ao mês e correção monetária."));
            blocos.Add(ContratoPdfBlocoModel.CriarDivisoria());

            blocos.Add(ContratoPdfBlocoModel.CriarTitulo("CLÁUSULA 3-A - DA FORMA DE PAGAMENTO:"));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("O pagamento do aluguel e encargos será realizado por meio de boleto bancário emitido pela plataforma ASAAS."));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("Será acrescido ao valor mensal o custo de emissão do boleto no valor de R$ 3,00 (três reais), a cargo dos LOCATÁRIOS."));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("O não pagamento do boleto até a data de vencimento implicará incidência dos encargos previstos neste contrato."));
            blocos.Add(ContratoPdfBlocoModel.CriarDivisoria());

            blocos.Add(ContratoPdfBlocoModel.CriarTitulo("CLÁUSULA 4 - DOS ENCARGOS:"));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("Ficam a cargo dos LOCATÁRIOS:"));
            blocos.Add(ContratoPdfBlocoModel.CriarLista(MontarEncargos(contrato)));
            blocos.Add(ContratoPdfBlocoModel.CriarDivisoria());

            blocos.Add(ContratoPdfBlocoModel.CriarTitulo("CLÁUSULA 5 - DO REAJUSTE:"));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("O aluguel será reajustado anualmente com base no índice IGP-M ou IPCA, prevalecendo aquele que estiver vigente à época da correção, ou outro índice que venha a substituí-lo."));
            blocos.Add(ContratoPdfBlocoModel.CriarDivisoria());

            blocos.AddRange(MontarClausulaGarantia(contrato));

            blocos.Add(ContratoPdfBlocoModel.CriarTitulo("CLÁUSULA 7 - DAS OBRIGAÇÕES DOS LOCATÁRIOS:"));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("Os LOCATÁRIOS se obrigam a:"));
            blocos.Add(ContratoPdfBlocoModel.CriarLista(new[]
            {
                "Zelar pelo imóvel;",
                "Restituí-lo no mesmo estado em que receberam, devendo entregá-lo com pintura nova na cor branca;",
                "Não realizar modificações sem autorização do LOCADOR;",
                "Permitir vistoria mediante prévio aviso;"
            }));
            blocos.Add(ContratoPdfBlocoModel.CriarDivisoria());

            blocos.Add(ContratoPdfBlocoModel.CriarTitulo("CLÁUSULA 8 - DAS OBRIGAÇÕES DO LOCADOR:"));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("O LOCADOR se obriga a:"));
            blocos.Add(ContratoPdfBlocoModel.CriarLista(new[]
            {
                "Entregar o imóvel em condições de uso;",
                "Garantir o uso pacífico do imóvel;"
            }));
            blocos.Add(ContratoPdfBlocoModel.CriarDivisoria());

            blocos.Add(ContratoPdfBlocoModel.CriarTitulo("CLÁUSULA 9 - DA RESPONSABILIDADE SOLIDÁRIA:"));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("Os LOCATÁRIOS assumem responsabilidade solidária por todas as obrigações decorrentes deste contrato, respondendo conjunta e individualmente pelo pagamento dos aluguéis, encargos, danos ao imóvel e demais obrigações aqui pactuadas."));
            blocos.Add(ContratoPdfBlocoModel.CriarDivisoria());

            blocos.Add(ContratoPdfBlocoModel.CriarTitulo("CLÁUSULA 10 - DO USO E CONVIVÊNCIA:"));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("Os LOCATÁRIOS comprometem-se a respeitar a Lei do Silêncio, abstendo-se de produzir ruídos que perturbem o sossego após as 22h."));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("Comprometem-se ainda a cumprir integralmente as regras do condomínio, bem como a manter comportamento compatível com a boa convivência, respeitando os demais moradores e vizinhos."));
            blocos.Add(ContratoPdfBlocoModel.CriarDivisoria());

            blocos.Add(ContratoPdfBlocoModel.CriarTitulo("CLÁUSULA 11 - DA RESCISÃO:"));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("O contrato poderá ser rescindido:"));
            blocos.Add(ContratoPdfBlocoModel.CriarLista(new[]
            {
                "Por acordo entre as partes;",
                "Por infração contratual;",
                "Pelo LOCATÁRIO, mediante aviso prévio de 30 dias;"
            }));
            blocos.Add(ContratoPdfBlocoModel.CriarDivisoria());

            blocos.Add(ContratoPdfBlocoModel.CriarTitulo("CLÁUSULA 12 - DA MULTA:"));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("Em caso de rescisão antecipada por iniciativa dos LOCATÁRIOS, será devida multa equivalente a 03 (três) meses de aluguel, calculada de forma proporcional ao período restante do contrato."));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("A proporcionalidade será apurada considerando-se o tempo faltante para o término do prazo contratual, nos termos da legislação vigente."));
            blocos.Add(ContratoPdfBlocoModel.CriarDivisoria());

            blocos.Add(ContratoPdfBlocoModel.CriarTitulo("CLÁUSULA 13 - DO FORO:"));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("Fica eleito o foro da comarca do imóvel para dirimir quaisquer controvérsias."));
            blocos.Add(ContratoPdfBlocoModel.CriarDivisoria());

            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("E, por estarem assim justos e contratados, assinam DE FORMA REMOTA, por e-mail, PELO ASSINADOR AUTENTIQUE, o presente instrumento, que será enviado por e-mail a todas as partes."));

            var cidadeAssinatura = TextoOuPadrao(contrato.Imovel?.Cidade, "Rio de Janeiro");
            var dataAtual = DateTime.Now;
            var mesExtenso = dataAtual.ToString("MMMM", new CultureInfo("pt-BR"));
            blocos.Add(ContratoPdfBlocoModel.CriarEspaco(14));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo($"{cidadeAssinatura}, {dataAtual:dd} de {mesExtenso} de {dataAtual:yyyy}", recuo: 150));
            blocos.Add(ContratoPdfBlocoModel.CriarDivisoria());

            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("LOCADOR:______________________________________________________________"));
            for (var indice = 0; indice < locatarios.Count; indice++)
            {
                blocos.Add(ContratoPdfBlocoModel.CriarParagrafo($"LOCATÁRIO {indice + 1}:___________________________________________________________"));
            }
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("TESTEMUNHA 1:___________________________________________________________"));
            blocos.Add(ContratoPdfBlocoModel.CriarParagrafo("TESTEMUNHA 2:___________________________________________________________"));

            return new ContratoLocacaoPdfModel
            {
                Titulo = $"CONTRATO DE {TextoOuPadrao(contrato.TipoContrato?.Nome, "LOCAÇÃO RESIDENCIAL").ToUpperInvariant()}",
                Blocos = new ReadOnlyCollection<ContratoPdfBlocoModel>(blocos)
            };
        }

        public static string FormatarMoedaPorExtenso(decimal valor)
        {
            var cultura = new CultureInfo("pt-BR");
            var valorFormatado = valor.ToString("F2", cultura).Split(',');
            var reais = int.Parse(valorFormatado[0]);
            var centavos = int.Parse(valorFormatado[1]);

            if (centavos > 0)
            {
                return $"{reais.ToWords(cultura)} reais e {centavos.ToWords(cultura)} centavos";
            }

            return $"{reais.ToWords(cultura)} reais";
        }

        private static IEnumerable<string> MontarEncargos(ContratoDAO contrato)
        {
            var encargos = new List<string>
            {
                "Contas de consumo (água, luz, gás);"
            };

            if (contrato.Imovel?.Condominio is > 0)
            {
                encargos.Add($"Taxa de condomínio no valor atual de R$ {contrato.Imovel.Condominio.Value:N2};");
            }

            if (contrato.Imovel?.TaxaIncendio is > 0)
            {
                encargos.Add("Funesbom anual;");
            }

            encargos.Add("IPTU e demais encargos incidentes sobre o imóvel;");

            return encargos;
        }

        private static IEnumerable<ContratoPdfBlocoModel> MontarClausulaGarantia(ContratoDAO contrato)
        {
            var modalidadeId = contrato.ModalidadeContrato?.Id ?? contrato.ModalidadeContratoId ?? 0;

            if (modalidadeId == 3)
            {
                yield return ContratoPdfBlocoModel.CriarTitulo("CLÁUSULA 6 - DA GARANTIA FIDEJUSSÓRIA (FIADOR):");
                yield return ContratoPdfBlocoModel.CriarParagrafo(FormatarCaputFiador(contrato.Fiador));
                yield return ContratoPdfBlocoModel.CriarParagrafo("Parágrafo Primeiro – A responsabilidade do(s) FIADOR(ES) permanecerá válida até a efetiva entrega das chaves, desocupação do imóvel e quitação integral de todos os débitos decorrentes da locação, ainda que haja prorrogação do contrato por prazo indeterminado, renunciando expressamente aos benefícios previstos nos artigos 827, 835, 837 e 838 do Código Civil.");
                yield return ContratoPdfBlocoModel.CriarParagrafo("Parágrafo Segundo – O(s) FIADOR(ES) declara(m) possuir plena capacidade civil e idoneidade financeira para assumir as obrigações decorrentes desta fiança, obrigando-se solidariamente ao cumprimento integral do presente contrato.");
                yield return ContratoPdfBlocoModel.CriarParagrafo("Parágrafo Terceiro – Em caso de falecimento, insolvência, incapacidade civil, exoneração, venda do imóvel do fiador ou qualquer hipótese que comprometa a garantia prestada, o LOCATÁRIO obriga-se a apresentar novo fiador idôneo ou outra garantia aceita pelo LOCADOR no prazo máximo de 15 (quinze) dias, sob pena de infração contratual e possibilidade de rescisão da locação.");
                yield return ContratoPdfBlocoModel.CriarParagrafo("Parágrafo Quarto - O(s) FIADOR(ES) acima qualificado(s) assume(m), de forma irrevogável e irretratável, a condição de responsável(is) solidário(s) com o LOCATÁRIO pelo fiel cumprimento de todas as obrigações decorrentes do presente contrato de locação, incluindo aluguéis, encargos locatícios, tributos, multas, danos ao imóvel, custas processuais, honorários advocatícios e demais obrigações legais e contratuais, permanecendo sua responsabilidade válida e integral até a efetiva entrega das chaves do imóvel ao LOCADOR, mediante quitação de todos os débitos eventualmente existentes.");
                yield return ContratoPdfBlocoModel.CriarParagrafo("Parágrafo Quinto – O(s) FIADOR(ES) renuncia(m) expressamente aos benefícios de ordem, divisão e exoneração previstos nos artigos 827, 835, 837 e 838 do Código Civil, obrigando-se solidariamente ao LOCATÁRIO até o encerramento definitivo da locação e devolução formal das chaves, ainda que o contrato venha a ser prorrogado por prazo indeterminado.");
                yield return ContratoPdfBlocoModel.CriarDivisoria();
                yield break;
            }

            if (modalidadeId == 4)
            {
                yield return ContratoPdfBlocoModel.CriarTitulo("CLÁUSULA 6 - DO SEGURO FIANÇA LOCATÍCIA:");
                yield return ContratoPdfBlocoModel.CriarParagrafo("O LOCATÁRIO obriga-se a contratar e manter vigente, durante toda a duração da locação e eventuais prorrogações, seguro fiança locatícia junto a seguradora regularmente autorizada pela SUSEP, em valor e condições suficientes para garantir o integral cumprimento das obrigações assumidas neste contrato, abrangendo, inclusive, aluguéis, encargos locatícios, multas contratuais, danos ao imóvel, custas processuais e honorários advocatícios.");
                yield return ContratoPdfBlocoModel.CriarParagrafo("Parágrafo Primeiro – A apólice do seguro fiança deverá ser apresentada ao LOCADOR antes da entrega das chaves, bem como renovada sucessivamente enquanto perdurar a locação, sob pena de caracterização de infração contratual.");
                yield return ContratoPdfBlocoModel.CriarParagrafo("Parágrafo Segundo – O não pagamento do prêmio do seguro, o cancelamento da apólice, sua não renovação ou qualquer situação que implique perda ou redução da garantia contratada obrigará o LOCATÁRIO a regularizar a garantia no prazo máximo de 10 (dez) dias, contados da notificação, sob pena de rescisão contratual, independentemente das demais penalidades previstas neste instrumento.");
                yield return ContratoPdfBlocoModel.CriarParagrafo("Parágrafo Terceiro – Todas as despesas decorrentes da contratação, renovação e manutenção do seguro fiança correrão exclusivamente por conta do LOCATÁRIO.");
                yield return ContratoPdfBlocoModel.CriarParagrafo("Parágrafo Quarto – O LOCADOR poderá exigir substituição ou complementação da garantia caso a apólice apresentada não cubra integralmente as obrigações locatícias assumidas neste contrato.");
                yield return ContratoPdfBlocoModel.CriarDivisoria();
                yield break;
            }

            var valorCaucao = contrato.ValorContrato * 3;

            yield return ContratoPdfBlocoModel.CriarTitulo("CLÁUSULA 6 - DO DEPÓSITO CAUÇÃO:");
            yield return ContratoPdfBlocoModel.CriarParagrafo($"Como garantia do fiel cumprimento das obrigações assumidas no presente contrato, o LOCATÁRIO entrega neste ato ao LOCADOR a quantia de R$ {valorCaucao:N2} ({FormatarMoedaPorExtenso(valorCaucao)}), correspondente a 03 (três) meses de aluguel, a título de DEPÓSITO CAUÇÃO, nos termos do artigo 38, §2º, da Lei nº 8.245/91 (Lei do Inquilinato).");
            yield return ContratoPdfBlocoModel.CriarParagrafo("Parágrafo Primeiro – O valor caucionado destina-se a garantir o pagamento de aluguéis, encargos locatícios, multas contratuais, danos ao imóvel, contas de consumo, tributos e quaisquer outras obrigações decorrentes da locação.");
            yield return ContratoPdfBlocoModel.CriarParagrafo("Parágrafo Segundo – O depósito caução deverá ser mantido em caderneta de poupança, em instituição financeira oficial, revertendo-se ao LOCATÁRIO, ao final da locação, os valores atualizados monetariamente, desde que o imóvel seja devolvido nas mesmas condições em que foi recebido e inexistam débitos pendentes.");
            yield return ContratoPdfBlocoModel.CriarParagrafo("Parágrafo Terceiro – Havendo débitos, danos ao imóvel ou quaisquer valores pendentes de responsabilidade do LOCATÁRIO, o LOCADOR poderá utilizar total ou parcialmente o valor caucionado para compensação das obrigações inadimplidas, devendo eventual saldo remanescente ser devolvido ao LOCATÁRIO após a efetiva entrega das chaves.");
            yield return ContratoPdfBlocoModel.CriarParagrafo("Parágrafo Quarto – Caso o valor do depósito caução seja insuficiente para quitação integral das obrigações pendentes, permanecerá o LOCATÁRIO responsável pelo pagamento da diferença eventualmente apurada.");
            yield return ContratoPdfBlocoModel.CriarDivisoria();
        }

        private static string FormatarCaputFiador(ClienteDAO? fiador)
        {
            return $"Como garantia do fiel cumprimento de todas as obrigações assumidas neste contrato, o LOCATÁRIO apresenta como FIADOR(ES) o(s) Sr.(a)(s) {TextoOuPadrao(fiador?.Nome, "_____________")}, nacionalidade {TextoOuPadrao(fiador?.Nacionalidade, "_______")}, estado civil {TextoOuPadrao(fiador?.EstadoCivil, "_______")}, profissão {TextoOuPadrao(fiador?.Profissao, "_______")}, portador(a) do RG nº {TextoOuPadrao(fiador?.Identidade, "_______")} e CPF nº {TextoOuPadrao(fiador?.CpfCnpj, "_______")}, residente(s) e domiciliado(s) à {TextoOuPadrao(fiador?.Endereco, "_________________")}, que assina(m) o presente instrumento na qualidade de principal(is) pagador(es) e solidariamente responsável(is) com o LOCATÁRIO por todas as obrigações decorrentes desta locação, inclusive aluguéis, encargos locatícios, multas, tributos, danos ao imóvel, honorários advocatícios, custas processuais e demais encargos contratuais e legais.";
        }

        private static string FormatarLocador(ClienteDAO? locador)
        {
            if (locador is null)
            {
                return "NÃO INFORMADO.";
            }

            return $"{TextoOuPadrao(locador.Nome, "NÃO INFORMADO").ToUpperInvariant()}, {TextoOuPadrao(locador.Nacionalidade, "não informado")}, {TextoOuPadrao(locador.EstadoCivil, "não informado")}, {TextoOuPadrao(locador.Profissao, "não informado")}, portador do CPF nº {TextoOuPadrao(locador.CpfCnpj, "não informado")}, residente nesta cidade. E-MAIL: {TextoOuPadrao(locador.Email, "não informado")}.";
        }

        private static string FormatarLocatario(ClienteDAO locatario)
        {
            var sb = new StringBuilder();
            sb.Append($"{TextoOuPadrao(locatario.Nome, "NÃO INFORMADO").ToUpperInvariant()}, ");
            sb.Append($"{TextoOuPadrao(locatario.Nacionalidade, "não informado")}, ");
            sb.Append($"{TextoOuPadrao(locatario.EstadoCivil, "não informado")}, ");
            sb.Append($"{TextoOuPadrao(locatario.Profissao, "não informado")}");

            if (locatario.DataNascimento.HasValue)
            {
                sb.Append($", nascido em {locatario.DataNascimento.Value:dd/MM/yyyy}");
            }

            sb.Append($", portador da identidade nº {TextoOuPadrao(locatario.Identidade, "não informada")} e CPF nº {TextoOuPadrao(locatario.CpfCnpj, "não informado")}");

            if (!string.IsNullOrWhiteSpace(locatario.Endereco))
            {
                sb.Append($", residente à {locatario.Endereco}");
            }

            sb.Append($", e-mail: {TextoOuPadrao(locatario.Email, "não informado")}." );
            return sb.ToString();
        }

        private static string FormatarEnderecoImovel(ImovelDAO? imovel)
        {
            if (imovel is null)
            {
                return "ENDEREÇO DO IMÓVEL NÃO INFORMADO.";
            }

            var complemento = string.IsNullOrWhiteSpace(imovel.Complemento) ? string.Empty : $", {imovel.Complemento}";
            return $"{TextoOuPadrao(imovel.Logradouro, "Logradouro não informado")}, nº {imovel.Numero}{complemento}, {TextoOuPadrao(imovel.Bairro, "Bairro não informado")}, {TextoOuPadrao(imovel.Cidade, "Cidade não informada")}/{TextoOuPadrao(imovel.Estado, "UF")}, CEP {TextoOuPadrao(imovel.Cep, "não informado")}.";
        }

        private static string FormatarFinalidadeImovel(string? tipoImovel)
        {
            return tipoImovel?.Trim().ToLowerInvariant() switch
            {
                "comercial" => "O imóvel destina-se exclusivamente para fins comerciais.",
                "misto" => "O imóvel destina-se para fins residenciais e comerciais.",
                _ => "O imóvel destina-se exclusivamente para fins residenciais."
            };
        }

        private static string FormatarData(DateTime? data)
        {
            return data?.ToString("dd/MM/yyyy") ?? "data não informada";
        }

        private static string TextoOuPadrao(string? valor, string padrao)
        {
            return string.IsNullOrWhiteSpace(valor) ? padrao : valor.Trim();
        }

        private static IEnumerable<ClienteDAO> ObterLocatarios(ContratoDAO contrato)
        {
            if (contrato.Contratante1 is not null)
            {
                yield return contrato.Contratante1;
            }

            if (contrato.Contratante2 is not null)
            {
                yield return contrato.Contratante2;
            }

            if (contrato.Contratante3 is not null)
            {
                yield return contrato.Contratante3;
            }

            if (contrato.Contratante4 is not null)
            {
                yield return contrato.Contratante4;
            }
        }
    }

    public enum ContratoPdfBlocoTipo
    {
        Titulo,
        Paragrafo,
        Lista,
        Divisoria,
        Espaco
    }

    public class ContratoPdfBlocoModel
    {
        public ContratoPdfBlocoTipo Tipo { get; init; }
        public string Texto { get; init; } = string.Empty;
        public IReadOnlyList<string> Itens { get; init; } = Array.Empty<string>();
        public double Espaco { get; init; }
        public int Recuo { get; init; }
        public bool Negrito { get; init; }
        public bool Centralizado { get; init; }
        public bool Sublinhado { get; init; }

        public static ContratoPdfBlocoModel CriarTitulo(string texto, bool principal = false, bool sublinhado = false, int recuo = 0)
        {
            return new ContratoPdfBlocoModel
            {
                Tipo = ContratoPdfBlocoTipo.Titulo,
                Texto = texto,
                Negrito = true,
                Centralizado = principal,
                Sublinhado = sublinhado,
                Recuo = recuo
            };
        }

        public static ContratoPdfBlocoModel CriarParagrafo(string texto, bool negrito = false, int recuo = 0)
        {
            return new ContratoPdfBlocoModel
            {
                Tipo = ContratoPdfBlocoTipo.Paragrafo,
                Texto = texto,
                Negrito = negrito,
                Recuo = recuo
            };
        }

        public static ContratoPdfBlocoModel CriarLista(IEnumerable<string> itens, int recuo = 0)
        {
            return new ContratoPdfBlocoModel
            {
                Tipo = ContratoPdfBlocoTipo.Lista,
                Itens = new ReadOnlyCollection<string>(itens.Where(item => !string.IsNullOrWhiteSpace(item)).ToList()),
                Recuo = recuo
            };
        }

        public static ContratoPdfBlocoModel CriarDivisoria()
        {
            return new ContratoPdfBlocoModel
            {
                Tipo = ContratoPdfBlocoTipo.Divisoria
            };
        }

        public static ContratoPdfBlocoModel CriarEspaco(double espaco)
        {
            return new ContratoPdfBlocoModel
            {
                Tipo = ContratoPdfBlocoTipo.Espaco,
                Espaco = espaco
            };
        }
    }
}
