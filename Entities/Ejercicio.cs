namespace Gimnasio.Entities
{
    /// <summary>
    /// Entrada de catálogo para un solo ejercicio físico
    /// (por ejemplo, "Sentadilla", "Press Banca").
    /// Se mapea a la tabla <c>Ejercicios</c>.
    /// </summary>
    public class Ejercicio
    {
        /// <summary>
        /// Clave primaria. Generada automáticamente por la base de datos (SERIAL).
        /// </summary>
        public int EjercicioId { get; set; }

        /// <summary>
        /// Nombre único del ejercicio. Sujeto a una restricción de unicidad en la base de datos.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Descripción opcional de cómo realizar correctamente el ejercicio.</summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// Grupo muscular objetivo (por ejemplo, "Piernas", "Pectoral", "Espalda").
        /// Opcional.
        /// </summary>
        public string? GrupoMuscular { get; set; }

        /// <summary>
        /// Indicador de borrado lógico. Establecer en <c>false</c> para retirar un ejercicio
        /// sin eliminarlo de las rutinas existentes.
        /// </summary>
        public bool IsActive { get; set; } = true;
        
        /// <summary>
        /// Filas que vinculan este ejercicio con registros específicos de <see cref="Rutina"/>,
        /// incluyendo series, repeticiones y otros parámetros.
        /// </summary>
        public ICollection<RutinaEjercicio> RutinaEjercicios { get; set; } = new List<RutinaEjercicio>();
    }
}