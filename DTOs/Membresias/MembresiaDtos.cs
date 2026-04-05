namespace Gimnasio.DTOs.Membresias
{
    /// <summary>
    /// Datos para crear un nuevo plan de membresía en el catálogo.
    /// Solo accesible por ADMIN.
    /// </summary>
    public class CrearMembresiaDto
    {
        /// <summary>Nombre único del plan (e.g. "Basic", "Plus", "Premium").</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Descripción del plan. Opcional.</summary>
        public string? Descripcion { get; set; }

        /// <summary>Duración del plan en días. Debe ser mayor a 0.</summary>
        public int DuracionDias { get; set; }

        /// <summary>Precio de lista del plan. Debe ser >= 0.</summary>
        public decimal Precio { get; set; }

        /// <summary>Indica si el plan puede renovarse al vencer.</summary>
        public bool EsRenovable { get; set; } = true;
    }

    /// <summary>
    /// Datos para actualizar un plan de membresía existente.
    /// Solo accesible por ADMIN.
    /// </summary>
    public class ActualizarMembresiaDto
    {
        /// <summary>Nueva descripción del plan.</summary>
        public string? Descripcion { get; set; }

        /// <summary>Nueva duración en días. Debe ser mayor a 0 si se proporciona.</summary>
        public int? DuracionDias { get; set; }

        /// <summary>Nuevo precio. Debe ser >= 0 si se proporciona.</summary>
        public decimal? Precio { get; set; }

        /// <summary>Actualiza si el plan es renovable.</summary>
        public bool? EsRenovable { get; set; }

        /// <summary>Activa o desactiva el plan en el catálogo.</summary>
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// Respuesta con la información de un plan de membresía.
    /// </summary>
    public class MembresiaResponseDto
    {
        /// <summary>Identificador del plan.</summary>
        public int MembresiaId { get; set; }

        /// <summary>Nombre del plan.</summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>Descripción del plan.</summary>
        public string? Descripcion { get; set; }

        /// <summary>Duración en días.</summary>
        public int DuracionDias { get; set; }

        /// <summary>Precio de lista.</summary>
        public decimal Precio { get; set; }

        /// <summary>Si el plan permite renovación.</summary>
        public bool EsRenovable { get; set; }

        /// <summary>Si el plan está activo en el catálogo.</summary>
        public bool IsActive { get; set; }

        /// <summary>Fecha de creación del plan.</summary>
        public string CreatedAt { get; set; } = string.Empty;
    }

    /// <summary>
    /// Datos para asignar una membresía a un socio.
    /// Solo accesible por ADMIN.
    /// </summary>
    public class AsignarMembresiaDto
    {
        /// <summary>Identificador del socio al que se asigna la membresía.</summary>
        public int SocioId { get; set; }

        /// <summary>Identificador del plan de membresía a asignar.</summary>
        public int MembresiaId { get; set; }

        /// <summary>Fecha de inicio de la suscripción.</summary>
        public DateOnly FechaInicio { get; set; }

        /// <summary>
        /// Monto efectivamente pagado por el socio.
        /// Puede diferir del precio de lista (promociones, descuentos).
        /// Debe ser >= 0.
        /// </summary>
        public decimal MontoPagado { get; set; }

        /// <summary>Notas adicionales (e.g. "Promoción Black Friday"). Opcional.</summary>
        public string? Notas { get; set; }
    }

    /// <summary>
    /// Respuesta con la información de una suscripción de membresía de un socio.
    /// </summary>
    public class SocioMembresiaResponseDto
    {
        /// <summary>Identificador de la suscripción.</summary>
        public int SocioMembresiaId { get; set; }

        /// <summary>Identificador del socio.</summary>
        public int SocioId { get; set; }

        /// <summary>Nombre del plan contratado.</summary>
        public string NombreMembresia { get; set; } = string.Empty;

        /// <summary>Fecha de inicio de la suscripción.</summary>
        public DateOnly FechaInicio { get; set; }

        /// <summary>Fecha de vencimiento de la suscripción.</summary>
        public DateOnly FechaFin { get; set; }

        /// <summary>Estado actual: ACTIVA, VENCIDA, PAUSADA, CANCELADA.</summary>
        public string Estado { get; set; } = string.Empty;

        /// <summary>Monto pagado por el socio.</summary>
        public decimal MontoPagado { get; set; }

        /// <summary>Notas de la suscripción.</summary>
        public string? Notas { get; set; }
    }
}