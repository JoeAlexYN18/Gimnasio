namespace Gimnasio.Entities
{
    /// <summary>
    /// Entidad de unión para la relación muchos a muchos entre
    /// <see cref="Rutina"/> y <see cref="Ejercicio"/>.
    /// Almacena parámetros de entrenamiento (series, repeticiones, peso, duración, descanso).
    /// Se mapea a la tabla <c>RutinaEjercicios</c>.
    /// La clave primaria compuesta es (<see cref="RutinaId"/>, <see cref="EjercicioId"/>).
    ///
    /// <para><b>Reglas de dominio aplicadas en esta clase (reflejan CHECKs de la DB):</b></para>
    /// <list type="bullet">
    ///   <item><see cref="Orden"/> — debe ser &gt; 0.</item>
    ///   <item><see cref="Series"/> — debe ser &gt; 0 cuando se proporcione.</item>
    ///   <item><see cref="Repeticiones"/> — debe ser &gt; 0 cuando se proporcione.</item>
    ///   <item><see cref="PesoObjetivoKg"/> — debe ser ≥ 0 cuando se proporcione.</item>
    ///   <item><see cref="DuracionSegundos"/> — debe ser ≥ 0 cuando se proporcione.</item>
    ///   <item><see cref="DescansoSegundos"/> — debe ser ≥ 0 cuando se proporcione.</item>
    /// </list>
    /// </summary>
    public class RutinaEjercicio
    {
        // ── Campos privados para propiedades validadas ─────────────────────────────
        private int _orden;
        private int? _series;
        private int? _repeticiones;
        private decimal? _pesoObjetivoKg;
        private int? _duracionSegundos;
        private int? _descansoSegundos;

        // ── Propiedades de clave compuesta (FK) ───────────────────────────────────
        public int RutinaId { get; set; }
        public int EjercicioId { get; set; }

        // ── Propiedades validadas ──────────────────────────────────────────────────

        public int Orden
        {
            get => _orden;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(Orden), value, "Orden must be > 0.");
                _orden = value;
            }
        }

        public int? Series
        {
            get => _series;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(Series), value, "Series must be > 0 when provided.");
                _series = value;
            }
        }

        public int? Repeticiones
        {
            get => _repeticiones;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(Repeticiones), value, "Repeticiones must be > 0 when provided.");
                _repeticiones = value;
            }
        }

        public decimal? PesoObjetivoKg
        {
            get => _pesoObjetivoKg;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(PesoObjetivoKg), value, "PesoObjetivoKg must be >= 0 when provided.");
                _pesoObjetivoKg = value;
            }
        }

        public int? DuracionSegundos
        {
            get => _duracionSegundos;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(DuracionSegundos), value, "DuracionSegundos must be >= 0 when provided.");
                _duracionSegundos = value;
            }
        }

        public int? DescansoSegundos
        {
            get => _descansoSegundos;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(DescansoSegundos), value, "DescansoSegundos must be >= 0 when provided.");
                _descansoSegundos = value;
            }
        }

        public string? Notas { get; set; }

        // ── Navegación ────────────────────────────────────────────────────────────
        public Rutina Rutina { get; set; } = null!;
        public Ejercicio Ejercicio { get; set; } = null!;
    }
}