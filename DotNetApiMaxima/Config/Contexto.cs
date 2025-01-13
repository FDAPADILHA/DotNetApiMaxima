using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DotNetApiMaxima.Config
{
    public class Contexto : DbContext
    {
        private readonly IConfiguration _configuration;

        public Contexto(DbContextOptions<Contexto> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Models.Produto> Produto { get; set; }

        //Adicionei a injeção do IConfiguration no construtor do Contexto. Isso permite que o contexto acesse as configurações definidas no appsettings.json
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseOracle(connectionString);
            }
        }
    }
}
