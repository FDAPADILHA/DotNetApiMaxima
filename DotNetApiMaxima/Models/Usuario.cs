using System.Text.Json.Serialization;

namespace DotNetApiMaxima.Models
{
    public class Usuario
    {
        public required int Id { get; set; }
        public required string Nome { get; set; }
        public required string Login { get; set; }
        public required string Senha { get; set; }
        public required string Status { get; set; }
    }
}
