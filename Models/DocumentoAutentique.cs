namespace Imob.Models
{
    public class DocumentoAutentique
    {
        public string IdDocumentoAutentique { get; set; }
        public string Nome { get; set; }
        public bool Sandbox { get; set; }
        public DateTimeOffset? CriadoEmUtc { get; set; }
        public List<SignatarioAutentique> Signatarios { get; set; }
    }

    public class SignatarioAutentique
    {
        public string PublicIdAutentique { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Acao { get; set; }
    }
}
