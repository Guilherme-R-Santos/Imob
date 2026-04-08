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

namespace Imob.Services.Pdf
{
    public class GeradorContratoPdf
    {
        public int linhaX = 25;
        public int linhaY = 110;
        PdfDocument document = new PdfDocument();
        
        public void CriarContratoLocacao(ContratoDAO contrato)
        {
            //var document = new PdfDocument();
            var page = document.AddPage();
            document.Info.Title = "Teste";
            document.Info.Subject = "Teste pdf";
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

            AdicionarLinhaParagrafo(gfx, $"LOCADOR: {contrato.Proprietario.Nome.ToUpper()}, {contrato.Proprietario.Nacionalidade}, {contrato.Proprietario.EstadoCivil}, {contrato.Proprietario.Profissao},", fontCorpo, linhaX, linhaY, page);
            AdicionarLinhaParagrafo(gfx, $"portador do CPF nº {contrato.Proprietario.CpfCnpj}, residente nesta cidade. E-MAIL: {contrato.Proprietario.Email}.", fontCorpo, linhaX, linhaY, page);

            linhaY += 20;

            AdicionarLinhaParagrafo(gfx, "LOCATÁRIOS:", fontTitulo, linhaX, linhaY, page);

            linhaY += 20;
            linhaX = 50;

            AdicionarLinhaParagrafo(gfx, $"1º LOCATÁRIO: {contrato.Contratante1.Nome.ToUpper()}, {contrato.Contratante1.Nacionalidade}, {contrato.Contratante1.EstadoCivil}, {contrato.Contratante1.Profissao},", fontCorpo, linhaX, linhaY, page);
            AdicionarLinhaParagrafo(gfx, $"Nascido em {contrato.Contratante1.DataNascimento?.ToString("dd/MM/yyyy")}, portador da identidade nº {contrato.Contratante1.Identidade} e CPF nº {contrato.Contratante1.CpfCnpj},", fontCorpo, linhaX, linhaY, page);
            
            if (contrato.Contratante1.Endereco != null)
            {
                AdicionarLinhaParagrafo(gfx, $"residente à {contrato.Contratante1.Endereco},", fontCorpo, linhaX, linhaY, page);
            }

            AdicionarLinhaParagrafo(gfx, $"e-mail: {contrato.Contratante1.Email}.", fontCorpo, linhaX, linhaY, page);

            if (contrato.Contratante2 != null)
            {
                linhaY += 5;
                AdicionarLinhaParagrafo(gfx, $"2º LOCATÁRIO: {contrato.Contratante2.Nome.ToUpper()}, {contrato.Contratante2.Nacionalidade}, {contrato.Contratante2.EstadoCivil}, {contrato.Contratante2.Profissao},", fontCorpo, linhaX, linhaY, page);
                AdicionarLinhaParagrafo(gfx, $"Nascido em {contrato.Contratante2.DataNascimento?.ToString("dd/MM/yyyy")}, portador da identidade nº {contrato.Contratante2.Identidade} e CPF nº {contrato.Contratante2.CpfCnpj},", fontCorpo, linhaX, linhaY, page);

                if (contrato.Contratante2.Endereco != null)
                {
                    AdicionarLinhaParagrafo(gfx, $"residente à {contrato.Contratante2.Endereco},", fontCorpo, linhaX, linhaY, page);
                }

                AdicionarLinhaParagrafo(gfx, $"e-mail: {contrato.Contratante2.Email}.", fontCorpo, linhaX, linhaY, page);

            }

            if (contrato.Contratante3 != null)
            {
                linhaY += 5;
                AdicionarLinhaParagrafo(gfx, $"3º LOCATÁRIO: {contrato.Contratante3.Nome.ToUpper()}, {contrato.Contratante3.Nacionalidade}, {contrato.Contratante3.EstadoCivil}, {contrato.Contratante3.Profissao},", fontCorpo, linhaX, linhaY, page);
                AdicionarLinhaParagrafo(gfx, $"Nascido em {contrato.Contratante3.DataNascimento?.ToString("dd/MM/yyyy")}, portador da identidade nº {contrato.Contratante3.Identidade} e CPF nº {contrato.Contratante3.CpfCnpj},", fontCorpo, linhaX, linhaY, page);

                if (contrato.Contratante3.Endereco != null)
                {
                    AdicionarLinhaParagrafo(gfx, $"residente à {contrato.Contratante3.Endereco},", fontCorpo, linhaX, linhaY, page);
                }

                AdicionarLinhaParagrafo(gfx, $"e-mail: {contrato.Contratante3.Email}.", fontCorpo, linhaX, linhaY, page);
            }

            if (contrato.Contratante4 != null)
            {
                linhaY += 5;
                AdicionarLinhaParagrafo(gfx, $"4º LOCATÁRIO: {contrato.Contratante4.Nome.ToUpper()}, {contrato.Contratante4.Nacionalidade}, {contrato.Contratante4.EstadoCivil}, {contrato.Contratante4.Profissao},", fontCorpo, linhaX, linhaY, page);
                AdicionarLinhaParagrafo(gfx, $"Nascido em {contrato.Contratante4.DataNascimento?.ToString("dd/MM/yyyy")}, portador da identidade nº {contrato.Contratante4.Identidade} e CPF nº {contrato.Contratante4.CpfCnpj},", fontCorpo, linhaX, linhaY, page);

                if (contrato.Contratante4.Endereco != null)
                {
                    AdicionarLinhaParagrafo(gfx, $"residente à {contrato.Contratante4.Endereco},", fontCorpo, linhaX, linhaY, page);
                }

                AdicionarLinhaParagrafo(gfx, $"e-mail: {contrato.Contratante4.Email}.", fontCorpo, linhaX, linhaY, page);
            }

            linhaY += 20;
            linhaX = 25;

            AdicionarLinhaParagrafo(gfx, "As partes acima identificadas têm entre si justo e contratado o presente contrato de Locação Residencial,", fontCorpo, linhaX, linhaY, page);
            AdicionarLinhaParagrafo(gfx, "que se regerá pelas cláusulas e condições seguintes:", fontCorpo, linhaX, linhaY, page);

            var filename = IOUtility.GetTempFullFileName("Contrato", "pdf");
            PdfFileUtility.SaveAndShowDocument(document, filename);
        }

        public void AdicionarLinhaParagrafo(XGraphics gfx, string texto, XFont fonte, int x, int y, PdfPage page)
        {
            var rect = new XRect(x, y, page.Width - x - 40, page.Height - y);
            gfx.DrawString(texto, fonte, XBrushes.Black, rect, XStringFormats.TopLeft);
            linhaY += 16;
        }
    }
}
