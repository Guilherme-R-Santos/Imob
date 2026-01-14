using System;
using System.Collections.Generic;
using System.Text;

namespace Imob.Models
{
    public class FotoDAO
    {
        public int Id { get; set; }
        public ImovelDAO Imovel { get; set; }
        public string NomeArquivo { get; set; }
        public TipoFotoDAO TipoFoto { get; set; }
        public byte[] Bin { get; set; }
        public UsuarioDAO Cadastrador { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataInativacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Principal { get; set; }
        public bool Ativo { get; set; }
        public VistoriaDAO Vistoria { get; set; }
    }
}
