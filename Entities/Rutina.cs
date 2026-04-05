namespace Gimnasio.Entities
{
    /// <summary>
    /// Plan de entrenamiento con nombre asignado a un socio, opcionalmente creado por un entrenador.
    /// Contiene una lista ordenada de ejercicios a través de <see cref="RutinaEjercicio"/>.
    /// Se mapea a la tabla <c>Rutinas</c>.
    /// </summary>
    public class Rutina
    {
        /// <summary>
        /// Clave primaria. Generada automáticamente por la base de datos (SERIAL).
        /// </summary>
        public int RutinaId { get; set; }

        /// <summary>Clave foránea que referencia a <see cref="Socio.SocioId"/>.</summary>
        public int SocioId { get; set; }

        /// <summary>
        /// Clave foránea que referencia a <see cref="Entrenador.EntrenadorId"/>.
        /// <c>null</c> si la rutina fue creada sin un entrenador (autoasignada).
        /// </summary>
        public int? EntrenadorId { get; set; }

        /// <summary>Nombre descriptivo corto de la rutina (por ejemplo, "Full Body Inicial").</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Objetivo o enfoque del entrenamiento (por ejemplo, "Adaptación neuromuscular", "Pérdida de peso").
        /// Opcional.
        /// </summary>
        public string? Objetivo { get; set; }

        /// <summary>
        /// Fecha desde la cual la rutina es efectiva.
        /// Por defecto <c>CURRENT_DATE</c> a nivel de base de datos.
        /// </summary>
        public DateOnly FechaInicio { get; set; }

        /// <summary>
        /// Fecha de finalización opcional. <c>null</c> significa que la rutina es abierta.
        /// </summary>
        public DateOnly? FechaFin { get; set; }

        /// <summary>
        /// <c>true</c> si esta es la rutina actualmente activa del socio.
        /// </summary>
        public bool Activa { get; set; } = true;

        /// <summary>Marca de tiempo UTC de creación del registro de esta rutina.</summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>El socio al que pertenece esta rutina.</summary>
        public Socio Socio { get; set; } = null!;

        /// <summary>El entrenador que diseñó esta rutina. Puede ser <c>null</c>.</summary>
        public Entrenador? Entrenador { get; set; }

        /// <summary>
        /// Entradas de ejercicios ordenadas que conforman esta rutina.
        /// </summary>
        public ICollection<RutinaEjercicio> RutinaEjercicios { get; set; } = new List<RutinaEjercicio>();
    }
}