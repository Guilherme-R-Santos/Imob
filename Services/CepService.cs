using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Imob.Services
{
    public class EnderecoCep
    {
        [JsonProperty("cep")] public string Cep { get; set; }
        [JsonProperty("uf")] public string Uf { get; set; }
        [JsonProperty("cidade")] public string Cidade { get; set; }
        [JsonProperty("bairro")] public string Bairro { get; set; }
        [JsonProperty("endereco")] public string Endereco { get; set; }
        [JsonProperty("complemento")] public string Complemento { get; set; }
    }

    public static class CepService
    {
        private static readonly HttpClient _http = new HttpClient();
        private static readonly SemaphoreSlim _tokenLock = new SemaphoreSlim(1, 1);
        private static string _token;
        private static DateTime _tokenExpiraEm = DateTime.MinValue;

        private class TokenResponse
        {
            [JsonProperty("access_token")] public string AccessToken { get; set; }
            [JsonProperty("expires_in")] public int ExpiresIn { get; set; }
        }

        public static string SomenteDigitos(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return string.Empty;
            var sb = new StringBuilder();
            foreach (var c in texto)
            {
                if (char.IsDigit(c)) sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>Retorna o endereço do CEP ou null se não encontrado.</summary>
        public static async Task<EnderecoCep> ConsultarAsync(string cep)
        {
            cep = SomenteDigitos(cep);
            if (cep.Length != 8) return null;

            var baseUrl = App.Configuration["Cep:BaseUrl"]?.TrimEnd('/');
            var cpf = App.Configuration["Cep:CpfUsuario"];
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException("Configuração 'Cep:BaseUrl' não preenchida.");
            if (string.IsNullOrWhiteSpace(cpf))
                throw new InvalidOperationException("Configuração 'Cep:CpfUsuario' não preenchida.");

            var token = await ObterTokenAsync();

            using var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/consulta/cep/{cep}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("x-cpf-usuario", cpf);

            using var response = await _http.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.BadRequest)
                return null;

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<EnderecoCep>(json);
        }

        private static async Task<string> ObterTokenAsync()
        {
            if (_token != null && DateTime.UtcNow < _tokenExpiraEm) return _token;

            await _tokenLock.WaitAsync();
            try
            {
                if (_token != null && DateTime.UtcNow < _tokenExpiraEm) return _token;

                var tokenUrl = App.Configuration["Cep:TokenUrl"];
                var clientId = App.Configuration["Cep:ClientId"];
                var clientSecret = App.Configuration["Cep:ClientSecret"];
                if (string.IsNullOrWhiteSpace(tokenUrl) || string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                    throw new InvalidOperationException("Configurações 'Cep:TokenUrl', 'Cep:ClientId' e 'Cep:ClientSecret' são obrigatórias.");

                using var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
                {
                    Content = new FormUrlEncodedContent(new[] { new System.Collections.Generic.KeyValuePair<string, string>("grant_type", "client_credentials") })
                };
                var basic = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);

                using var response = await _http.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var resp = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());

                _token = resp.AccessToken;
                _tokenExpiraEm = DateTime.UtcNow.AddSeconds(Math.Max(60, resp.ExpiresIn - 300));
                return _token;
            }
            finally
            {
                _tokenLock.Release();
            }
        }
    }
}
