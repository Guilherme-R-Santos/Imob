using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using Imob.Models;
using Newtonsoft.Json;

namespace Imob
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient client;
        private bool sucessoConect = false;
        private string tokenJwt;
        private DateTime? tokenExpiration;

        private class LoginJwtResponse
        {
            [JsonProperty("token")]
            public string Token { get; set; }

            [JsonProperty("expiration")]
            public DateTime? Expiration { get; set; }

            [JsonProperty("tipo")]
            public string Tipo { get; set; }
        }

        public MainWindow()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7251/");
            InitializeComponent();
            WindowState = WindowState.Maximized;
            Loaded += MainWindow_Loaded;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await client.GetAsync("Usuario/Connect");

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show("Não foi possível carregar os usuários.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    sucessoConect = true;
                    btnEntrar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF77A10"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar usuários: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = txtUsuario.Text;
            var senha = txtSenha.Password;

            if (!sucessoConect)
            {
                MessageBox.Show("Não foi possível carregar os usuários. Entre em contato com o administrador do sistema", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var response = await client.GetAsync($"Usuario/Login?login={login}&senha={senha}");
                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Usuário ou senha incorretos. Tente novamente.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var loginResponseJson = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginJwtResponse>(loginResponseJson);

                if (loginResponse == null || string.IsNullOrWhiteSpace(loginResponse.Token))
                {
                    MessageBox.Show("Resposta de autenticação inválida.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                tokenJwt = loginResponse.Token;
                tokenExpiration = loginResponse.Expiration;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(loginResponse.Tipo ?? "Bearer", tokenJwt);

                var userResponse = await client.GetAsync($"Usuario/ObterPorLogin/{login}");
                if (!userResponse.IsSuccessStatusCode)
                {
                    MessageBox.Show("Não foi possível obter os dados do usuário. Tente novamente.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var usuarioJson = await userResponse.Content.ReadAsStringAsync();
                var usuarioLogado = JsonConvert.DeserializeObject<UsuarioDAO>(usuarioJson);

                var sistemaWindow = new Sistema();
                sistemaWindow.SetUsuarioLogado(usuarioLogado);
                sistemaWindow.SetAutenticacao(tokenJwt, tokenExpiration, login, senha);
                sistemaWindow.MenuNav.Visibility = Visibility.Hidden;
                sistemaWindow.WindowState = WindowState.Maximized;
                sistemaWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void btnCriar_Click(object sender, RoutedEventArgs e)
        {
            //TODO: Implementar funcionalidade de novo acesso
            MessageBox.Show("Funcionalidade de novo acesso será implementada aqui.", "Novo acesso", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EsqueciSenha_Click(object sender, MouseButtonEventArgs e)
        {
            //TODO: Implementar funcionalidade de recuperação de senha
            MessageBox.Show("Funcionalidade de recuperação de senha será implementada aqui.", "Recuperar Senha", MessageBoxButton.OK, MessageBoxImage.Information);

        }
    }
}