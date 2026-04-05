using Gimnasio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gimnasio.Configurations
{
    /// <summary>
    /// Configuración de Fluent API para la entidad join <see cref="UserRole"/>.
    /// Se mapea a la tabla <c>UserRoles</c> en PostgreSQL.
    /// Las relaciones son gestionadas por <see cref="UserConfiguration"/> y
    /// <see cref="RoleConfiguration"/>; esta clase maneja la clave primaria, columnas e índice.
    /// </summary>
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            // ── Tabla ─────────────────────────────────────────────────────────────
            builder.ToTable("UserRoles");

            // ── Clave primaria compuesta ──────────────────────────────────────────
            /// <summary>
            /// Clave primaria compuesta (UserId, RoleId) coincide con PRIMARY KEY (UserId, RoleId)
            /// en el script SQL. EF Core crea tanto la PK como el índice subyacente.
            /// </summary>
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            // ── Columnas ─────────────────────────────────────────────────────────
            builder.Property(ur => ur.AssignedAt)
                .IsRequired()
                .HasColumnType("timestamp(0) without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // ── Índice adicional ─────────────────────────────────────────────────
            /// <summary>
            /// IX_UserRoles_RoleId — refleja el índice en el script SQL.
            /// Mejora el rendimiento de consultas "buscar todos los usuarios en el rol X".
            /// </summary>
            builder.HasIndex(ur => ur.RoleId)
                .HasDatabaseName("IX_UserRoles_RoleId");
        }
    }
}