using DotNetApiMaxima.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetApiMaxima.Config
{
    public class ProdutoMapping : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("MXSPRODUTO");
            builder.HasKey(p => p.Codprod);

            builder.Property(p => p.Id)
                .HasColumnName("IDPROD")
                .IsRequired(false);

            builder.Property(p => p.Codprod)
                .HasColumnName("CODPROD")
                .IsRequired(true);

            builder.Property(p => p.Descricao)
                .HasColumnName("DESCRICAO")
                .IsRequired(false)
                .HasMaxLength(4000);

            builder.Property(p => p.Coddepto)
                .HasColumnName("CODDEPTO")
                .IsRequired(true);

            builder.Property(p => p.Preco)
                .HasColumnName("PRECO")
                .IsRequired(true);

            builder.Property(p => p.Status)
                .HasColumnName("STATUS")
                .IsRequired(true)
                .HasMaxLength(1);

            builder.Property(p => p.Codoperacao)
                .HasColumnName("CODOPERACAO")
                .IsRequired(false);
        }
    }
}
