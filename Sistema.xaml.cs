using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Imob
{

    public partial class Sistema : Window
    {
        public string UsuarioLogado { get; set; }
        public string SenhaUsuarioLogado { get; set; }

        public void SetUsuarioLogado(string usuario)
        {
            UsuarioLogado = usuario;
            UsuarioAtivo.Content = UsuarioLogado;
        }

        public void SetSenhaUsuarioLogado(string senha)
        {
            SenhaUsuarioLogado = senha;
        }

        public Sistema()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
        }
    }
}
