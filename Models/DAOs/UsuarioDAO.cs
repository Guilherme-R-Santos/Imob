using Imob.Models.DAOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imob.Models
{
    public class UsuarioDAO
    {
        public int Id { get; set; }
        public TipoUsuarioDAO Tipo { get; set;  }
        public string Nome { get; set; }
        public string Login { get; set; }
        public string email { get; set; }
        public string Senha { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }

    }
}
