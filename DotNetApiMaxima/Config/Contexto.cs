using DotNetApiMaxima.Models;
using DotNetApiMaxima.Config;
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

        public DbSet<Produto> Produto { get; set; }
        public DbSet<Departamento> Departamento { get; set; }
        public DbSet<Usuario> Usuario { get; set; }


        // Configuração da conexão com o banco de dados
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseOracle(connectionString);

                optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
            }
        }

        // Configuração dos mapeamentos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new DepartamentoMappging());
            modelBuilder.ApplyConfiguration(new ProdutoMapping());
            modelBuilder.ApplyConfiguration(new UsuarioMapping());


        }


    }
}
