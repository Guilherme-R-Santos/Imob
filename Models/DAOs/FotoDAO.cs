using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Imob.Models
{
    public class FotoDAO
    {
        public int Id { get; set; }
        public ImovelDAO Imovel { get; set; }
        public string NomeArquivo { get; set; }
        public TipoFotoDAO TipoFoto { get; set; }
        public byte[] Bin { get; set; }
        public UsuarioDAO Cadastrador { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataInativacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Principal { get; set; }
        public bool Ativo { get; set; }
        public VistoriaDAO Vistoria { get; set; }

        public static async Task<List<FotoDAO>> GetFotosPorImovel(int imovelId, HttpClient httpClient)
        {
            try
            {
                var response = await httpClient.GetAsync($"Foto/ObterPorImovel/{imovelId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<FotoDAO>>(json);

                } else
                {
                    throw new Exception("Erro ao obter fotos do imóvel.");
                }
            } catch
            {
                throw new Exception("Erro ao conectar com o servidor.");
            }
        }
    }
}
