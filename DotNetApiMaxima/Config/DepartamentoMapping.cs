using DotNetApiMaxima.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetApiMaxima.Config
{
    public class DepartamentoMappging : IEntityTypeConfiguration<Departamento>
    {
        public void Configure(EntityTypeBuilder<Departamento> builder)
        {
            builder.ToTable("MXSDEPARTAMENTO");
            builder.HasKey(d => d.Coddepto);

            builder.Property(d => d.Id)
                .HasColumnName("IDDEPTO")
                .IsRequired(false);

            builder.Property(d => d.Descricao)
                .HasColumnName("DESCRICAO")
                .IsRequired(false)
                .HasMaxLength(4000);

            builder.Property(d=> d.Coddepto)
                .HasColumnName("CODDEPTO")
                .IsRequired();

            builder.Property(d => d.Status)
                .HasColumnName("STATUS")
                .IsRequired(false);
        }
    }
}
