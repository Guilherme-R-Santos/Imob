using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace Imob.Models
{
    public class FotoDTO
    {
        public int Id { get; set; }
        public int ImovelId { get; set; }
        public string NomeArquivo { get; set; }
        public int TipoFoto { get; set; }
        public byte[] Bin { get; set; }
        public int CadastradorId { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataInativacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Principal { get; set; }
        public bool Ativo { get; set; }
        public int VistoriaId { get; set; }

        public async Task CadastrarFoto(HttpClient httpClient)
        {
            var fotoJson = new
            {
                Imovel = new { Id = this.ImovelId },
                NomeArquivo = this.NomeArquivo,
                Bin = this.Bin,
                Cadastrador = new { Id = this.CadastradorId },
                TipoFoto = new { Id = this.TipoFoto },
                Principal = this.Principal
            };
            var jsonContent = JsonConvert.SerializeObject(fotoJson);
            var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("Foto/Criar", contentString);
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Erro ao cadastrar foto: " + response.StatusCode);
            }
        }

        public async Task InativarFoto(int idFoto, HttpClient httpClient)
        {
            var response = await httpClient.PutAsync($"Foto/Inativar/{idFoto}", null);
            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Erro ao inativar foto: " + response.StatusCode);
            }
        }
    }
}