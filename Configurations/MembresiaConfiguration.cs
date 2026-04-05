using Gimnasio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gimnasio.Configurations
{
    /// <summary>
    /// Configuración de Fluent API para la entidad <see cref="Membresia"/>.
    /// Se mapea a la tabla <c>Membresias</c> en PostgreSQL.
    /// </summary>
    public class MembresiaConfiguration : IEntityTypeConfiguration<Membresia>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Membresia> builder)
        {
            // ── Tabla ─────────────────────────────────────────────────────────────
            builder.ToTable("Membresias");

            // ── Clave primaria ────────────────────────────────────────────────────
            builder.HasKey(m => m.MembresiaId);
            builder.Property(m => m.MembresiaId)
                .ValueGeneratedOnAdd();

            // ── Columnas ─────────────────────────────────────────────────────────
            builder.Property(m => m.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Descripcion)
                .HasMaxLength(300);

            /// <summary>
            /// DuracionDias debe ser &gt; 0 — se aplica mediante un CHECK en la base de datos.
            /// EF Core no genera constraints CHECK por defecto; la restricción
            /// existe en el script SQL y se valida a nivel de base de datos.
            /// </summary>
            builder.Property(m => m.DuracionDias)
                .IsRequired();

            /// <summary>
            /// Precio usa decimal(10,2). El CHECK Precio >= 0 se encuentra en la base de datos.
            /// </summary>
            builder.Property(m => m.Precio)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(m => m.EsRenovable)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(m => m.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(m => m.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp(0) without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // ── Restricciones únicas ─────────────────────────────────────────────
            builder.HasIndex(m => m.Nombre)
                .IsUnique()
                .HasDatabaseName("UQ_Membresias_Nombre");

            // ── Relaciones ───────────────────────────────────────────────────────

            /// <summary>
            /// Una Membresia → muchas filas de SocioMembresia.
            /// Eliminación restringida: un plan en uso no puede eliminarse
            /// sin antes eliminar o reasignar sus suscripciones.
            /// </summary>
            builder.HasMany(m => m.SocioMembresias)
                .WithOne(sm => sm.Membresia)
                .HasForeignKey(sm => sm.MembresiaId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}