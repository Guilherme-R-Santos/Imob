using Imob.Models.DAOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Imob.Models.DAOs
{
    public class TipoUsuarioDAO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public DateTime DataInativacao { get; set; }
    }
}
