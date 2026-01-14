using System;
using System.Collections.Generic;
using System.Text;

namespace Imob.Models
{
    public class ObjetoContratoDAO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public UsuarioDAO Cadastrador { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }
    }
}
