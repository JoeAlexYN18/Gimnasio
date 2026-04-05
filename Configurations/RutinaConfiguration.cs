using Gimnasio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gimnasio.Configurations
{
    /// <summary>
    /// Configuración de Fluent API para la entidad <see cref="Rutina"/>.
    /// Se mapea a la tabla <c>Rutinas</c> en PostgreSQL.
    /// Las relaciones con <see cref="Socio"/> y <see cref="Entrenador"/>
    /// se configuran en <see cref="SocioConfiguration"/> y
    /// <see cref="EntrenadorConfiguration"/> respectivamente.
    /// </summary>
    public class RutinaConfiguration : IEntityTypeConfiguration<Rutina>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Rutina> builder)
        {
            // ── Tabla ─────────────────────────────────────────────────────────────
            builder.ToTable("Rutinas");

            // ── Clave primaria ────────────────────────────────────────────────────
            builder.HasKey(r => r.RutinaId);
            builder.Property(r => r.RutinaId)
                .ValueGeneratedOnAdd();

            // ── Columnas ─────────────────────────────────────────────────────────
            builder.Property(r => r.SocioId)
                .IsRequired();

            /// <summary>
            /// EntrenadorId es nullable: una rutina puede existir sin entrenador asignado.
            /// </summary>
            builder.Property(r => r.EntrenadorId);

            builder.Property(r => r.Nombre)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(r => r.Objetivo)
                .HasMaxLength(300);

            builder.Property(r => r.FechaInicio)
                .IsRequired()
                .HasColumnType("date")
                .HasDefaultValueSql("CURRENT_DATE");

            builder.Property(r => r.FechaFin)
                .HasColumnType("date");

            builder.Property(r => r.Activa)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp(0) without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // ── Índices ──────────────────────────────────────────────────────────

            /// <summary>
            /// IX_Rutinas_Socio_Activa — refleja el índice en el script SQL.
            /// Optimiza consultas para "obtener la rutina activa del socio X".
            /// </summary>
            builder.HasIndex(r => new { r.SocioId, r.Activa })
                .HasDatabaseName("IX_Rutinas_Socio_Activa");

            // ── Relaciones ───────────────────────────────────────────────────────

            /// <summary>
            /// Una Rutina → muchas entradas de RutinaEjercicios (borrado en cascada).
            /// Eliminar una rutina elimina todas sus entradas de ejercicios.
            /// </summary>
            builder.HasMany(r => r.RutinaEjercicios)
                .WithOne(re => re.Rutina)
                .HasForeignKey(re => re.RutinaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}