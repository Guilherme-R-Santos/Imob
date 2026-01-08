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
using Imob.Components;
using Imob.Models;

namespace Imob
{
    public partial class Sistema : Window
    {
        public UsuarioDAO UsuarioLogado { get; set; }

        public void SetUsuarioLogado(UsuarioDAO usuarioLogado)
        {
            UsuarioLogado = usuarioLogado;
            UsuarioAtivo.Content = UsuarioLogado.Login;
        }

        public Sistema()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;

            LogoNav.MouseDown += (s, e) =>
            {
                //MessageBox.Show("Funcionalidade de retornar para o Home será implementada aqui.", "Retornar para o Home", MessageBoxButton.OK, MessageBoxImage.Information);
                var visibilidadeMenu = NavMenu.Visibility;
                if (visibilidadeMenu == Visibility.Visible)
                {
                    NavMenu.Visibility = Visibility.Collapsed;

                }
                else
                {
                    NavMenu.Visibility = Visibility.Visible;
                }
            };
        }

        private void btnCriarUsuario_Click(object sender, RoutedEventArgs e)
        {
            var JanelaCadastrarUsuario = new JanelaCadastroUsuario();
            JanelaCadastrarUsuario.Show();

        }
    }
}
