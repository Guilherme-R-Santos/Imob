using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace Imob.Models
{
    public class VistoriaDAO
    {
        public int Id { get; set; }
        public UsuarioDAO Cadastrador { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }
        public ContratoDAO Contrato { get; set; }
        public ImovelDAO Imovel { get; set; }
        public DateTime? DataVistoria { get; set; }
        public DateTime? DataEntregaChaves { get; set; }
        public string Observacoes { get; set; }
    }
}
