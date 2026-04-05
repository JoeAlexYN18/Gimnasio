using Gimnasio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gimnasio.Configurations
{
    /// <summary>
    /// Configuración de Fluent API para la entidad de unión <see cref="SocioEntrenador"/>.
    /// Se mapea a la tabla <c>SocioEntrenador</c> en PostgreSQL.
    /// Las relaciones se manejan en <see cref="SocioConfiguration"/> y
    /// <see cref="EntrenadorConfiguration"/>; esta clase solo configura la PK y las columnas.
    /// </summary>
    public class SocioEntrenadorConfiguration : IEntityTypeConfiguration<SocioEntrenador>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<SocioEntrenador> builder)
        {
            // ── Tabla ─────────────────────────────────────────────────────────────
            builder.ToTable("SocioEntrenador");

            // ── Clave primaria compuesta ──────────────────────────────────────────
            /// <summary>
            /// PK compuesta (SocioId, EntrenadorId) refleja la PRIMARY KEY en SQL.
            /// </summary>
            builder.HasKey(se => new { se.SocioId, se.EntrenadorId });

            // ── Columnas ─────────────────────────────────────────────────────────
            builder.Property(se => se.FechaAsignacion)
                .IsRequired()
                .HasColumnType("date")
                .HasDefaultValueSql("CURRENT_DATE");

            builder.Property(se => se.Activo)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}