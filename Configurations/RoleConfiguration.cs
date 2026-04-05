using Gimnasio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gimnasio.Configurations
{
    /// <summary>
    /// Configuración Fluent API para la entidad <see cref="Role"/>.
    /// Se mapea a la tabla <c>Roles</c> en PostgreSQL.
    /// </summary>
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // ── Tabla ─────────────────────────────────────────────────────────────
            builder.ToTable("Roles");

            // ── Clave primaria ───────────────────────────────────────────────────
            /// <summary>
            /// Columna SERIAL. EF Core usa ValueGeneratedOnAdd por defecto
            /// para PKs enteras, lo cual coincide con PostgreSQL SERIAL / GENERATED ALWAYS AS IDENTITY.
            /// </summary>
            builder.HasKey(r => r.RoleId);
            builder.Property(r => r.RoleId)
                .ValueGeneratedOnAdd();

            // ── Columnas ─────────────────────────────────────────────────────────
            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.NormalizedName)
                .IsRequired()
                .HasMaxLength(50);

            /// <summary>
            /// Columna BOOLEAN; EF Core mapea C# bool a PostgreSQL bool nativamente.
            /// HasDefaultValue refleja la restricción DEFAULT TRUE en SQL.
            /// </summary>
            builder.Property(r => r.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            /// <summary>
            /// TIMESTAMP(0) — precisión de segundos fraccionarios = 0.
            /// HasDefaultValueSql refleja DEFAULT CURRENT_TIMESTAMP en la DB.
            /// La aplicación también puede establecerlo antes de guardar para pruebas.
            /// </summary>
            builder.Property(r => r.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp(0) without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // ── Restricciones de unicidad ─────────────────────────────────────────
            builder.HasIndex(r => r.Name)
                .IsUnique()
                .HasDatabaseName("UQ_Roles_Name");

            builder.HasIndex(r => r.NormalizedName)
                .IsUnique()
                .HasDatabaseName("UQ_Roles_NormalizedName");
        }
    }
}
