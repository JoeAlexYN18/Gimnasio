namespace Gimnasio.Entities
{
    /// <summary>
    /// Perfil extendido para los miembros del gimnasio (socios).
    /// Tiene una relación uno-a-uno con <see cref="User"/> a través de <see cref="UserId"/>.
    /// Se mapea a la tabla <c>Socios</c>.
    ///
    /// <para><b>Reglas de dominio aplicadas en esta clase (reflejan restricciones CHECK de la BD):</b></para>
    /// <list type="bullet">
    ///   <item>
    ///     <see cref="Genero"/> — debe ser <c>'M'</c>, <c>'F'</c>, <c>'O'</c>, o <c>null</c>.
    ///     Validado por <see cref="ValueObjects.Genero"/>.
    ///   </item>
    ///   <item><see cref="AlturaCm"/> — debe ser ≥ 0 cuando se proporciona.</item>
    ///   <item><see cref="PesoKg"/> — debe ser ≥ 0 cuando se proporciona.</item>
    /// </list>
    /// </summary>
    public class Socio
    {
        // ── Campos privados para propiedades con validación ───────────────────────
        private string?  _genero;
        private decimal? _alturaCm;
        private decimal? _pesoKg;

        // ── Propiedades escalares ─────────────────────────────────────────────────

        /// <summary>Clave primaria. Generada automáticamente por la base de datos (SERIAL).</summary>
        public int SocioId { get; set; }

        /// <summary>
        /// Clave foránea hacia <see cref="User.UserId"/>. Única — garantiza la relación 1:1.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>Fecha de nacimiento. Opcional.</summary>
        public DateOnly? FechaNacimiento { get; set; }

        /// <summary>
        /// Código de género del socio.
        /// <para><b>Regla de dominio:</b> valores aceptados son <c>'M'</c> (Masculino),
        /// <c>'F'</c> (Femenino), <c>'O'</c> (Otro), o <c>null</c> (no especificado).</para>
        /// <para>Refleja: <c>CHECK ("Genero" IN ('M','F','O') OR "Genero" IS NULL)</c>.</para>
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Se lanza cuando el valor no es nulo y no está en ('M','F','O').
        /// </exception>
        public string? Genero
        {
            get => _genero;
            set
            {
                ValueObjects.Genero.Validate(value);
                _genero = value;
            }
        }

        /// <summary>
        /// Altura en centímetros.
        /// <para><b>Regla de dominio:</b> debe ser ≥ 0 cuando se proporciona.</para>
        /// <para>Refleja: <c>CHECK ("AlturaCm" >= 0)</c>.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza cuando se asigna un valor negativo.</exception>
        public decimal? AlturaCm
        {
            get => _alturaCm;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(AlturaCm), value, "AlturaCm debe ser >= 0.");
                _alturaCm = value;
            }
        }

        /// <summary>
        /// Peso en kilogramos.
        /// <para><b>Regla de dominio:</b> debe ser ≥ 0 cuando se proporciona.</para>
        /// <para>Refleja: <c>CHECK ("PesoKg" >= 0)</c>.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza cuando se asigna un valor negativo.</exception>
        public decimal? PesoKg
        {
            get => _pesoKg;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(PesoKg), value, "PesoKg debe ser >= 0.");
                _pesoKg = value;
            }
        }

        /// <summary>Nombre completo de la persona de contacto de emergencia. Opcional.</summary>
        public string? EmergenciaNombre { get; set; }

        /// <summary>Teléfono de contacto de emergencia. Opcional.</summary>
        public string? EmergenciaTelefono { get; set; }

        /// <summary>
        /// Fecha en que el socio se registró en el gimnasio.
        /// Por defecto es <c>CURRENT_DATE</c> a nivel de base de datos.
        /// </summary>
        public DateOnly FechaRegistro { get; set; }

        /// <summary>Indicador de borrado lógico / membresía activa.</summary>
        public bool IsActive { get; set; } = true;

        // ── Navegación ────────────────────────────────────────────────────────────

        /// <summary>La cuenta de autenticación vinculada a este perfil de socio.</summary>
        public User User { get; set; } = null!;

        /// <summary>Asignaciones de entrenadores para este socio (actuales e históricas).</summary>
        public ICollection<SocioEntrenador> SocioEntrenadores { get; set; } = new List<SocioEntrenador>();

        /// <summary>Membresías del socio (actuales e históricas).</summary>
        public ICollection<SocioMembresia> SocioMembresias { get; set; } = new List<SocioMembresia>();

        /// <summary>Registros de asistencia / check-in del socio.</summary>
        public ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();

        /// <summary>Rutinas de entrenamiento asignadas a este socio.</summary>
        public ICollection<Rutina> Rutinas { get; set; } = new List<Rutina>();
    }
}