namespace Gimnasio.DTOs.Socios
{
    /// <summary>
    /// Datos requeridos para registrar un nuevo socio en el gimnasio.
    /// Solo accesible por ADMIN.
    /// </summary>
    public class CrearSocioDto
    {
        /// <summary>Nombre de usuario para la cuenta de autenticación.</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>Correo electrónico del socio.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Contraseña inicial del socio.</summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>Teléfono de contacto opcional.</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Fecha de nacimiento del socio. Opcional.</summary>
        public DateOnly? FechaNacimiento { get; set; }

        /// <summary>
        /// Género del socio. Valores permitidos: 'M', 'F', 'O', o null.
        /// Validado por la capa de dominio (ValueObjects.Genero).
        /// </summary>
        public string? Genero { get; set; }

        /// <summary>Altura en centímetros. Debe ser >= 0 si se proporciona.</summary>
        public decimal? AlturaCm { get; set; }

        /// <summary>Peso en kilogramos. Debe ser >= 0 si se proporciona.</summary>
        public decimal? PesoKg { get; set; }

        /// <summary>Nombre del contacto de emergencia.</summary>
        public string? EmergenciaNombre { get; set; }

        /// <summary>Teléfono del contacto de emergencia.</summary>
        public string? EmergenciaTelefono { get; set; }
    }

    /// <summary>
    /// Datos para actualizar el perfil de un socio existente.
    /// Solo accesible por ADMIN.
    /// </summary>
    public class ActualizarSocioDto
    {
        /// <summary>Nuevo correo electrónico. Opcional.</summary>
        public string? Email { get; set; }

        /// <summary>Nuevo teléfono de contacto. Opcional.</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Género actualizado. Valores: 'M', 'F', 'O', o null.</summary>
        public string? Genero { get; set; }

        /// <summary>Altura actualizada en centímetros.</summary>
        public decimal? AlturaCm { get; set; }

        /// <summary>Peso actualizado en kilogramos.</summary>
        public decimal? PesoKg { get; set; }

        /// <summary>Nombre actualizado del contacto de emergencia.</summary>
        public string? EmergenciaNombre { get; set; }

        /// <summary>Teléfono actualizado del contacto de emergencia.</summary>
        public string? EmergenciaTelefono { get; set; }
    }

    /// <summary>
    /// Respuesta con la información de un socio.
    /// Se devuelve en consultas de listado y detalle.
    /// </summary>
    public class SocioResponseDto
    {
        /// <summary>Identificador del perfil de socio.</summary>
        public int SocioId { get; set; }

        /// <summary>Identificador de la cuenta de usuario.</summary>
        public int UserId { get; set; }

        /// <summary>Nombre de usuario.</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>Correo electrónico.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Teléfono de contacto.</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Fecha de nacimiento.</summary>
        public DateOnly? FechaNacimiento { get; set; }

        /// <summary>Género del socio.</summary>
        public string? Genero { get; set; }

        /// <summary>Altura en centímetros.</summary>
        public decimal? AlturaCm { get; set; }

        /// <summary>Peso en kilogramos.</summary>
        public decimal? PesoKg { get; set; }

        /// <summary>Nombre del contacto de emergencia.</summary>
        public string? EmergenciaNombre { get; set; }

        /// <summary>Teléfono del contacto de emergencia.</summary>
        public string? EmergenciaTelefono { get; set; }

        /// <summary>Fecha de registro en el gimnasio.</summary>
        public DateOnly FechaRegistro { get; set; }

        /// <summary>Indica si el socio está activo.</summary>
        public bool IsActive { get; set; }
    }
}