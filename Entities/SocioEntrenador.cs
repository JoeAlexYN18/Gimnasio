namespace Gimnasio.Entities
{
    /// <summary>
    /// Entidad de unión para la relación muchos-a-muchos entre
    /// <see cref="Socio"/> y <see cref="Entrenador"/>.
    /// Permite rastrear asignaciones activas e históricas de entrenadores para cada socio.
    /// Se mapea a la tabla <c>SocioEntrenador</c>.
    /// La clave primaria compuesta es (<see cref="SocioId"/>, <see cref="EntrenadorId"/>).
    /// </summary>
    public class SocioEntrenador
    {
        /// <summary>
        /// Clave foránea que referencia a <see cref="Socio.SocioId"/>.
        /// Parte de la clave primaria compuesta.
        /// </summary>
        public int SocioId { get; set; }

        /// <summary>
        /// Clave foránea que referencia a <see cref="Entrenador.EntrenadorId"/>.
        /// Parte de la clave primaria compuesta.
        /// </summary>
        public int EntrenadorId { get; set; }

        /// <summary>
        /// Fecha en la que el entrenador fue asignado al socio.
        /// Por defecto es <c>CURRENT_DATE</c> a nivel de base de datos.
        /// </summary>
        public DateOnly FechaAsignacion { get; set; }

        /// <summary>
        /// <c>true</c> si la asignación está actualmente activa;
        /// <c>false</c> si ha finalizado pero se mantiene para historial.
        /// </summary>
        public bool Activo { get; set; } = true;

        // ── Navegación ────────────────────────────────────────────────────────────

        /// <summary>El socio en esta asignación.</summary>
        public Socio Socio { get; set; } = null!;

        /// <summary>El entrenador en esta asignación.</summary>
        public Entrenador Entrenador { get; set; } = null!;
    }
}