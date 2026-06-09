using System;
using System.Collections.Generic;
using Imob.Models;
using System.Text;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using PdfSharp.Quality;
using PdfSharp.Snippets.Font;
using System.Windows.Input.Manipulations;
using Humanizer;
using System.Globalization;

// TODO: FIX contrato fiador para adequação de páginas. Provavelmente reescrever todo o contrato dentro do if de fiador

namespace Imob.Services.Pdf
{
    public class GeradorContratoPdf
    {
        public int linhaX = 50;
        public int linhaY = 110;
        PdfDocument document = new PdfDocument();

        public void CriarContratoLocacao(ContratoDAO contrato)
        {
            document.Info.Title = "Teste";
            document.Info.Subject = "Teste pdf";
            var page = document.AddPage();
            var width = page.Width.Point;
            var height = page.Height.Point;
            page.Size = PageSize.A4;
            page.Orientation = PageOrientation.Portrait;
            var gfx = XGraphics.FromPdfPage(page);
            var fontCorpo = new XFont("Times New Roman", 12, XFontStyleEx.Regular);
            var fontTitulo = new XFont("Times New Roman", 14, XFontStyleEx.Bold);
            var fontTituloPrincipal = new XFont("Times New Roman", 20, XFontStyleEx.Bold);

            var tituloContrato = $"CONTRATO DE {contrato.TipoContrato.Nome.ToUpper()}";
            gfx.DrawString(tituloContrato, fontTituloPrincipal, XBrushes.Black, new XRect(0, 50, page.Width, page.Height), XStringFormats.TopCenter);
            var tituloSize = gfx.MeasureString(tituloContrato, fontTituloPrincipal);
            var linhaInicioX = (page.Width.Point - tituloSize.Width) / 2;
            var linhaFimX = linhaInicioX + tituloSize.Width;
            gfx.DrawLine(XPens.Black, linhaInicioX, 80, linhaFimX, 80);

            AdicionaParagrafo(gfx, $"LOCADOR: {contrato.Proprietario.Nome.ToUpper()}, {contrato.Proprietario.Nacionalidade}, {contrato.Proprietario.EstadoCivil}, " +
                $"{contrato.Proprietario.Profissao}, portador do CPF nº {contrato.Proprietario.CpfCnpj}, residente nesta cidade. E-MAIL: {contrato.Proprietario.Email}.", 
                fontCorpo, linhaX, linhaY, page);

            linhaY += 20;

            AdicionarLinhaParagrafo(gfx, "LOCATÁRIOS:", fontTitulo, linhaX, linhaY, page);

            linhaY += 20;
            linhaX += 25;

            AdicionaParagrafo(gfx, $"1º LOCATÁRIO: {contrato.Contratante1.Nome.ToUpper()}, {contrato.Contratante1.Nacionalidade}, " +
                $"{contrato.Contratante1.EstadoCivil}, {contrato.Contratante1.Profissao}, Nascido em {contrato.Contratante1.DataNascimento?.ToString("dd/MM/yyyy")}, " +
                $"portador da identidade nº {contrato.Contratante1.Identidade} e CPF nº {contrato.Contratante1.CpfCnpj},", fontCorpo, linhaX, linhaY, page);

            if (contrato.Contratante1.Endereco != null)
            {
                AdicionaParagrafo(gfx, $"residente à {contrato.Contratante1.Endereco},", fontCorpo, linhaX, linhaY, page);
            }

            AdicionaParagrafo(gfx, $"e-mail: {contrato.Contratante1.Email}.", fontCorpo, linhaX, linhaY, page);

            if (contrato.Contratante2 != null)
            {
                linhaY += 5;
                AdicionaParagrafo(gfx, $"2º LOCATÁRIO: {contrato.Contratante2.Nome.ToUpper()}, {contrato.Contratante2.Nacionalidade}, " +
                    $"{contrato.Contratante2.EstadoCivil}, {contrato.Contratante2.Profissao}, Nascido em {contrato.Contratante2.DataNascimento?.ToString("dd/MM/yyyy")}, " +
                    $"portador da identidade nº {contrato.Contratante2.Identidade} e CPF nº {contrato.Contratante2.CpfCnpj},", fontCorpo, linhaX, linhaY, page);

                if (contrato.Contratante2.Endereco != null)
                {
                    AdicionaParagrafo(gfx, $"residente à {contrato.Contratante2.Endereco},", fontCorpo, linhaX, linhaY, page);
                }

                AdicionaParagrafo(gfx, $"e-mail: {contrato.Contratante2.Email}.", fontCorpo, linhaX, linhaY, page);

            }

            if (contrato.Contratante3 != null)
            {
                linhaY += 5;
                AdicionaParagrafo(gfx, $"3º LOCATÁRIO: {contrato.Contratante3.Nome.ToUpper()}, {contrato.Contratante3.Nacionalidade}, " +
                    $"{contrato.Contratante3.EstadoCivil}, {contrato.Contratante3.Profissao}, Nascido em {contrato.Contratante3.DataNascimento?.ToString("dd/MM/yyyy")}, " +
                    $"portador da identidade nº {contrato.Contratante3.Identidade} e CPF nº {contrato.Contratante3.CpfCnpj},", fontCorpo, linhaX, linhaY, page);

                if (contrato.Contratante3.Endereco != null)
                {
                    AdicionaParagrafo(gfx, $"residente à {contrato.Contratante3.Endereco},", fontCorpo, linhaX, linhaY, page);
                }

                AdicionaParagrafo(gfx, $"e-mail: {contrato.Contratante3.Email}.", fontCorpo, linhaX, linhaY, page);
            }

            if (contrato.Contratante4 != null)
            {
                linhaY += 5;
                AdicionaParagrafo(gfx, $"4º LOCATÁRIO: {contrato.Contratante4.Nome.ToUpper()}, {contrato.Contratante4.Nacionalidade}, " +
                    $"{contrato.Contratante4.EstadoCivil}, {contrato.Contratante4.Profissao}, Nascido em {contrato.Contratante4.DataNascimento?.ToString("dd/MM/yyyy")}, " +
                    $"portador da identidade nº {contrato.Contratante4.Identidade} e CPF nº {contrato.Contratante4.CpfCnpj},", fontCorpo, linhaX, linhaY, page);

                if (contrato.Contratante4.Endereco != null)
                {
                    AdicionaParagrafo(gfx, $"residente à {contrato.Contratante4.Endereco},", fontCorpo, linhaX, linhaY, page);
                }

                AdicionaParagrafo(gfx, $"e-mail: {contrato.Contratante4.Email}.", fontCorpo, linhaX, linhaY, page);
            }

            linhaY += 20;
            linhaX -= 25;

            AdicionaParagrafo(gfx, "As partes acima identificadas têm entre si justo e contratado o presente contrato de Locação Residencial, " +
                "que se regerá pelas cláusulas e condições seguintes:", fontCorpo, linhaX, linhaY, page);

            AdicionarLinhaDivisoria(gfx, page);

            AdicionaParagrafo(gfx, "CLÁUSULA 1 - DO IMÓVEL:", fontTitulo, linhaX, linhaY, page);

            linhaY += 20;

            AdicionaParagrafo(gfx, "O LOCADOR dá em locação ao(s) LOCATÁRIO(S) o imóvel situado à:", fontCorpo, linhaX, linhaY, page);

            linhaY += 20;

            AdicionaParagrafo(gfx, $"{contrato.Imovel.Logradouro}, {contrato.Imovel.Numero} {(!string.IsNullOrEmpty(contrato.Imovel.Complemento) ? ", " +
                "" + contrato.Imovel.Complemento : "")}, {contrato.Imovel.Bairro}, {contrato.Imovel.Cidade} / {contrato.Imovel.Estado}, " +
                $"CEP {contrato.Imovel.Cep}.", fontCorpo, linhaX, linhaY, page);

            if (contrato.Imovel.TipoImovel.Nome != null)
            {
                if (contrato.Imovel.TipoImovel.Nome.Equals("Residencial"))
                {
                    AdicionaParagrafo(gfx, $"O imóvel destina-se exclusivamente para fins residenciais.", fontCorpo, linhaX, linhaY, page);
                }

                if (contrato.Imovel.TipoImovel.Nome.Equals("Comercial"))
                {
                    AdicionaParagrafo(gfx, $"O imóvel destina-se exclusivamente para fins comerciais.", fontCorpo, linhaX, linhaY, page);
                }

                if (contrato.Imovel.TipoImovel.Nome.Equals("Misto"))
                {
                    AdicionaParagrafo(gfx, $"O imóvel destina-se para fins residenciais e comerciais.", fontCorpo, linhaX, linhaY, page);
                }
            }

            AdicionarLinhaDivisoria(gfx, page);

            AdicionaParagrafo(gfx, "CLÁUSULA 2 - DO PRAZO:", fontTitulo, linhaX, linhaY, page);

            linhaY += 20;

            AdicionaParagrafo(gfx, $"O prazo da locação é de {contrato.PrazoMeses} meses, iniciando-se em {contrato.DataInicioVigencia?.ToString("dd/MM/yyyy")} e " +
                $"terminando em {contrato.DataFimVigencia?.ToString("dd/MM/yyyy")}.", fontCorpo, linhaX, linhaY, page);

            linhaY += 20;

            AdicionaParagrafo(gfx, "Ao término do prazo, caso não haja manifestação contrária, a locação prorrogar-se-á por prazo indeterminado. ", fontCorpo, linhaX, linhaY, page);

            AdicionarLinhaDivisoria(gfx, page);

            AdicionaParagrafo(gfx, "CLÁUSULA 3 - DO VALOR DO ALUGUEL:", fontTitulo, linhaX, linhaY, page);

            linhaY += 20;

            int valorAluguelReais = int.Parse(contrato.ValorContrato.ToString("F2").Split(',')[0]);

            int valorAluguelCentavos = int.Parse(contrato.ValorContrato.ToString("F2").Split(',')[1]);

            if (valorAluguelCentavos > 0)
            {
                AdicionaParagrafo(gfx, $"O valor mensal do aluguel será de R$ {contrato.ValorContrato.ToString("N2")} ({valorAluguelReais.ToWords(new CultureInfo("pt-BR"))} reais " +
                    $"e {valorAluguelCentavos.ToWords(new CultureInfo("pt-BR"))} centavos), a ser pago até o dia {contrato.Vencimento} de cada mês, sob pena de multa de 10% acrescida de juros" +
                    $" de 1% ao mês e correção monetária.", fontCorpo, linhaX, linhaY, page);
            } else
            {
                AdicionaParagrafo(gfx, $"O valor mensal do aluguel será de R$ {contrato.ValorContrato.ToString("N2")} ({valorAluguelReais.ToWords(new CultureInfo("pt-BR"))} reais), " +
                    $"a ser pago até o dia {contrato.Vencimento} de cada mês, sob pena de multa de 10% acrescida de juros de 1% ao mês e correção monetária.", fontCorpo, linhaX, linhaY, page);
            }

            var page2 = document.AddPage();
            width = page2.Width.Point;
            height = page2.Height.Point;
            page2.Size = PageSize.A4;
            page2.Orientation = PageOrientation.Portrait;
            gfx.Dispose();
            gfx = XGraphics.FromPdfPage(page2);
            linhaY = 70;

            AdicionaParagrafo(gfx, "CLÁUSULA 3-A - DA FORMA DE PAGAMENTO:", fontTitulo, linhaX, linhaY, page2);

            linhaY += 20;

            AdicionaParagrafo(gfx, "O pagamento do aluguel e encargos será realizado por meio de boleto bancário emitido pela plataforma ASAAS.", fontCorpo, linhaX, linhaY, page2);

            linhaY += 20;

            AdicionaParagrafo(gfx, "Será acrescido ao valor mensal o custo de emissão do boleto no valor de R$ 3,00 (três reais), a cargo dos LOCATÁRIOS.", fontCorpo, linhaX, linhaY, page2);

            linhaY += 20;

            AdicionaParagrafo(gfx, "O não pagamento do boleto até a data de vencimento implicará incidência dos encargos previstos neste contrato.", fontCorpo, linhaX, linhaY, page2);

            AdicionarLinhaDivisoria(gfx, page2);

            AdicionaParagrafo(gfx, "CLÁUSULA 4 - DOS ENCARGOS", fontTitulo, linhaX, linhaY, page2);

            linhaY += 20;

            AdicionaParagrafo(gfx, "Ficam a cargo dos LOCATÁRIOS:", fontCorpo, linhaX, linhaY, page2);

            linhaY += 20;

            AdicionaParagrafo(gfx, "● Contas de consumo (água, luz, gás);", fontCorpo, linhaX, linhaY, page2);
            
            if (contrato.Imovel.Condominio.HasValue && contrato.Imovel.Condominio > 0)
            {
                AdicionaParagrafo(gfx, $"● Taxa de condomínio no valor atual de R$ {contrato.Imovel.Condominio.Value.ToString("N2")}", fontCorpo, linhaX, linhaY, page2);
            }

            if (contrato.Imovel.TaxaIncendio.HasValue && contrato.Imovel.TaxaIncendio > 0)
            {

                AdicionaParagrafo(gfx, "● Funesbom anual;", fontCorpo, linhaX, linhaY, page2);

            }

            AdicionaParagrafo(gfx, "● IPTU e demais encargos incidentes sobre o imóvel;", fontCorpo, linhaX, linhaY, page2);

            AdicionarLinhaDivisoria(gfx, page2);

            AdicionaParagrafo(gfx, "CLÁUSULA 5 - DO REAJUSTE:", fontTitulo, linhaX, linhaY, page2);

            linhaY += 20;

            AdicionaParagrafo(gfx, "O aluguel será reajustado anualmente com base no índice IGP-M ou IPCA, prevalecendo aquele que estiver vigente à época da correção, ou " +
                "outro índice que venha a substituí-lo.", fontCorpo, linhaX, linhaY, page2);

            AdicionarLinhaDivisoria(gfx, page2);

            var page3 = document.AddPage();
            width = page3.Width.Point;
            height = page3.Height.Point;
            page3.Size = PageSize.A4;
            page3.Orientation = PageOrientation.Portrait;
            gfx.Dispose();
            gfx = XGraphics.FromPdfPage(page3);
            linhaY = 70;

            if (contrato.ModalidadeContrato.Id == 1 || contrato.ModalidadeContratoId == 2) // Caução
            {
                AdicionaParagrafo(gfx, "CLÁUSULA 6 - DA GARANTIA:", fontTitulo, linhaX, linhaY, page2);

                linhaY += 20;

                decimal valorCaucao = contrato.ValorContrato * 3;

                int valorCaucaoReais = int.Parse(valorCaucao.ToString("F2").Split(',')[0]);

                int valorCaucaoCentavos = int.Parse(valorCaucao.ToString("F2").Split(',')[1]);

                string caucaoExtenso = "";

                if (valorCaucaoCentavos > 0)
                {
                    caucaoExtenso = $"{valorCaucaoReais.ToWords(new CultureInfo("pt-BR"))} reais e {valorCaucaoCentavos.ToWords(new CultureInfo("pt-BR"))} centavos";
                } else
                {
                    caucaoExtenso = $"{valorCaucaoReais.ToWords(new CultureInfo("pt-BR"))} reais";
                }
            
                AdicionaParagrafo(gfx, $"Como garantia da locação, os LOCATARIOS oferecem caução no valor equivalente a 03 (três) meses de aluguel, " +
                    $"totalizando R$ {valorCaucao.ToString("N2")} ({caucaoExtenso}), a ser pago, através de depósito bancário na conta do LOCADOR, banco: " +
                    $"({contrato.Proprietario.CodBanco}) {contrato.Proprietario.Banco}, AG: {contrato.Proprietario.Agencia}; Conta corrente: {contrato.Proprietario.Conta}, " +
                    $"(CHAVE PIX: {contrato.Proprietario.ChavePix}) nome e CPF do LOCADOR.", fontCorpo, linhaX, linhaY, page2);

                linhaY += 20;

                AdicionaParagrafo(gfx, "PARÁGRADO ÚNICO: A devolução da garantia ocorrerá da seguinte forma: Os valores correspondentes a 02 (dois) meses serão utilizados para quitação do dois" +
                    "últimos meses da locação, e o valor correspondente a 01 (um) mês será devolvido ao final do contrato, desde que não haja débitos ou danos ao imóvel.", fontCorpo, linhaX, linhaY, page2);

                AdicionarLinhaDivisoria(gfx, page2);

            } else if (contrato.ModalidadeContrato.Id == 3) // Fiador
            {
                linhaY = 70;

                AdicionaParagrafo(gfx, "CLÁUSULA 6 - DA GARANTIA FIDEJUSSÓRIA (FIADOR):", fontTitulo, linhaX, linhaY, page3);
                linhaY += 20;
                AdicionaParagrafo(gfx, $"Como garantia do fiel cumprimento de todas as obrigações assumidas neste contrato, o LOCATÁRIO apresenta como FIADOR(ES) o(s) Sr.(a)(s) {contrato.Fiador.Nome}, " +
                    $"nacionalidade {contrato.Fiador.Nacionalidade}, estado civil {contrato.Fiador.EstadoCivil}, profissão {contrato.Fiador.Profissao}, portador(a) do RG nº {contrato.Fiador.Identidade} " +
                    $"e CPF nº {contrato.Fiador.CpfCnpj}, residente(s) e domiciliado(s) à {contrato.Fiador.Endereco}, que assina(m)" +
                    $" o presente instrumento na qualidade de principal(is) pagador(es) e solidariamente responsável(is) com o LOCATÁRIO por todas as obrigações decorrentes desta locação, inclusive" +
                    $" alugueis, encargos locatícios, multas, tributos, danos ao imóvel, honorários advocatícios, custas processuais e demais encargos contratuais e legais. Parágrafo Primeiro" +
                    $" – A responsabilidade do(s) FIADOR(ES) permanecerá válida até a efetiva entrega das chaves, desocupação do imóvel e quitação integral de todos os débitos decorrentes da locação," +
                    $" ainda que haja prorrogação do contrato por prazo indeterminado, renunciando expressamente aos benefícios previstos nos artigos 827, 835, 837 e 838 do Código Civil. Parágrafo" +
                    $" Segundo – O(s) FIADOR(ES) declara(m) possuir plena capacidade civil e idoneidade financeira para assumir as obrigações decorrentes desta fiança, obrigando-se solidariamente ao" +
                    $" cumprimento integral do presente contrato. Parágrafo Terceiro – Em caso de falecimento, insolvência, incapacidade civil, exoneração, venda do imóvel do fiador ou qualquer hipótese" +
                    $" que comprometa a garantia prestada, o LOCATÁRIO obriga-se a apresentar novo fiador idôneo ou outra garantia aceita pelo LOCADOR no prazo máximo de 15 (quinze) dias, sob pena de infração" +
                    $" contratual e possibilidade de rescisão da locação. Parágrafo Quarto - O(s) FIADOR(ES) acima qualificado(s) assume(m), de forma irrevogável e irretratável, a condição de responsável(is)" +
                    $" solidário(s) com o LOCATÁRIO pelo fiel cumprimento de todas as obrigações decorrentes do presente contrato de locação, incluindo alugueis, encargos locatícios, tributos, multas," +
                    $" danos ao imóvel, custas processuais, honorários advocatícios e demais obrigações legais e contratuais, permanecendo sua responsabilidade válida e integral até a efetiva entrega das chaves" +
                    $" do imóvel ao LOCADOR, mediante quitação de todos os débitos eventualmente existentes. Parágrafo quinto – O(s) FIADOR(ES) renuncia(m) expressamente aos benefícios de ordem, divisão e exoneração" +
                    $" previstos nos artigos 827, 835, 837 e 838 do Código Civil, obrigando-se solidariamente ao LOCATÁRIO até o encerramento definitivo da locação e devolução formal das chaves, ainda que o contrato venha" +
                    $" a ser prorrogado por prazo indeterminado.",
                    fontCorpo, linhaX, linhaY, page3);

            } else if (contrato.ModalidadeContrato.Id == 4)
            {
                AdicionaParagrafo(gfx, "CLÁUSULA 6 - DO SEGURO FIANÇA LOCATÍCIA:", fontTitulo, linhaX, linhaY, page2);
                linhaY += 20;
                AdicionaParagrafo(gfx, $"O LOCATÁRIO obriga-se a contratar e manter vigente, durante toda a duração da locação e eventuais prorrogações, seguro fiança locatícia junto a seguradora regularmente autorizada pela SUSEP" +
                    $", em valor e condições suficientes para garantir o integral cumprimento das obrigações assumidas neste contrato, abrangendo, inclusive, alugueis, encargos locatícios, multas contratuais, danos ao imóvel," +
                    $" custas processuais e honorários advocatícios.\r\n\r\nParágrafo Primeiro – A apólice do seguro fiança deverá ser apresentada ao LOCADOR antes da entrega das chaves, bem como renovada sucessivamente enquanto" +
                    $" perdurar a locação, sob pena de caracterização de infração contratual.\r\n\r\nParágrafo Segundo – O não pagamento do prêmio do seguro, o cancelamento da apólice, sua não renovação ou qualquer situação que" +
                    $" implique perda ou redução da garantia contratada obrigará o LOCATÁRIO a regularizar a garantia no prazo máximo de 10 (dez) dias, contados da notificação, sob pena de rescisão contratual, independentemente" +
                    $" das demais penalidades previstas neste instrumento.\r\n\r\nParágrafo Terceiro – Todas as despesas decorrentes da contratação, renovação e manutenção do seguro fiança correrão exclusivamente por conta do" +
                    $" LOCATÁRIO.\r\n\r\nParágrafo Quarto – O LOCADOR poderá exigir substituição ou complementação da garantia caso a apólice apresentada não cubra integralmente as obrigações locatícias assumidas neste contrato.",
                    fontCorpo, linhaX, linhaY, page2);

                AdicionarLinhaDivisoria(gfx, page2);
            }

            if (contrato.ModalidadeContrato.Id != 3)
            {

                linhaY = 70;


                AdicionaParagrafo(gfx, "CLÁUSULA 7 - DAS OBRIGAÇÕES DO(s) LOCATÁRIO(s):", fontTitulo, linhaX, linhaY, page3);

                linhaY += 20;

                AdicionaParagrafo(gfx, "O(s) LOCATARIO(S) se obriga(m) a:", fontCorpo, linhaX, linhaY, page3);

                linhaY += 20;

                AdicionaParagrafo(gfx, "●   Zelar pelo imóvel;", fontCorpo, linhaX, linhaY, page3);
                AdicionaParagrafo(gfx, "●   Restituí-lo no mesmo estado em que receberam, devendo entregá-lo com pintura nova na cor branca;", fontCorpo, linhaX, linhaY, page3);
                AdicionaParagrafo(gfx, "●   Não realizar modificações sem autorização do LOCADOR;", fontCorpo, linhaX, linhaY, page3);
                AdicionaParagrafo(gfx, "●   Permitir vistoria mediante prévio aviso;", fontCorpo, linhaX, linhaY, page3);

                AdicionarLinhaDivisoria(gfx, page3);

                AdicionaParagrafo(gfx, "CLÁUSULA 8 - DAS OBRIGAÇÕES DO LOCADOR:", fontTitulo, linhaX, linhaY, page3);

                linhaY += 20;

                AdicionaParagrafo(gfx, "O LOCADOR se obriga a:", fontCorpo, linhaX, linhaY, page3);

                linhaY += 20;

                AdicionaParagrafo(gfx, "●   Entregar o imóvel em condições de uso;", fontCorpo, linhaX, linhaY, page3);
                AdicionaParagrafo(gfx, "●   Garantir o uso pacífico do imóvel;", fontCorpo, linhaX, linhaY, page3);

                AdicionarLinhaDivisoria(gfx, page3);

                AdicionaParagrafo(gfx, "CLÁUSULA 9 - DA RESPONSABILIDADE SOLIDÁRIA:", fontTitulo, linhaX, linhaY, page3);

                linhaY += 20;

                AdicionaParagrafo(gfx, "Os LOCATÁRIOS assumem responsabilidade solidária por todas as obrigações decorrentes deste contrato, respondendo conjunta e individualmente pelo pagamento dos " +
                    "aluguéis, encargos, danos ao imóvel e demais obrigações aqui pactuadas.", fontCorpo, linhaX, linhaY, page3);

                AdicionarLinhaDivisoria(gfx, page3);

                AdicionaParagrafo(gfx, "CLÁUSULA 10 - DO USO E CONVIVÊNCIA:", fontTitulo, linhaX, linhaY, page3);

                linhaY += 20;

                AdicionaParagrafo(gfx, "Os LOCATÁRIOS comprometem-se a respeitar a Lei do Silêncio, abstendo-se de produzir ruídos que perturbem o sossego após as 22h.", fontCorpo, linhaX, linhaY, page3);

                linhaY += 20;

                AdicionaParagrafo(gfx, "Compromentem-se ainda a cumprir integralmente as regras do condomínio, caso o mesmo esteja localizado em um, bem como manter comportamento compatível com a boa convivência, " +
                    "respeitando os demais moradores e vizinhos.", fontCorpo, linhaX, linhaY, page3);

                AdicionarLinhaDivisoria(gfx, page3);

                AdicionaParagrafo(gfx, "CLÁUSULA 11 - DA RECISÃO:", fontTitulo, linhaX, linhaY, page3);

                linhaY += 20;

                AdicionaParagrafo(gfx, "O contrato poderá ser rescindido:", fontCorpo, linhaX, linhaY, page3);

                linhaY += 20;

                AdicionaParagrafo(gfx, "●   Por acordo entre as partes", fontCorpo, linhaX, linhaY, page3);

                AdicionaParagrafo(gfx, "●   Por infração contratual;", fontCorpo, linhaX, linhaY, page3);

                AdicionaParagrafo(gfx, "●   Pelo LOCATÁRIO, mediante aviso prévio de 30 dias;", fontCorpo, linhaX, linhaY, page3);

                var page4 = document.AddPage();
                width = page4.Width.Point;
                height = page4.Height.Point;
                page4.Size = PageSize.A4;
                page4.Orientation = PageOrientation.Portrait;
                gfx.Dispose();
                gfx = XGraphics.FromPdfPage(page4);
                linhaY = 70;

                AdicionarLinhaDivisoria(gfx, page4);

                AdicionaParagrafo(gfx, "CLÁUSULA 12 - DA MULTA:", fontTitulo, linhaX, linhaY, page4);

                linhaY += 20;

                AdicionaParagrafo(gfx, "Em caso de rescisão antecipada por iniciativa dos LOCATÁRIOS, será devida multa equivalente a 03 (três) meses de aluguel, calculada" +
                    " de forma proporcional ao período restante do contrato.", fontCorpo, linhaX, linhaY, page4);

                linhaY += 20;

                AdicionaParagrafo(gfx, "A proporcionalidade será apurada considerando-se o tempo faltante para o término do prazo \r\ncontratual, nos termos da legislação vigente.", fontCorpo, linhaX, linhaY, page4);

                AdicionarLinhaDivisoria(gfx, page4);

                AdicionaParagrafo(gfx, "CLÁUSULA 13 - DO FORO:", fontTitulo, linhaX, linhaY, page4);

                AdicionaParagrafo(gfx, "Fica eleito o foro da comarca do imóvel para dirimir quaisquer controvérsias.", fontCorpo, linhaX, linhaY, page4);

                AdicionarLinhaDivisoria(gfx, page4);

                AdicionaParagrafo(gfx, "E, por estarem assim justos e contratados, assinam DE FORMA REMOTA, por e-mail, PELO ASSINADOR AUTENTIQUE, o presente" +
                    " instrumento, que será enviado por e-mail à todas as partes.", fontCorpo, linhaX, linhaY, page4);

                linhaY += 40;
                linhaX = 200;

                string dataHoje = DateTime.Now.ToString("dd/MM/yyyy");

                string dia = dataHoje.Substring(0, 2);
                string mesExtenso = DateTime.Now.ToString("MMMM", new CultureInfo("pt-BR"));
                string ano = dataHoje.Substring(6, 4);

                AdicionaParagrafo(gfx, $"Rio de Janeiro, {dia} de {mesExtenso} de {ano}", fontCorpo, linhaX, linhaY, page4);

                linhaX = 50;

                AdicionarLinhaDivisoria(gfx, page4);

                linhaY += 40;

                AdicionaParagrafo(gfx, "LOCADOR:_______________________________________________________", fontCorpo, linhaX, linhaY, page4);

                linhaY += 15;

                AdicionaParagrafo(gfx, "LOCATÁRIO 1:____________________________________________________", fontCorpo, linhaX, linhaY, page4);

                linhaY += 15;

                if (contrato.Contratante2 != null)
                {
                    AdicionaParagrafo(gfx, "LOCATÁRIO 2:________________________________________________", fontCorpo, linhaX, linhaY, page4);

                    linhaY += 15;
                }

                if (contrato.Contratante3 != null)
                {
                    AdicionaParagrafo(gfx, "LOCATÁRIO 3:________________________________________________", fontCorpo, linhaX, linhaY, page4);

                    linhaY += 15;
                }

                if (contrato.Contratante4 != null)
                {
                    AdicionaParagrafo(gfx, "LOCATÁRIO 4:________________________________________________", fontCorpo, linhaX, linhaY, page4);

                    linhaY += 15;
                }

                AdicionaParagrafo(gfx, "TESTEMUNHA 1:__________________________________________________", fontCorpo, linhaX, linhaY, page4);

                linhaY += 15;

                AdicionaParagrafo(gfx, "TESTEMUNHA 2:__________________________________________________", fontCorpo, linhaX, linhaY, page4);

                linhaY += 15;

            }


            var filename = IOUtility.GetTempFullFileName("Contrato", "pdf");
            PdfFileUtility.SaveAndShowDocument(document, filename);
        }

        public void AdicionarLinhaParagrafo(XGraphics gfx, string texto, XFont fonte, int x, int y, PdfPage page)
        {
            var rect = new XRect(x, y, page.Width - x - 40, page.Height - y);
            gfx.DrawString(texto, fonte, XBrushes.Black, rect, XStringFormats.TopLeft);
            linhaY += 16;
        }

        public void AdicionaParagrafo(XGraphics gfx, string texto, XFont fonte, int x, int y, PdfPage page)
        {
            if (linhaY != y)
            {
                linhaY = y;
            }

            var larguraMaxima = page.Width.Point - x - 50;
            var palavras = texto.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var linhaAtual = new StringBuilder();

            foreach (var palavra in palavras)
            {
                var textoTeste = linhaAtual.Length == 0 ? palavra : $"{linhaAtual} {palavra}";

                if (gfx.MeasureString(textoTeste, fonte).Width <= larguraMaxima)
                {
                    linhaAtual.Clear();
                    linhaAtual.Append(textoTeste);
                    continue;
                }

                if (linhaAtual.Length > 0)
                {
                    var rectLinha = new XRect(x, linhaY, larguraMaxima, page.Height - linhaY);
                    gfx.DrawString(linhaAtual.ToString(), fonte, XBrushes.Black, rectLinha, XStringFormats.TopLeft);
                    linhaY += 16;
                }

                linhaAtual.Clear();
                linhaAtual.Append(palavra);
            }

            if (linhaAtual.Length > 0)
            {
                var rectLinha = new XRect(x, linhaY, larguraMaxima, page.Height - linhaY);
                gfx.DrawString(linhaAtual.ToString(), fonte, XBrushes.Black, rectLinha, XStringFormats.TopLeft);
                linhaY += 16;
            }
        }

        public void AdicionarLinhaDivisoria(XGraphics gfx, PdfPage page)
        {
            linhaY += 20;
            gfx.DrawLine(XPens.Black, 50, linhaY + 4, page.Width.Point - 50, linhaY + 4);
            linhaY += 20;
        }
    }
}
