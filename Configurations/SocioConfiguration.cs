using Gimnasio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gimnasio.Configurations
{
    /// <summary>
    /// Configuración de Fluent API para la entidad <see cref="Socio"/>.
    /// Se mapea a la tabla <c>Socios</c> en PostgreSQL.
    /// La relación 1:1 con <see cref="User"/> se configura en
    /// <see cref="UserConfiguration"/>.
    /// </summary>
    public class SocioConfiguration : IEntityTypeConfiguration<Socio>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Socio> builder)
        {
            // ── Tabla ─────────────────────────────────────────────────────────────
            builder.ToTable("Socios");

            // ── Clave primaria ────────────────────────────────────────────────────
            builder.HasKey(s => s.SocioId);
            builder.Property(s => s.SocioId)
                .ValueGeneratedOnAdd();

            // ── Columnas ─────────────────────────────────────────────────────────

            /// <summary>
            /// UserId es la FK y tiene un índice único, asegurando la relación 1:1 con User.
            /// </summary>
            builder.Property(s => s.UserId)
                .IsRequired();

            builder.HasIndex(s => s.UserId)
                .IsUnique();

            builder.Property(s => s.FechaNacimiento)
                .HasColumnType("date");

            /// <summary>
            /// Género se almacena como CHAR(1) con una restricción CHECK en la BD.
            /// EF Core no aplica CHECK constraints de manera nativa; se mantienen en el script SQL.
            /// Valores válidos: 'M', 'F', 'O' o null.
            /// </summary>
            builder.Property(s => s.Genero)
                .HasColumnType("char(1)")
                .HasMaxLength(1);

            builder.Property(s => s.AlturaCm)
                .HasColumnType("decimal(5,2)");

            builder.Property(s => s.PesoKg)
                .HasColumnType("decimal(6,2)");

            builder.Property(s => s.EmergenciaNombre)
                .HasMaxLength(120);

            builder.Property(s => s.EmergenciaTelefono)
                .HasMaxLength(25);

            builder.Property(s => s.FechaRegistro)
                .IsRequired()
                .HasColumnType("date")
                .HasDefaultValueSql("CURRENT_DATE");

            builder.Property(s => s.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // ── Relaciones ───────────────────────────────────────────────────────

            /// <summary>
            /// Un Socio → muchas asignaciones SocioEntrenador (cascade).
            /// </summary>
            builder.HasMany(s => s.SocioEntrenadores)
                .WithOne(se => se.Socio)
                .HasForeignKey(se => se.SocioId)
                .OnDelete(DeleteBehavior.Cascade);

            /// <summary>
            /// Un Socio → muchos registros SocioMembresia (cascade).
            /// </summary>
            builder.HasMany(s => s.SocioMembresias)
                .WithOne(sm => sm.Socio)
                .HasForeignKey(sm => sm.SocioId)
                .OnDelete(DeleteBehavior.Cascade);

            /// <summary>
            /// Un Socio → muchas Asistencias (cascade).
            /// </summary>
            builder.HasMany(s => s.Asistencias)
                .WithOne(a => a.Socio)
                .HasForeignKey(a => a.SocioId)
                .OnDelete(DeleteBehavior.Cascade);

            /// <summary>
            /// Un Socio → muchas Rutinas (cascade).
            /// </summary>
            builder.HasMany(s => s.Rutinas)
                .WithOne(r => r.Socio)
                .HasForeignKey(r => r.SocioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}