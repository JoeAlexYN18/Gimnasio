using Gimnasio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gimnasio.Configurations
{
    /// <summary>
    /// Configuración de Fluent API para la entidad de unión <see cref="RutinaEjercicio"/>.
    /// Se mapea a la tabla <c>RutinaEjercicios</c> en PostgreSQL.
    /// Las relaciones son gestionadas por <see cref="RutinaConfiguration"/> y
    /// <see cref="EjercicioConfiguration"/>; esta clase maneja la PK, columnas
    /// y el índice de ordenamiento.
    /// </summary>
    public class RutinaEjercicioConfiguration : IEntityTypeConfiguration<RutinaEjercicio>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<RutinaEjercicio> builder)
        {
            // ── Tabla ─────────────────────────────────────────────────────────────
            builder.ToTable("RutinaEjercicios");

            // ── Clave primaria compuesta ──────────────────────────────────────────
            /// <summary>
            /// PK compuesta (RutinaId, EjercicioId) refleja la PRIMARY KEY en SQL.
            /// Un ejercicio solo puede aparecer una vez en una rutina; use Orden para secuenciarlo.
            /// </summary>
            builder.HasKey(re => new { re.RutinaId, re.EjercicioId });

            // ── Columnas ─────────────────────────────────────────────────────────
            /// <summary>
            /// Orden debe ser &gt; 0 — enforceado mediante un CHECK en la base de datos.
            /// </summary>
            builder.Property(re => re.Orden)
                .IsRequired();

            /// <summary>
            /// Todas las columnas de parámetros de entrenamiento son nullable
            /// para soportar ejercicios basados en series/repeticiones o en duración.
            /// Los valores no nulos deben ser &gt; 0 (Series, Repeticiones) o &gt;= 0
            /// (PesoObjetivoKg, DuracionSegundos, DescansoSegundos) — enforceado por CHECK en la base de datos.
            /// </summary>
            builder.Property(re => re.Series);

            builder.Property(re => re.Repeticiones);

            builder.Property(re => re.PesoObjetivoKg)
                .HasColumnType("decimal(6,2)");

            builder.Property(re => re.DuracionSegundos);

            builder.Property(re => re.DescansoSegundos);

            builder.Property(re => re.Notas)
                .HasMaxLength(250);

            // ── Índices ───────────────────────────────────────────────────────────
            /// <summary>
            /// IX_RutinaEjercicios_Rutina_Orden — refleja el índice en el script SQL.
            /// Permite la recuperación eficiente de los ejercicios de una rutina en orden de visualización.
            /// </summary>
            builder.HasIndex(re => new { re.RutinaId, re.Orden })
                .HasDatabaseName("IX_RutinaEjercicios_Rutina_Orden");
        }
    }
}