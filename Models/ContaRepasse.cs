using System;

namespace Imob.Models
{
    public class ContaRepasse
    {
        public string Modalidade { get; set; }
        public string TipoValor { get; set; }
        public decimal Valor { get; set; }
        public string ChavePix { get; set; }
        public string TipoChavePix { get; set; }
        public string CodigoBanco { get; set; }
        public string NomeTitular { get; set; }
        public string CpfCnpjTitular { get; set; }
        public DateTime? DataNascimentoTitular { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public string DigitoConta { get; set; }
        public string TipoConta { get; set; }
    }
}
