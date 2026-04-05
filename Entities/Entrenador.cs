namespace Gimnasio.Entities
{
    /// <summary>
    /// Perfil extendido para usuarios que son entrenadores.
    /// Tiene una relación uno-a-uno con <see cref="User"/> a través de <see cref="UserId"/>.
    /// Se mapea a la tabla <c>Entrenadores</c>.
    /// </summary>
    public class Entrenador
    {
        /// <summary>
        /// Clave primaria. Generada automáticamente por la base de datos (SERIAL).
        /// </summary>
        public int EntrenadorId { get; set; }

        /// <summary>
        /// Clave foránea hacia <see cref="User.UserId"/>. Única — garantiza la relación 1:1.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Especialidad principal de entrenamiento (por ejemplo, "Fuerza e hipertrofia").
        /// Campo opcional.
        /// </summary>
        public string? Especialidad { get; set; }

        /// <summary>
        /// Lista de certificaciones profesionales separadas por comas o en texto libre
        /// (por ejemplo, "NSCA-CPT, ACSM"). Campo opcional.
        /// </summary>
        public string? Certificaciones { get; set; }

        /// <summary>
        /// Fecha en que el entrenador se incorporó al personal del gimnasio.
        /// Por defecto es <c>CURRENT_DATE</c> a nivel de base de datos.
        /// </summary>
        public DateOnly FechaIngreso { get; set; }

        /// <summary>
        /// Indicador de borrado lógico / estado laboral.
        /// </summary>
        public bool IsActive { get; set; } = true;

        // ── Navegación ────────────────────────────────────────────────────────────

        /// <summary>La cuenta de autenticación vinculada a este perfil de entrenador.</summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Asignaciones activas o históricas de socios a este entrenador.
        /// </summary>
        public ICollection<SocioEntrenador> SocioEntrenadores { get; set; } = new List<SocioEntrenador>();

        /// <summary>
        /// Rutinas de entrenamiento diseñadas por este entrenador.
        /// </summary>
        public ICollection<Rutina> Rutinas { get; set; } = new List<Rutina>();
    }
}