using Gimnasio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gimnasio.Configurations
{
    /// <summary>
    /// Configuración de Fluent API para la entidad <see cref="Ejercicio"/>.
    /// Se mapea a la tabla <c>Ejercicios</c> en PostgreSQL.
    /// </summary>
    public class EjercicioConfiguration : IEntityTypeConfiguration<Ejercicio>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Ejercicio> builder)
        {
            // ── Tabla ─────────────────────────────────────────────────────────────
            builder.ToTable("Ejercicios");

            // ── Clave primaria ────────────────────────────────────────────────────
            builder.HasKey(e => e.EjercicioId);
            builder.Property(e => e.EjercicioId)
                .ValueGeneratedOnAdd();

            // ── Columnas ─────────────────────────────────────────────────────────
            builder.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(e => e.Descripcion)
                .HasMaxLength(400);

            builder.Property(e => e.GrupoMuscular)
                .HasMaxLength(60);

            builder.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // ── Restricciones únicas ─────────────────────────────────────────────
            builder.HasIndex(e => e.Nombre)
                .IsUnique()
                .HasDatabaseName("UQ_Ejercicios_Nombre");

            // ── Relaciones ───────────────────────────────────────────────────────

            /// <summary>
            /// Un Ejercicio → muchas entradas de RutinaEjercicios (borrado en cascada).
            /// Eliminar un ejercicio del catálogo elimina sus registros en rutinas.
            /// En la práctica, se recomienda usar soft-delete mediante IsActive = false.
            /// </summary>
            builder.HasMany(e => e.RutinaEjercicios)
                .WithOne(re => re.Ejercicio)
                .HasForeignKey(re => re.EjercicioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}