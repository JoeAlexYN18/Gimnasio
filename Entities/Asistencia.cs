namespace Gimnasio.Entities
{
    /// <summary>
    /// Registra una única visita al gimnasio (entrada/salida) de un socio.
    /// Se mapea a la tabla <c>Asistencias</c>.
    ///
    /// <para><b>Reglas de dominio aplicadas en esta clase (reflejan restricciones CHECK de la BD):</b></para>
    /// <list type="bullet">
    ///   <item>
    ///     <see cref="FechaHoraSalida"/> — cuando no es <c>null</c>, debe ser
    ///     ≥ <see cref="FechaHoraEntrada"/>.
    ///     Refleja: <c>CHECK ("FechaHoraSalida" IS NULL OR "FechaHoraSalida" >= "FechaHoraEntrada")</c>.
    ///   </item>
    /// </list>
    /// </summary>
    public class Asistencia
    {
        // ── Campos privados para propiedades con validación ───────────────────────
        private DateTime  _fechaHoraEntrada;
        private DateTime? _fechaHoraSalida;
        
        /// <summary>Clave primaria. Generada automáticamente por la base de datos (SERIAL).</summary>
        public int AsistenciaId { get; set; }

        /// <summary>Clave foránea que referencia a <see cref="Socio.SocioId"/>.</summary>
        public int SocioId { get; set; }

        /// <summary>
        /// Marca de tiempo UTC de la entrada del socio a la instalación.
        /// Al establecer esta propiedad, se revalida <see cref="FechaHoraSalida"/>
        /// si ya existe un horario de salida.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Se lanza cuando la nueva hora de entrada sería posterior a una hora de salida ya establecida.
        /// </exception>
        public DateTime FechaHoraEntrada
        {
            get => _fechaHoraEntrada;
            set
            {
                if (_fechaHoraSalida.HasValue && value > _fechaHoraSalida.Value)
                    throw new ArgumentException(
                        "FechaHoraEntrada no puede ser posterior a FechaHoraSalida.",
                        nameof(FechaHoraEntrada));
                _fechaHoraEntrada = value;
            }
        }

        /// <summary>
        /// Marca de tiempo UTC de la salida del socio.
        /// <c>null</c> mientras el socio siga dentro (sesión abierta).
        /// <para><b>Regla de dominio:</b> al asignar, debe ser ≥ <see cref="FechaHoraEntrada"/>.</para>
        /// <para>Refleja: <c>CHECK ("FechaHoraSalida" IS NULL OR "FechaHoraSalida" >= "FechaHoraEntrada")</c>.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Se lanza cuando se asigna un valor anterior a <see cref="FechaHoraEntrada"/>.
        /// </exception>
        public DateTime? FechaHoraSalida
        {
            get => _fechaHoraSalida;
            set
            {
                if (value.HasValue && value.Value < _fechaHoraEntrada)
                    throw new ArgumentOutOfRangeException(
                        nameof(FechaHoraSalida), value,
                        "FechaHoraSalida debe ser >= FechaHoraEntrada.");
                _fechaHoraSalida = value;
            }
        }

        /// <summary>Observaciones opcionales en texto libre sobre la visita.</summary>
        public string? Observaciones { get; set; }

        /// <summary>
        /// Clave foránea a <see cref="User.UserId"/> identificando al personal
        /// (recepcionista / administrador) que registró la entrada.
        /// <c>null</c> si el registro fue autoservicio o automatizado.
        /// </summary>
        public int? RegistradaPorUserId { get; set; }

        /// <summary>El socio cuya visita describe este registro.</summary>
        public Socio Socio { get; set; } = null!;

        /// <summary>El usuario del personal que registró esta asistencia. Puede ser <c>null</c>.</summary>
        public User? RegistradaPorUser { get; set; }
    }
}