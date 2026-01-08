using System;
using System.Collections.Generic;
using System.Text;

namespace Imob.Models
{
    internal class ClienteDAO
    {
        public int Id { get; set; }
        public TipoClienteDAO TipoCliente { get; set; }
        public UsuarioDAO Cadastrador { get; set; }
        public string Nome { get; set; }
        public string CpfCnpj { get; set; }
        public string Identidade { get; set; }
        public string OrgaoExpedidor { get; set; }
        public string Nacionalidade { get; set; }
        public string Naturalidade { get; set; }
        public string EstadoCivil { get; set; }
        public string Profissao { get; set; }
        public string Endereco { get; set; }
        public string Agencia { get; set; }
        public string Conta { get; set; }
        public string CodBanco { get; set; }
        public string Banco { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public bool Ativo { get; set; }
        public DateTime? DataNascimento { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public DateTime? DataInativacao { get; set; }
    }
}
