namespace Gimnasio.DTOs.Auth
{
    /// <summary>
    /// Datos requeridos para iniciar sesión en el sistema.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>Nombre de usuario o correo electrónico.</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>Contraseña en texto plano (se compara contra el hash almacenado).</summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// Respuesta devuelta tras un login exitoso.
    /// Contiene el token JWT y metadata básica del usuario autenticado.
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>Token JWT firmado. Incluir en el header: Authorization: Bearer {Token}.</summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>Fecha y hora UTC de expiración del token.</summary>
        public string Expiration { get; set; } = string.Empty;

        /// <summary>Identificador del usuario autenticado.</summary>
        public int UserId { get; set; }

        /// <summary>Nombre de usuario.</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>Lista de roles asignados al usuario (ADMIN, ENTRENADOR, SOCIO).</summary>
        public List<string> Roles { get; set; } = new();
    }

    /// <summary>
    /// Datos para registrar un nuevo usuario base en el sistema.
    /// Usado internamente por los servicios de registro de socios y entrenadores.
    /// </summary>
    public class RegisterUserDto
    {
        /// <summary>Nombre de usuario único.</summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>Correo electrónico único.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Contraseña en texto plano (el servicio genera el hash).</summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>Teléfono de contacto opcional.</summary>
        public string? PhoneNumber { get; set; }
    }
}