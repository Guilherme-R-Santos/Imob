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

            AdicionaParagrafo(gfx, $"LOCADOR: {contrato.Proprietario.Nome.ToUpper()}, {contrato.Proprietario.Nacionalidade}, {contrato.Proprietario.EstadoCivil}, {contrato.Proprietario.Profissao}, portador do CPF nº {contrato.Proprietario.CpfCnpj}, residente nesta cidade. E-MAIL: {contrato.Proprietario.Email}.", fontCorpo, linhaX, linhaY, page);

            linhaY += 20;

            AdicionarLinhaParagrafo(gfx, "LOCATÁRIOS:", fontTitulo, linhaX, linhaY, page);

            linhaY += 20;
            linhaX += 25;

            AdicionaParagrafo(gfx, $"1º LOCATÁRIO: {contrato.Contratante1.Nome.ToUpper()}, {contrato.Contratante1.Nacionalidade}, {contrato.Contratante1.EstadoCivil}, {contrato.Contratante1.Profissao}, Nascido em {contrato.Contratante1.DataNascimento?.ToString("dd/MM/yyyy")}, portador da identidade nº {contrato.Contratante1.Identidade} e CPF nº {contrato.Contratante1.CpfCnpj},", fontCorpo, linhaX, linhaY, page);

            if (contrato.Contratante1.Endereco != null)
            {
                AdicionaParagrafo(gfx, $"residente à {contrato.Contratante1.Endereco},", fontCorpo, linhaX, linhaY, page);
            }

            AdicionaParagrafo(gfx, $"e-mail: {contrato.Contratante1.Email}.", fontCorpo, linhaX, linhaY, page);

            if (contrato.Contratante2 != null)
            {
                linhaY += 5;
                AdicionaParagrafo(gfx, $"2º LOCATÁRIO: {contrato.Contratante2.Nome.ToUpper()}, {contrato.Contratante2.Nacionalidade}, {contrato.Contratante2.EstadoCivil}, {contrato.Contratante2.Profissao}, Nascido em {contrato.Contratante2.DataNascimento?.ToString("dd/MM/yyyy")}, portador da identidade nº {contrato.Contratante2.Identidade} e CPF nº {contrato.Contratante2.CpfCnpj},", fontCorpo, linhaX, linhaY, page);

                if (contrato.Contratante2.Endereco != null)
                {
                    AdicionaParagrafo(gfx, $"residente à {contrato.Contratante2.Endereco},", fontCorpo, linhaX, linhaY, page);
                }

                AdicionaParagrafo(gfx, $"e-mail: {contrato.Contratante2.Email}.", fontCorpo, linhaX, linhaY, page);

            }

            if (contrato.Contratante3 != null)
            {
                linhaY += 5;
                AdicionaParagrafo(gfx, $"3º LOCATÁRIO: {contrato.Contratante3.Nome.ToUpper()}, {contrato.Contratante3.Nacionalidade}, {contrato.Contratante3.EstadoCivil}, {contrato.Contratante3.Profissao}, Nascido em {contrato.Contratante3.DataNascimento?.ToString("dd/MM/yyyy")}, portador da identidade nº {contrato.Contratante3.Identidade} e CPF nº {contrato.Contratante3.CpfCnpj},", fontCorpo, linhaX, linhaY, page);

                if (contrato.Contratante3.Endereco != null)
                {
                    AdicionaParagrafo(gfx, $"residente à {contrato.Contratante3.Endereco},", fontCorpo, linhaX, linhaY, page);
                }

                AdicionaParagrafo(gfx, $"e-mail: {contrato.Contratante3.Email}.", fontCorpo, linhaX, linhaY, page);
            }

            if (contrato.Contratante4 != null)
            {
                linhaY += 5;
                AdicionaParagrafo(gfx, $"4º LOCATÁRIO: {contrato.Contratante4.Nome.ToUpper()}, {contrato.Contratante4.Nacionalidade}, {contrato.Contratante4.EstadoCivil}, {contrato.Contratante4.Profissao}, Nascido em {contrato.Contratante4.DataNascimento?.ToString("dd/MM/yyyy")}, portador da identidade nº {contrato.Contratante4.Identidade} e CPF nº {contrato.Contratante4.CpfCnpj},", fontCorpo, linhaX, linhaY, page);

                if (contrato.Contratante4.Endereco != null)
                {
                    AdicionaParagrafo(gfx, $"residente à {contrato.Contratante4.Endereco},", fontCorpo, linhaX, linhaY, page);
                }

                AdicionaParagrafo(gfx, $"e-mail: {contrato.Contratante4.Email}.", fontCorpo, linhaX, linhaY, page);
            }

            linhaY += 20;
            linhaX -= 25;

            AdicionaParagrafo(gfx, "As partes acima identificadas têm entre si justo e contratado o presente contrato de Locação Residencial, que se regerá pelas cláusulas e condições seguintes:", fontCorpo, linhaX, linhaY, page);

            AdicionarLinhaDivisoria(gfx, page);

            AdicionaParagrafo(gfx, "CLÁUSULA 1 - DO IMÓVEL:", fontTitulo, linhaX, linhaY, page);

            linhaY += 20;

            AdicionaParagrafo(gfx, "O LOCADOR dá em locação ao(s) LOCATÁRIO(S) o imóvel situado à:", fontCorpo, linhaX, linhaY, page);

            linhaY += 20;

            AdicionaParagrafo(gfx, $"{contrato.Imovel.Logradouro}, {contrato.Imovel.Numero} {(!string.IsNullOrEmpty(contrato.Imovel.Complemento) ? ", " + contrato.Imovel.Complemento : "")}, {contrato.Imovel.Bairro}, {contrato.Imovel.Cidade} / {contrato.Imovel.Estado}, CEP {contrato.Imovel.Cep}.", fontCorpo, linhaX, linhaY, page);

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

            AdicionaParagrafo(gfx, $"O prazo da locação é de {contrato.PrazoMeses} meses, iniciando-se em {contrato.DataInicioVigencia?.ToString("dd/MM/yyyy")} e terminando em {contrato.DataFimVigencia?.ToString("dd/MM/yyyy")}.", fontCorpo, linhaX, linhaY, page);

            linhaY += 20;

            AdicionaParagrafo(gfx, "Ao término do prazo, caso não haja manifestação contrária, a locação prorrogar-se-á por prazo indeterminado. ", fontCorpo, linhaX, linhaY, page);

            AdicionarLinhaDivisoria(gfx, page);

            AdicionaParagrafo(gfx, "CLÁUSULA 3 - DO VALOR DO ALUGUEL:", fontTitulo, linhaX, linhaY, page);

            linhaY += 20;

            int valorAluguelReais = int.Parse(contrato.Imovel.ValorLocacao.ToString("F2").Split(',')[0]);

            int valorAluguelCentavos = int.Parse(contrato.Imovel.ValorLocacao.ToString("F2").Split(',')[1]);

            if (valorAluguelCentavos > 0)
            {
                AdicionaParagrafo(gfx, $"O valor mensal do aluguel será de R$ {contrato.Imovel.ValorLocacao.ToString("N2")} ({valorAluguelReais.ToWords(new CultureInfo("pt-BR"))} reais e {valorAluguelCentavos.ToWords(new CultureInfo("pt-BR"))} centavos), a ser pago até o dia {contrato.Vencimento} de cada mês, sob pena de multa de 10% acrescida de juros de 1% ao mês e correção monetária.", fontCorpo, linhaX, linhaY, page);
            } else
            {
                AdicionaParagrafo(gfx, $"O valor mensal do aluguel será de R$ {contrato.Imovel.ValorLocacao.ToString("N2")} ({valorAluguelReais.ToWords(new CultureInfo("pt-BR"))} reais), a ser pago até o dia {contrato.Vencimento} de cada mês, sob pena de multa de 10% acrescida de juros de 1% ao mês e correção monetária.", fontCorpo, linhaX, linhaY, page);
            }

            var page2 = document.AddPage();
            width = page2.Width.Point;
            height = page2.Height.Point;
            page2.Size = PageSize.A4;
            page2.Orientation = PageOrientation.Portrait;
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
            
            if (contrato.Imovel.Condominio > 0)
            {
                AdicionaParagrafo(gfx, $"● Taxa de condomínio no valor atual de R$ {contrato.Imovel.Condominio.ToString("N2")}", fontCorpo, linhaX, linhaY, page2);
            }

            if (contrato.Imovel.TaxaIncendio > 0)
            {

                AdicionaParagrafo(gfx, "● Funesbom anual;", fontCorpo, linhaX, linhaY, page2);

            }

            AdicionaParagrafo(gfx, "● IPTU e demais encargos incidentes sobre o imóvel;", fontCorpo, linhaX, linhaY, page2);

            AdicionarLinhaDivisoria(gfx, page2);

            AdicionaParagrafo(gfx, "CLÁUSULA 5 - DO REAJUSTE:", fontTitulo, linhaX, linhaY, page2);

            linhaY += 20;

            AdicionaParagrafo(gfx, "O aluguel será reajustado anualmente com base no índice IGP-M ou IPCA, prevalecendo aquele que estiver vigente à época da correção, ou outro índice que venha a substituí-lo.", fontCorpo, linhaX, linhaY, page2);

            AdicionarLinhaDivisoria(gfx, page2);

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
