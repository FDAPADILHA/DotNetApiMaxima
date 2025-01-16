using System.Text.Json.Serialization;

namespace DotNetApiMaxima.Models
{
    public class Departamento
    {
        [JsonIgnore]
        public int? Id { get; set; }

        public required string Coddepto { get; set; }
        public string? Descricao { get; set; }
        public string? Status { get; set; }
    }
}
