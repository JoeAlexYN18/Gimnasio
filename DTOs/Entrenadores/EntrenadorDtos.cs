namespace Gimnasio.DTOs.Entrenadores
{
    /// <summary>
    /// Datos requeridos para registrar un nuevo entrenador.
    /// Solo accesible por ADMIN.
    /// </summary>
    public class CrearEntrenadorDto
    {
        /// <summary>Nombre de usuario para la cuenta de autenticación.</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>Correo electrónico del entrenador.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Contraseña inicial.</summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>Teléfono de contacto. Opcional.</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Especialidad del entrenador (e.g. "Fuerza e Hipertrofia").</summary>
        public string? Especialidad { get; set; }

        /// <summary>Certificaciones profesionales (e.g. "NSCA-CPT, ACSM").</summary>
        public string? Certificaciones { get; set; }
    }

    /// <summary>
    /// Datos para actualizar el perfil de un entrenador.
    /// Solo accesible por ADMIN.
    /// </summary>
    public class ActualizarEntrenadorDto
    {
        /// <summary>Especialidad actualizada.</summary>
        public string? Especialidad { get; set; }

        /// <summary>Certificaciones actualizadas.</summary>
        public string? Certificaciones { get; set; }

        /// <summary>Nuevo teléfono de contacto.</summary>
        public string? PhoneNumber { get; set; }
    }

    /// <summary>
    /// Respuesta con la información de un entrenador.
    /// </summary>
    public class EntrenadorResponseDto
    {
        /// <summary>Identificador del perfil de entrenador.</summary>
        public int EntrenadorId { get; set; }

        /// <summary>Identificador de la cuenta de usuario.</summary>
        public int UserId { get; set; }

        /// <summary>Nombre de usuario.</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>Correo electrónico.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Teléfono de contacto.</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Especialidad del entrenador.</summary>
        public string? Especialidad { get; set; }

        /// <summary>Certificaciones del entrenador.</summary>
        public string? Certificaciones { get; set; }

        /// <summary>Fecha de ingreso al staff del gimnasio.</summary>
        public DateOnly FechaIngreso { get; set; }

        /// <summary>Indica si el entrenador está activo.</summary>
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Datos para asignar un entrenador a un socio.
    /// Solo accesible por ADMIN.
    /// </summary>
    public class AsignarEntrenadorDto
    {
        /// <summary>Identificador del socio al que se asigna el entrenador.</summary>
        public int SocioId { get; set; }

        /// <summary>Identificador del entrenador a asignar.</summary>
        public int EntrenadorId { get; set; }
    }
}