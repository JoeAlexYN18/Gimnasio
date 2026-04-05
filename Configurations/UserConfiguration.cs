using Gimnasio.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gimnasio.Configurations
{
    /// <summary>
    /// Configuración de Fluent API para la entidad <see cref="User"/>.
    /// Se mapea a la tabla <c>Users</c> en PostgreSQL.
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // ── Tabla ─────────────────────────────────────────────────────────────
            builder.ToTable("Users");

            // ── Clave primaria ───────────────────────────────────────────────────
            builder.HasKey(u => u.UserId);
            builder.Property(u => u.UserId)
                .ValueGeneratedOnAdd();

            // ── Columnas ─────────────────────────────────────────────────────────
            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.NormalizedUserName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(u => u.NormalizedEmail)
                .IsRequired()
                .HasMaxLength(256);

            /// <summary>
            /// PasswordHash almacena el resultado de un algoritmo de hashing fuerte (BCrypt, Argon2…).
            /// La longitud máxima 512 coincide con la definición de columna en la base de datos.
            /// </summary>
            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(25);

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.LastLoginAt)
                .HasColumnType("timestamp(0) without time zone");

            builder.Property(u => u.CreatedAt)
                .IsRequired()
                .HasColumnType("timestamp(0) without time zone")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(u => u.UpdatedAt)
                .HasColumnType("timestamp(0) without time zone");

            // ── Restricciones únicas ─────────────────────────────────────────────
            builder.HasIndex(u => u.UserName)
                .IsUnique()
                .HasDatabaseName("UQ_Users_UserName");

            builder.HasIndex(u => u.NormalizedUserName)
                .IsUnique()
                .HasDatabaseName("UQ_Users_NormalizedUserName");

            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("UQ_Users_Email");

            builder.HasIndex(u => u.NormalizedEmail)
                .IsUnique()
                .HasDatabaseName("UQ_Users_NormalizedEmail");

            // ── Relaciones ───────────────────────────────────────────────────────

            /// <summary>
            /// Un Usuario → muchos UserRoles (el borrado en cascada refleja ON DELETE CASCADE en SQL).
            /// El inverso es UserRole.User.
            /// </summary>
            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            /// <summary>
            /// Un Usuario → cero o un perfil de Entrenador (1:1).
            /// Borrado en cascada: eliminar un Usuario también elimina el perfil de entrenador.
            /// </summary>
            builder.HasOne(u => u.Entrenador)
                .WithOne(e => e.User)
                .HasForeignKey<Entrenador>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            /// <summary>
            /// Un Usuario → cero o un perfil de Socio (1:1).
            /// Borrado en cascada: eliminar un Usuario también elimina el perfil de socio.
            /// </summary>
            builder.HasOne(u => u.Socio)
                .WithOne(s => s.User)
                .HasForeignKey<Socio>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            /// <summary>
            /// Un Usuario (staff) → muchas Asistencias registradas por ese usuario.
            /// Sin cascada: eliminar una cuenta de staff no debe borrar el historial de asistencias.
            /// La columna FK es nullable (RegistradaPorUserId).
            /// </summary>
            builder.HasMany(u => u.AsistenciasRegistradas)
                .WithOne(a => a.RegistradaPorUser)
                .HasForeignKey(a => a.RegistradaPorUserId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}