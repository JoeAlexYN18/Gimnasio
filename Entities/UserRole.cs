namespace Gimnasio.Entities
{
    /// <summary>
    /// Entidad de unión para la relación muchos-a-muchos entre
    /// <see cref="User"/> y <see cref="Role"/>.
    /// Se mapea a la tabla <c>UserRoles</c>.
    /// La clave primaria compuesta es (<see cref="UserId"/>, <see cref="RoleId"/>).
    /// </summary>
    public class UserRole
    {
        /// <summary>
        /// Clave foránea que referencia a <see cref="User.UserId"/>.
        /// Parte de la clave primaria compuesta.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Clave foránea que referencia a <see cref="Role.RoleId"/>.
        /// Parte de la clave primaria compuesta.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Marca de tiempo UTC de cuando el rol fue asignado al usuario.
        /// </summary>
        public DateTime AssignedAt { get; set; }

        // ── Navegación ────────────────────────────────────────────────────────────

        /// <summary>El usuario que posee este rol.</summary>
        public User User { get; set; } = null!;

        /// <summary>El rol asignado al usuario.</summary>
        public Role Role { get; set; } = null!;
    }
}