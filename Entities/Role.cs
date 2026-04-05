namespace Gimnasio.Entities
{
    /// <summary>
    /// Representa un rol del sistema utilizado para autorización.
    /// Se mapea a la tabla <c>Roles</c> en la base de datos.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Clave primaria. Generada automáticamente por la base de datos (SERIAL / IDENTITY).
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Nombre del rol legible para humanos (por ejemplo, "ADMIN", "SOCIO").
        /// Sujeto a una restricción de unicidad en la base de datos.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Versión normalizada en mayúsculas de <see cref="Name"/> usada para
        /// búsquedas sin distinción de mayúsculas/minúsculas. Sujeta a una restricción de unicidad.
        /// </summary>
        public string NormalizedName { get; set; } = string.Empty;

        /// <summary>
        /// Indicador de borrado lógico. <c>true</c> = el rol está disponible para asignación.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Marca de tiempo UTC de cuando se creó este rol.
        /// Por defecto es <c>CURRENT_TIMESTAMP</c> a nivel de base de datos.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Registros de unión que vinculan este rol con registros de <see cref="User"/>.
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}