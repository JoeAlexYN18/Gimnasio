using Gimnasio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gimnasio.Configurations
{
    /// <summary>
    /// Configuración de Fluent API para la entidad <see cref="SocioMembresia"/>.
    /// Se mapea a la tabla <c>SocioMembresia</c> en PostgreSQL.
    /// Las relaciones son gestionadas por <see cref="SocioConfiguration"/> y
    /// <see cref="MembresiaConfiguration"/>; esta clase maneja la clave primaria, las columnas
    /// y los índices adicionales únicamente.
    /// </summary>
    public class SocioMembresiaConfiguration : IEntityTypeConfiguration<SocioMembresia>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<SocioMembresia> builder)
        {
            // ── Tabla ─────────────────────────────────────────────────────────────
            builder.ToTable("SocioMembresia");

            // ── Clave primaria ────────────────────────────────────────────────────
            /// <summary>
            /// Clave primaria surrogate entera (SERIAL) — no es una clave compuesta,
            /// porque un miembro puede suscribirse al mismo plan varias veces
            /// en diferentes periodos.
            /// </summary>
            builder.HasKey(sm => sm.SocioMembresiaId);
            builder.Property(sm => sm.SocioMembresiaId)
                .ValueGeneratedOnAdd();

            // ── Columnas ─────────────────────────────────────────────────────────
            builder.Property(sm => sm.SocioId)
                .IsRequired();

            builder.Property(sm => sm.MembresiaId)
                .IsRequired();

            builder.Property(sm => sm.FechaInicio)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(sm => sm.FechaFin)
                .IsRequired()
                .HasColumnType("date");

            /// <summary>
            /// Estado es una columna VARCHAR(20) con CHECK a nivel de base de datos.
            /// Valores válidos: 'ACTIVA', 'VENCIDA', 'PAUSADA', 'CANCELADA'.
            /// EF Core no lo aplica a nivel de modelo; la validación se realiza
            /// en la capa de servicio o dominio de la aplicación.
            /// </summary>
            builder.Property(sm => sm.Estado)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(sm => sm.MontoPagado)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(sm => sm.Notas)
                .HasMaxLength(300);

            builder.Property(sm => sm.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp(0) without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // ── Índices ───────────────────────────────────────────────────────────

            /// <summary>
            /// IX_SocioMembresia_Socio — refleja el índice en el script SQL.
            /// Acelera consultas como "todas las suscripciones del miembro X".
            /// </summary>
            builder.HasIndex(sm => sm.SocioId)
                .HasDatabaseName("IX_SocioMembresia_Socio");

            /// <summary>
            /// IX_SocioMembresia_Estado_FechaFin — refleja el índice compuesto en el script SQL.
            /// Optimiza consultas que filtran por estado y fecha de vencimiento
            /// (ej. "encontrar todas las membresías ACTIVA que vencen antes de hoy").
            /// </summary>
            builder.HasIndex(sm => new { sm.Estado, sm.FechaFin })
                .HasDatabaseName("IX_SocioMembresia_Estado_FechaFin");
        }
    }
}