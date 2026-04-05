namespace Gimnasio.Entities
{
    /// <summary>
    /// Entrada de catálogo para un plan de membresía ofrecido por el gimnasio
    /// (por ejemplo, "Basic", "Plus", "Premium").
    /// Se mapea a la tabla <c>Membresias</c>.
    ///
    /// <para><b>Reglas de dominio aplicadas en esta clase (reflejan restricciones CHECK de la BD):</b></para>
    /// <list type="bullet">
    ///   <item><see cref="DuracionDias"/> — debe ser &gt; 0.</item>
    ///   <item><see cref="Precio"/> — debe ser ≥ 0.</item>
    /// </list>
    /// </summary>
    public class Membresia
    {
        // ── Campos privados para propiedades con validación ───────────────────────
        private int     _duracionDias;
        private decimal _precio;

        // ── Propiedades escalares ─────────────────────────────────────────────────

        /// <summary>Clave primaria. Generada automáticamente por la base de datos (SERIAL).</summary>
        public int MembresiaId { get; set; }

        /// <summary>
        /// Nombre del plan único y legible para humanos (por ejemplo, "Plus").
        /// Sujeto a una restricción de unicidad en la base de datos.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Descripción opcional de marketing u operativa de lo que incluye el plan.</summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// Duración del plan en días.
        /// <para><b>Regla de dominio:</b> debe ser &gt; 0.</para>
        /// <para>Refleja: <c>CHECK ("DuracionDias" > 0)</c>.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza cuando es cero o negativo.</exception>
        public int DuracionDias
        {
            get => _duracionDias;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(DuracionDias), value, "DuracionDias debe ser > 0.");
                _duracionDias = value;
            }
        }

        /// <summary>
        /// Precio de lista de este plan.
        /// <para><b>Regla de dominio:</b> debe ser ≥ 0.</para>
        /// <para>Refleja: <c>CHECK ("Precio" >= 0)</c>.</para>
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Se lanza cuando es un valor negativo.</exception>
        public decimal Precio
        {
            get => _precio;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Precio), value, "Precio debe ser >= 0.");
                _precio = value;
            }
        }

        /// <summary>Indica si los socios pueden renovar este plan al vencer.</summary>
        public bool EsRenovable { get; set; } = true;

        /// <summary>
        /// Indicador de borrado lógico. Establecer en <c>false</c> para retirar un plan sin
        /// eliminar sus registros históricos de suscripciones.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>Marca de tiempo UTC cuando el plan fue creado en el catálogo.</summary>
        public DateTime CreatedAt { get; set; }

        // ── Navegación ────────────────────────────────────────────────────────────

        /// <summary>Todas las membresías de socios (actuales e históricas) vinculadas a este plan.</summary>
        public ICollection<SocioMembresia> SocioMembresias { get; set; } = new List<SocioMembresia>();
    }
}