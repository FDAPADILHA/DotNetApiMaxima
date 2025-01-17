using DotNetApiMaxima.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetApiMaxima.Config
{
    public class UsuarioMapping : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("MXSUSUARIOS");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasColumnName("IDUSUARIO")
                .IsRequired(true);

            builder.Property(u => u.Nome)
                .HasColumnName("NOME")
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(u => u.Login)
                .HasColumnName("LOGIN")
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(u => u.Senha)
               .HasColumnName("SENHA")
               .IsRequired(true)
               .HasMaxLength(50);

            builder.Property(u => u.Status)
                .HasColumnName("STATUS")
                .IsRequired(true)
                .HasMaxLength(1);

        }
    }
}
