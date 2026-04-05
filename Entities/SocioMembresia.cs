using Gimnasio.ValueObjects;

namespace Gimnasio.Entities
{
    /// <summary>
    /// Registra un único período de suscripción de membresía para un socio del gimnasio.
    /// Mantiene el historial completo: un socio puede tener múltiples registros a lo largo del tiempo.
    /// Se mapea a la tabla <c>SocioMembresia</c>.
    ///
    /// <para><b>Reglas de dominio aplicadas en esta clase (reflejan restricciones CHECK de la BD):</b></para>
    /// <list type="bullet">
    ///   <item>
    ///     <see cref="Estado"/> — debe ser uno de <c>ACTIVA</c>, <c>VENCIDA</c>,
    ///     <c>PAUSADA</c>, <c>CANCELADA</c>. Validado por <see cref="EstadoMembresia"/>.
    ///   </item>
    ///   <item><see cref="MontoPagado"/> — debe ser ≥ 0.</item>
    /// </list>
    /// </summary>
    public class SocioMembresia
    {
        // ── Campos privados para propiedades con validación ───────────────────────
        private string  _estado      = EstadoMembresia.Activa;
        private decimal _montoPagado;

        // ── Propiedades escalares ─────────────────────────────────────────────────

        /// <summary>Clave primaria sustituta. Generada automáticamente por la base de datos (SERIAL).</summary>
        public int SocioMembresiaId { get; set; }

        /// <summary>Clave foránea que referencia a <see cref="Socio.SocioId"/>.</summary>
        public int SocioId { get; set; }

        /// <summary>Clave foránea que referencia a <see cref="Membresia.MembresiaId"/>.</summary>
        public int MembresiaId { get; set; }

        /// <summary>Fecha en la que inicia este período de suscripción (inclusiva).</summary>
        public DateOnly FechaInicio { get; set; }

        /// <summary>Fecha en la que finaliza este período de suscripción (inclusiva).</summary>
        public DateOnly FechaFin { get; set; }

        /// <summary>
        /// Estado del ciclo de vida de esta suscripción.
        /// <para><b>Regla de dominio:</b> debe ser uno de los valores definidos en
        /// <see cref="EstadoMembresia"/>:
        /// <c>ACTIVA</c>, <c>VENCIDA</c>, <c>PAUSADA</c>, <c>CANCELADA</c>.</para>
        /// <para>Refleja: <c>CHECK ("Estado" IN ('ACTIVA','VENCIDA','PAUSADA','CANCELADA'))</c>.</para>
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Se lanza cuando se asigna un valor no presente en <see cref="EstadoMembresia"/>.
        /// </exception>
        public string Estado
        {
            get => _estado;
            set
            {
                EstadoMembresia.Validate(value);
                _estado = value;
            }
        }

        /// <summary>
        /// Monto realmente pagado por el socio en este período.
        /// Puede diferir de <see cref="Membresia.Precio"/> (promociones, etc.).
        /// <para><b>Regla de dominio:</b> debe ser ≥ 0.</para>
        /// <para>Refleja: <c>CHECK ("MontoPagado" >= 0)</c>.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza cuando se asigna un valor negativo.</exception>
        public decimal MontoPagado
        {
            get => _montoPagado;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(MontoPagado), value, "MontoPagado debe ser >= 0.");
                _montoPagado = value;
            }
        }

        /// <summary>Notas opcionales en texto libre (por ejemplo, "Promoción Black Friday").</summary>
        public string? Notas { get; set; }

        /// <summary>Marca de tiempo UTC cuando se creó este registro de suscripción.</summary>
        public DateTime CreatedAt { get; set; }

        // ── Navegación ────────────────────────────────────────────────────────────

        /// <summary>El socio que posee esta suscripción.</summary>
        public Socio Socio { get; set; } = null!;

        /// <summary>El plan en el que se basa esta suscripción.</summary>
        public Membresia Membresia { get; set; } = null!;
    }
}