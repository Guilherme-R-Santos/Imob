using System.Text;
using System.Windows;
using System.Windows.Controls;
using Imob.Models;

namespace Imob.Views.Modals
{
    public class DocumentosAutentiqueWindow : Window
    {
        public DocumentosAutentiqueWindow(ContratoDAO contrato, IReadOnlyList<DocumentoAutentique> documentos)
        {
            Title = "Registros de assinatura";
            Width = 700;
            Height = 500;
            MinWidth = 450;
            MinHeight = 300;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var texto = new StringBuilder();
            texto.AppendLine($"Contrato: {contrato.Nome} (ID {contrato.Id})");
            texto.AppendLine($"Documento vinculado: {contrato.IdDocumentoAutentique ?? "não informado"}");
            texto.AppendLine(contrato.EnvioAutentiqueEmAndamento
                ? "Envio em andamento ou pendente de conciliação."
                : "Nenhum envio em andamento ou pendente de conciliação.");
            texto.AppendLine();
            texto.AppendLine("Registros salvos pela ImobAPI; não são uma consulta em tempo real à Autentique. Status e datas de assinatura podem estar desatualizados.");
            texto.AppendLine();

            if (documentos.Count == 0)
                texto.AppendLine("Nenhum documento registrado para este contrato.");

            foreach (var documento in documentos)
            {
                texto.AppendLine($"Documento: {documento.Nome}");
                texto.AppendLine($"ID: {documento.IdDocumentoAutentique}");
                texto.AppendLine($"Ambiente: {(documento.Sandbox ? "sandbox" : "produção")}");
                texto.AppendLine($"Criado em: {documento.CriadoEmUtc?.ToLocalTime().ToString("g") ?? "não informado"}");
                texto.AppendLine("Participantes registrados:");
                if (documento.Signatarios == null || documento.Signatarios.Count == 0)
                    texto.AppendLine("  Nenhum participante registrado.");
                else
                    foreach (var signatario in documento.Signatarios)
                    {
                        texto.AppendLine($"  {signatario.Nome ?? "Nome não informado"} | {signatario.Email ?? signatario.Telefone ?? "Contato não informado"} | {signatario.Acao ?? "Ação não informada"}");
                        texto.AppendLine($"    ID público: {signatario.PublicIdAutentique}");
                    }
                texto.AppendLine();
            }

            Content = new TextBox
            {
                Text = texto.ToString(),
                IsReadOnly = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Margin = new Thickness(15)
            };
        }
    }
}
