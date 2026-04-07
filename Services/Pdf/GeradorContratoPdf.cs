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

namespace Imob.Services.Pdf
{
    public class GeradorContratoPdf
    {
        //public void CriarContrato(ContratoDAO contrato)
        public void CriarContrato()
        {
            var document = new PdfDocument();
            document.Info.Title = "Teste";
            document.Info.Subject = "Teste pdf";
            var page = document.AddPage();
            var width = page.Width.Point;
            var height = page.Height.Point;
            page.Size = PageSize.A4;
            page.Orientation = PageOrientation.Portrait;
            var gfx = XGraphics.FromPdfPage(page);
            var fontCorpo = new XFont("Times New Roman", 12, XFontStyleEx.Regular);
            var fontTitulo = new XFont("Times New Roman", 18, XFontStyleEx.Bold);
            var fontTituloPrincipal = new XFont("Times New Roman", 24, XFontStyleEx.Bold);

            gfx.DrawString("CONTRATO DE LOCAÇÃO", fontTituloPrincipal, XBrushes.Black, new XRect(0, 50, page.Width, page.Height), XStringFormats.TopCenter);
            gfx.DrawString("Pelo presente instrumento, na melhor forma de direito e de acordo com todas as", fontCorpo, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);
            //gfx.DrawString("disposições legais expressas em lei, fica acordada a seguinte locação:", fontCorpo, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);
            //gfx.DrawString("DO IMÓVEL: Rua Marechal Cantuária, 102, loja C, URCA, Rio de Janeiro,", fontCorpo, XBrushes.Black, new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);

            var filename = IOUtility.GetTempFullFileName("Contrato", "pdf");
            //document.Save(filename);
            PdfFileUtility.SaveAndShowDocument(document, filename);
        }
    }
}
