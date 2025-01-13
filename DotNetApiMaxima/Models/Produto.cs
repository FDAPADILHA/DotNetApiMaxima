namespace DotNetApiMaxima.Models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Codprod { get; set; }
        public string Descricao { get; set; }
        public string Coddepto { get; set; }
        public decimal Preco { get; set; }
        public string Status { get; set; }
        public int Codoperacao { get; set; }

    }
}
