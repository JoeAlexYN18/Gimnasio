namespace Gimnasio.Entities
{
    /// <summary>
    /// Cuenta de autenticación para cada persona en el sistema
    /// (administradores, entrenadores y socios).
    /// Se mapea a la tabla <c>Users</c>.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Clave primaria. Generada automáticamente por la base de datos (SERIAL).
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Nombre de usuario único para iniciar sesión (por ejemplo, "jsmith").
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Versión en mayúsculas de <see cref="UserName"/> usada para
        /// comparaciones sin distinción de mayúsculas/minúsculas. Sujeta a una restricción de unicidad.
        /// </summary>
        public string NormalizedUserName { get; set; } = string.Empty;

        /// <summary>
        /// Dirección de correo electrónico de contacto. Sujeta a una restricción de unicidad.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Versión en mayúsculas de <see cref="Email"/>. Sujeta a una restricción de unicidad.
        /// </summary>
        public string NormalizedEmail { get; set; } = string.Empty;

        /// <summary>
        /// Cadena de contraseña hasheada generada por el backend
        /// (por ejemplo, BCrypt / Argon2 / PBKDF2). Nunca almacenar texto plano aquí.
        /// </summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Número de teléfono de contacto opcional.
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Indicador de borrado lógico / cuenta habilitada.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Marca de tiempo UTC del inicio de sesión exitoso más reciente del usuario.
        /// <c>null</c> si el usuario nunca ha iniciado sesión.
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Marca de tiempo UTC cuando se insertó el registro.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Marca de tiempo UTC de la última actualización del perfil. <c>null</c> hasta la primera edición.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // ── Navegación ────────────────────────────────────────────────────────────

        /// <summary>Registros de unión que vinculan este usuario con los <see cref="Role"/> asignados.</summary>
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        /// <summary>
        /// Perfil de entrenador asociado a esta cuenta, si el usuario es un entrenador.
        /// <c>null</c> para administradores y socios.
        /// </summary>
        public Entrenador? Entrenador { get; set; }

        /// <summary>
        /// Perfil de socio asociado a esta cuenta, si el usuario es un socio.
        /// <c>null</c> para administradores y entrenadores.
        /// </summary>
        public Socio? Socio { get; set; }

        /// <summary>
        /// Registros de asistencia registrados por este usuario (por ejemplo, recepcionista / administrador).
        /// </summary>
        public ICollection<Asistencia> AsistenciasRegistradas { get; set; } = new List<Asistencia>();
    }
}