using Gimnasio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gimnasio.Configurations
{
    /// <summary>
    /// Configuración de Fluent API para la entidad <see cref="Asistencia"/>.
    /// Se mapea a la tabla <c>Asistencias</c> en PostgreSQL.
    /// Las relaciones con <see cref="Socio"/> y <see cref="User"/> se
    /// configuran en <see cref="SocioConfiguration"/> y
    /// <see cref="UserConfiguration"/> respectivamente.
    /// </summary>
    public class AsistenciaConfiguration : IEntityTypeConfiguration<Asistencia>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Asistencia> builder)
        {
            // ── Tabla ─────────────────────────────────────────────────────────────
            builder.ToTable("Asistencias");

            // ── Clave primaria ────────────────────────────────────────────────────
            builder.HasKey(a => a.AsistenciaId);
            builder.Property(a => a.AsistenciaId)
                .ValueGeneratedOnAdd();

            // ── Columnas ─────────────────────────────────────────────────────────
            builder.Property(a => a.SocioId)
                .IsRequired();

            builder.Property(a => a.FechaHoraEntrada)
                .IsRequired()
                .HasColumnType("timestamp(0) without time zone");

            /// <summary>
            /// FechaHoraSalida es nullable — null significa que la sesión sigue abierta.
            /// El CHECK en la base de datos (FechaHoraSalida >= FechaHoraEntrada) garantiza
            /// la integridad cuando hay un valor; esto no se replica en Fluent API de EF Core
            /// porque HasCheckConstraint sólo afecta a migraciones y no a la validación en tiempo de ejecución.
            /// Esta regla debe aplicarse en la capa de servicio antes de guardar.
            /// </summary>
            builder.Property(a => a.FechaHoraSalida)
                .HasColumnType("timestamp(0) without time zone");

            builder.Property(a => a.Observaciones)
                .HasMaxLength(300);

            /// <summary>
            /// RegistradaPorUserId es nullable: null = autoservicio / check-in automatizado.
            /// La FK hacia Users se configura en UserConfiguration con SetNull en delete,
            /// por lo que el historial de asistencias sobrevive a la eliminación de la cuenta de personal.
            /// </summary>
            builder.Property(a => a.RegistradaPorUserId);

            // ── Índices ──────────────────────────────────────────────────────────

            /// <summary>
            /// IX_Asistencias_Socio_Fecha — refleja el índice compuesto en el script SQL.
            /// Optimiza la consulta común: "historial de asistencias del miembro X,
            /// ordenado/filtrado por fecha".
            /// </summary>
            builder.HasIndex(a => new { a.SocioId, a.FechaHoraEntrada })
                .HasDatabaseName("IX_Asistencias_Socio_Fecha");
        }
    }
}