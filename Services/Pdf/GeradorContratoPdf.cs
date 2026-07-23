using System;
using System.Collections.Generic;
using Imob.Models;
using Imob.Models.Documentos.Contratos;
using System.Text;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Quality;
using PdfSharp.Snippets.Font;
using System.Globalization;

namespace Imob.Services.Pdf
{
    public class GeradorContratoPdf
    {
        private const double MargemEsquerda = 50;
        private const double MargemDireita = 50;
        private const double MargemSuperior = 70;
        private const double MargemInferior = 95;
        private const double AlturaLinha = 16;
        private const double EspacoBloco = 6;
        private const double AlturaLinhaRodape = 11;
        private const string RodapeLinha1 = "Estrada dos Bandeirantes, 470, sala 719, Jacarepagua, Rio de Janeiro";
        private const string RodapeLinha2 = "e-mail:forumimoveisrj.com.br - Tel/whatsapp: (21) 98224-6367";

        private static readonly XFont FonteCorpo = new XFont("Times New Roman", 12, XFontStyleEx.Regular);
        private static readonly XFont FonteTitulo = new XFont("Times New Roman", 14, XFontStyleEx.Bold);
        private static readonly XFont FonteTituloPrincipal = new XFont("Times New Roman", 20, XFontStyleEx.Bold);
        private static readonly XFont FonteRodape = new XFont("Times New Roman", 9, XFontStyleEx.Regular);
        private static readonly XFont FonteMarca = new XFont("Times New Roman", 8, XFontStyleEx.BoldItalic);

        public void CriarContratoLocacao(ContratoDAO contrato)
        {
            ArgumentNullException.ThrowIfNull(contrato);

            var model = ContratoLocacaoPdfModel.FromContrato(contrato);
            var document = new PdfDocument();
            document.Info.Title = "Teste";
            document.Info.Subject = "Teste pdf";
            using var contexto = CriarContexto(document);
            RenderizarMarcaPrimeiraFolha(contexto);
            RenderizarTituloPrincipal(contexto, model.Titulo);
            contexto.Y = 110;

            foreach (var bloco in model.Blocos)
            {
                RenderizarBloco(contexto, bloco);
            }

            RenderizarRodape(contexto);

            var filename = IOUtility.GetTempFullFileName("Contrato", "pdf");
            PdfFileUtility.SaveAndShowDocument(document, filename);
        }

        private static RenderContext CriarContexto(PdfDocument document)
        {
            var page = document.AddPage();
            page.Size = PageSize.A4;
            page.Orientation = PageOrientation.Portrait;

            return new RenderContext
            {
                Document = document,
                Page = page,
                Graphics = XGraphics.FromPdfPage(page),
                Y = MargemSuperior
            };
        }

        private static void NovaPagina(RenderContext contexto)
        {
            RenderizarRodape(contexto);
            contexto.Graphics.Dispose();
            var page = contexto.Document.AddPage();
            page.Size = PageSize.A4;
            page.Orientation = PageOrientation.Portrait;

            contexto.Page = page;
            contexto.Graphics = XGraphics.FromPdfPage(page);
            contexto.Y = MargemSuperior;
        }

        private static void GarantirEspaco(RenderContext contexto, double alturaNecessaria)
        {
            var limiteInferior = contexto.Page.Height.Point - MargemInferior;
            if (contexto.Y + alturaNecessaria <= limiteInferior)
            {
                return;
            }

            NovaPagina(contexto);
        }

        private static void RenderizarTituloPrincipal(RenderContext contexto, string titulo)
        {
            var larguraPagina = contexto.Page.Width.Point;
            contexto.Graphics.DrawString(titulo, FonteTituloPrincipal, XBrushes.Black, new XRect(0, 50, contexto.Page.Width, contexto.Page.Height), XStringFormats.TopCenter);

            var tituloSize = contexto.Graphics.MeasureString(titulo, FonteTituloPrincipal);
            var linhaInicioX = (larguraPagina - tituloSize.Width) / 2;
            var linhaFimX = linhaInicioX + tituloSize.Width;
            contexto.Graphics.DrawLine(XPens.Black, linhaInicioX, 80, linhaFimX, 80);
        }

        private static void RenderizarBloco(RenderContext contexto, ContratoPdfBlocoModel bloco)
        {
            switch (bloco.Tipo)
            {
                case ContratoPdfBlocoTipo.Titulo:
                    RenderizarTexto(contexto, bloco.Texto, FonteTitulo, bloco.Recuo, centralizado: bloco.Centralizado, sublinhar: bloco.Sublinhado, espacoDepois: EspacoBloco + 4);
                    break;
                case ContratoPdfBlocoTipo.Paragrafo:
                    RenderizarTexto(contexto, bloco.Texto, bloco.Negrito ? FonteTitulo : FonteCorpo, bloco.Recuo, espacoDepois: EspacoBloco);
                    break;
                case ContratoPdfBlocoTipo.Lista:
                    foreach (var item in bloco.Itens)
                    {
                        RenderizarTexto(contexto, $"• {item}", FonteCorpo, bloco.Recuo, espacoDepois: 2);
                    }
                    contexto.Y += 4;
                    break;
                case ContratoPdfBlocoTipo.Divisoria:
                    GarantirEspaco(contexto, 28);
                    contexto.Y += 8;
                    contexto.Graphics.DrawLine(XPens.Black, MargemEsquerda, contexto.Y, contexto.Page.Width.Point - MargemDireita, contexto.Y);
                    contexto.Y += 20;
                    break;
                case ContratoPdfBlocoTipo.Espaco:
                    contexto.Y += bloco.Espaco;
                    break;
            }
        }

        private static void RenderizarTexto(RenderContext contexto, string texto, XFont fonte, int recuo = 0, bool centralizado = false, bool sublinhar = false, double espacoDepois = EspacoBloco)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return;
            }

            var x = MargemEsquerda + recuo;
            var largura = contexto.Page.Width.Point - x - MargemDireita;

            if (centralizado)
            {
                GarantirEspaco(contexto, AlturaLinha + espacoDepois);
                contexto.Graphics.DrawString(texto, fonte, XBrushes.Black, new XRect(0, contexto.Y, contexto.Page.Width.Point, AlturaLinha), XStringFormats.TopCenter);

                if (sublinhar)
                {
                    var tamanho = contexto.Graphics.MeasureString(texto, fonte);
                    var inicioX = (contexto.Page.Width.Point - tamanho.Width) / 2;
                    contexto.Graphics.DrawLine(XPens.Black, inicioX, contexto.Y + AlturaLinha, inicioX + tamanho.Width, contexto.Y + AlturaLinha);
                }

                contexto.Y += AlturaLinha + espacoDepois;
                return;
            }

            var linhas = QuebrarTexto(contexto.Graphics, texto, fonte, largura);
            GarantirEspaco(contexto, (linhas.Count * AlturaLinha) + espacoDepois);

            foreach (var linha in linhas)
            {
                var rectLinha = new XRect(x, contexto.Y, largura, AlturaLinha);
                contexto.Graphics.DrawString(linha, fonte, XBrushes.Black, rectLinha, XStringFormats.TopLeft);
                contexto.Y += AlturaLinha;
            }

            contexto.Y += espacoDepois;
        }

        private static void RenderizarRodape(RenderContext contexto)
        {
            var yLinha1 = contexto.Page.Height.Point - MargemInferior + 34;
            var yLinha2 = yLinha1 + AlturaLinhaRodape;

            contexto.Graphics.DrawString(RodapeLinha1, FonteRodape, XBrushes.Black,
                new XRect(0, yLinha1, contexto.Page.Width.Point, AlturaLinhaRodape), XStringFormats.TopCenter);

            contexto.Graphics.DrawString(RodapeLinha2, FonteRodape, XBrushes.Black,
                new XRect(0, yLinha2, contexto.Page.Width.Point, AlturaLinhaRodape), XStringFormats.TopCenter);
        }

        private static void RenderizarMarcaPrimeiraFolha(RenderContext contexto)
        {
            const double origemX = 12;
            const double origemY = 8;
            var dourado = XColor.FromArgb(201, 176, 106);
            var douradoClaro = XColor.FromArgb(214, 191, 126);

            var brushDourado = new XSolidBrush(dourado);
            var brushDouradoClaro = new XSolidBrush(douradoClaro);
            const double yBasePredios = 69;

            const double larguraBlocoLateral = 16;
            const double larguraBlocoCentral = 22;
            const double espacoEntreBlocos = 1;

            const double xEsquerda = 35;
            var xCentro = xEsquerda + larguraBlocoLateral + espacoEntreBlocos;
            var xDireita = xCentro + larguraBlocoCentral + espacoEntreBlocos;

            // Bloco central inclinado
            var blocoCentro = new[]
            {
                new XPoint(origemX + xCentro, origemY + 16),
                new XPoint(origemX + xCentro + larguraBlocoCentral, origemY + 8),
                new XPoint(origemX + xCentro + larguraBlocoCentral, origemY + yBasePredios),
                new XPoint(origemX + xCentro, origemY + yBasePredios)
            };
            contexto.Graphics.DrawPolygon(brushDouradoClaro, blocoCentro, XFillMode.Winding);

            // Bloco esquerdo
            var blocoEsquerdo = new[]
            {
                new XPoint(origemX + xEsquerda, origemY + 32),
                new XPoint(origemX + xEsquerda + larguraBlocoLateral, origemY + 22),
                new XPoint(origemX + xEsquerda + larguraBlocoLateral, origemY + yBasePredios),
                new XPoint(origemX + xEsquerda, origemY + yBasePredios)
            };
            contexto.Graphics.DrawPolygon(brushDourado, blocoEsquerdo, XFillMode.Winding);

            // Bloco direito
            var blocoDireito = new[]
            {
                new XPoint(origemX + xDireita, origemY + 22),
                new XPoint(origemX + xDireita + larguraBlocoLateral, origemY + 28),
                new XPoint(origemX + xDireita + larguraBlocoLateral, origemY + yBasePredios),
                new XPoint(origemX + xDireita, origemY + yBasePredios)
            };
            contexto.Graphics.DrawPolygon(brushDourado, blocoDireito, XFillMode.Winding);

            // Faixa/base curva simplificada
            var baseFaixa = new[]
            {
                new XPoint(origemX + 8, origemY + 72),
                new XPoint(origemX + 30, origemY + yBasePredios),
                new XPoint(origemX + 100, origemY + yBasePredios),
                new XPoint(origemX + 120, origemY + 72)

            };
            contexto.Graphics.DrawPolygon(brushDourado, baseFaixa, XFillMode.Winding);

            contexto.Graphics.DrawString("Rosimeri Ribeiro", FonteMarca, brushDourado,
                new XRect(origemX + 20, origemY + 74, 90, 10), XStringFormats.TopCenter);
        }

        private static List<string> QuebrarTexto(XGraphics graphics, string texto, XFont fonte, double larguraMaxima)
        {
            var linhasResultado = new List<string>();
            var partes = texto.Replace("\r", string.Empty).Split('\n');

            foreach (var parte in partes)
            {
                if (string.IsNullOrWhiteSpace(parte))
                {
                    linhasResultado.Add(string.Empty);
                    continue;
                }

                var palavras = parte.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var linhaAtual = new StringBuilder();

                foreach (var palavra in palavras)
                {
                    var textoTeste = linhaAtual.Length == 0 ? palavra : $"{linhaAtual} {palavra}";

                    if (graphics.MeasureString(textoTeste, fonte).Width <= larguraMaxima)
                    {
                        linhaAtual.Clear();
                        linhaAtual.Append(textoTeste);
                        continue;
                    }

                    if (linhaAtual.Length > 0)
                    {
                        linhasResultado.Add(linhaAtual.ToString());
                    }

                    linhaAtual.Clear();
                    linhaAtual.Append(palavra);
                }

                if (linhaAtual.Length > 0)
                {
                    linhasResultado.Add(linhaAtual.ToString());
                }
            }

            return linhasResultado;
        }

        private sealed class RenderContext : IDisposable
        {
            public required PdfDocument Document { get; init; }
            public required PdfPage Page { get; set; }
            public required XGraphics Graphics { get; set; }
            public double Y { get; set; }

            public void Dispose()
            {
                Graphics.Dispose();
            }
        }
    }
}
