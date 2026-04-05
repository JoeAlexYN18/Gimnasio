using Gimnasio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gimnasio.Configurations
{
    /// <summary>
    /// Configuración de Fluent API para la entidad <see cref="Entrenador"/>.
    /// Se mapea a la tabla <c>Entrenadores</c> en PostgreSQL.
    /// La relación 1:1 con <see cref="User"/> se configura en
    /// <see cref="UserConfiguration"/>.
    /// </summary>
    public class EntrenadorConfiguration : IEntityTypeConfiguration<Entrenador>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Entrenador> builder)
        {
            // ── Tabla ─────────────────────────────────────────────────────────────
            builder.ToTable("Entrenadores");

            // ── Clave primaria ────────────────────────────────────────────────────
            builder.HasKey(e => e.EntrenadorId);
            builder.Property(e => e.EntrenadorId)
                .ValueGeneratedOnAdd();

            // ── Columnas ─────────────────────────────────────────────────────────

            /// <summary>
            /// UserId es la FK y también tiene una restricción única,
            /// asegurando la relación 1:1 a nivel de base de datos.
            /// </summary>
            builder.Property(e => e.UserId)
                .IsRequired();

            builder.HasIndex(e => e.UserId)
                .IsUnique();

            builder.Property(e => e.Especialidad)
                .HasMaxLength(120);

            builder.Property(e => e.Certificaciones)
                .HasMaxLength(250);

            /// <summary>
            /// DateOnly se mapea a PostgreSQL <c>date</c> — sin componente de tiempo.
            /// HasDefaultValueSql refleja DEFAULT CURRENT_DATE en el script SQL.
            /// </summary>
            builder.Property(e => e.FechaIngreso)
                .IsRequired()
                .HasColumnType("date")
                .HasDefaultValueSql("CURRENT_DATE");

            builder.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // ── Relaciones ───────────────────────────────────────────────────────

            /// <summary>
            /// Un Entrenador → muchas asignaciones SocioEntrenador.
            /// Cascade: eliminar un entrenador borra las filas de asignación.
            /// </summary>
            builder.HasMany(e => e.SocioEntrenadores)
                .WithOne(se => se.Entrenador)
                .HasForeignKey(se => se.EntrenadorId)
                .OnDelete(DeleteBehavior.Cascade);

            /// <summary>
            /// Un Entrenador → muchas Rutinas.
            /// Restricción: las rutinas permanecen en la BD incluso si se elimina el entrenador
            /// (EntrenadorId es nullable en Rutina).
            /// </summary>
            builder.HasMany(e => e.Rutinas)
                .WithOne(r => r.Entrenador)
                .HasForeignKey(r => r.EntrenadorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}