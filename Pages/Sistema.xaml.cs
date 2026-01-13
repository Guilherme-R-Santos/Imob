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

        public void FecharPanelsAtivos()
        {
            if (ProprietariosPanel.Visibility == Visibility.Visible) ProprietariosPanel.Visibility = Visibility.Hidden;
            
            if (LocatariosPanel.Visibility == Visibility.Visible) LocatariosPanel.Visibility = Visibility.Hidden;
            
            if (FiadoresPanel.Visibility == Visibility.Visible) FiadoresPanel.Visibility = Visibility.Hidden;
            
            if (ImoveisPanel.Visibility == Visibility.Visible) ImoveisPanel.Visibility = Visibility.Hidden;
            
            if (ContratosPanel.Visibility == Visibility.Visible) ContratosPanel.Visibility = Visibility.Hidden;
            
            if (VistoriasPanel.Visibility == Visibility.Visible) VistoriasPanel.Visibility = Visibility.Hidden;
        }
        
        // TODO: Implementar resetar filtros e valores dos panels
        public void ResetarPanels()
        {

        }

        // TODO: Implementar obtenção de informações da API
        public void GetInformacoesApi(string panelAtivo)
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
            FecharPanelsAtivos();
            // TODO: Resetar valores e trazer todos os imóveis
            ResetarPanels();
            ImoveisPanel.Visibility = Visibility.Visible;
        }

        private void ContratosTreeCompraVenda_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            // TODO: Resetar valores e trazer contratos com filtro de tipo Compra e Venda
            ResetarPanels();
            ContratosPanel.Visibility = Visibility.Visible;
        }

        private void ContratosTreeLocacao_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            // TODO: Resetar valores e trazer contratos com filtro de tipo locação
            ResetarPanels();
            ContratosPanel.Visibility = Visibility.Visible;
        }

        private void ProprietariosTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            // TODO: Resetar valores e trazer todos os proprietários
            ResetarPanels();
            ProprietariosPanel.Visibility = Visibility.Visible;
        }

        private void LocatariosTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            // TODO: Resetar valores e trazer todos os locatários
            ResetarPanels();
            LocatariosPanel.Visibility = Visibility.Visible;
        }

        private void FiadoresTree_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            // TODO: Resetar valores e trazer todos os fiadores
            ResetarPanels();
            FiadoresPanel.Visibility = Visibility.Visible;
        }

        private void VistoriaTreeListar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            // TODO: Resetar valores e trazer todas as vistorias
            ResetarPanels();
            VistoriasPanel.Visibility = Visibility.Visible;
        }

        private void VistoriaTreeCriar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FecharPanelsAtivos();
            // TODO: Resetar valores e trazer painel de criação de vistoria
            ResetarPanels();
            VistoriasPanel.Visibility = Visibility.Visible;
        }

        private void BtnAdicionarImovel_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Finalizar
            ImoveilModalOverlayCriar.Visibility = Visibility.Visible;
        }

        private void BtnFecharModalImovel_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Resetar valores do modal
            ImoveilModalOverlayCriar.Visibility = Visibility.Hidden;
        }

        //Click Events Fim
    }
}
