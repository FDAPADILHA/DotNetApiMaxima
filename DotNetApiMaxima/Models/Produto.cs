using System.Text.Json.Serialization;

namespace DotNetApiMaxima.Models
{
    public class Produto
    {
        [JsonIgnore]
        public int? Id { get; set; }
        public required string Codprod { get; set; }
        public string? Descricao { get; set; }
        public required string Coddepto { get; set; }
        public decimal Preco { get; set; }
        public string? Status { get; set; }
        [JsonIgnore]
        public int? Codoperacao { get; set; }
    }
}
