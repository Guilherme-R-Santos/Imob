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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;

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

        // Funções auxilires Inicio

        public void checarMenusAtivos()
        {

        }

        public void FecharMenusAbertos()
        {

        }

        public void ResetarMenus()
        {

        }

        // Funções auxilires Fim

        public Sistema()
        {

            InitializeComponent();
            WindowState = WindowState.Maximized;

            // Mouse Enter and Leave Events Inicio

            ImgPwrOff.MouseEnter += (s, e) =>
            {
                var circle = CircPwrOff;
                Mouse.OverrideCursor = Cursors.Hand;
                circle.Fill = new SolidColorBrush(Color.FromRgb(200, 200, 200));

            };

            ImgPwrOff.MouseLeave += (s, e) =>
            {
                var circle = CircPwrOff;

                Mouse.OverrideCursor = Cursors.Arrow;
                circle.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            };

            RetangLogo.MouseEnter += (s, e) => {
                var ret = RetangLogo;
                Mouse.OverrideCursor = Cursors.Hand;
            };

            RetangLogo.MouseLeave += (s, e) =>
            {
                var ret = RetangLogo;
                Mouse.OverrideCursor = Cursors.Arrow;
            };

            LogoNav.MouseEnter += (s, e) =>
            {
                var ret = RetangLogo;
                Mouse.OverrideCursor = Cursors.Hand;
            };

            LogoNav.MouseLeave += (s, e) =>
            {
                var ret = RetangLogo;
                Mouse.OverrideCursor = Cursors.Arrow;
            };

            // Mouse Enter and Leave Events Fim

            //Click Events Inicio

            RetangLogo.MouseDown += (s, e) =>
            {
                var ret = RetangLogo;

                var visibilidadeMenu = MenuNav.Visibility;
                if (visibilidadeMenu == Visibility.Visible)
                {
                    MenuNav.Visibility = Visibility.Collapsed;
                    ret.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));

                }
                else
                {
                    MenuNav.Visibility = Visibility.Visible;
                    ret.Fill = new SolidColorBrush(Color.FromRgb(200, 200, 200));
                }
            };

            ImgPwrOff.MouseDown += (s, e) =>
            {
                var result = MessageBox.Show("Tem certeza que deseja sair do sistema?", "Sair", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var loginWindow = new MainWindow();
                    loginWindow.Show();
                    this.Close();
                }
            };

            LogoNav.MouseDown += (s, e) =>
            {
                var ret = RetangLogo;
                
                var visibilidadeMenu = MenuNav.Visibility;
                if (visibilidadeMenu == Visibility.Visible)
                {
                    MenuNav.Visibility = Visibility.Collapsed;
                    ret.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));

                }
                else
                {
                    MenuNav.Visibility = Visibility.Visible;
                    ret.Fill = new SolidColorBrush(Color.FromRgb(200, 200, 200));
                }
            };
        }

        private void ImoveisTree_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ContratosTreeCompraVenda_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ContratosTreeLocacao_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ProprietariosTree_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void LocatariosTree_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void FiadoresTree_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void VistoriaTreeListar_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void VistoriaTreeCriar_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        //Click Events Fim
    }
}
