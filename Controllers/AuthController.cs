using Gimnasio.DTOs;
using Gimnasio.DTOs.Auth;
using Gimnasio.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gimnasio.Controllers
{
    /// <summary>
    /// Controlador de autenticación.
    /// Gestiona el login y la emisión de tokens JWT.
    /// No requiere autenticación previa (endpoint público).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        /// <summary>
        /// Inicia sesión con credenciales y retorna un token JWT.
        /// </summary>
        /// <remarks>
        /// Ejemplo de request:
        ///
        ///     POST /api/auth/login
        ///     {
        ///         "userName": "admin",
        ///         "password": "Admin123!"
        ///     }
        ///
        /// El token devuelto debe incluirse en el header de las demás peticiones:
        ///     Authorization: Bearer {token}
        /// </remarks>
        /// <param name="dto">Credenciales del usuario (userName + password).</param>
        /// <returns>Token JWT, fecha de expiración y roles del usuario.</returns>
        /// <response code="200">Login exitoso. Retorna el token JWT.</response>
        /// <response code="401">Credenciales incorrectas o cuenta inactiva.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),           StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(ApiResponse<LoginResponseDto>.Ok(result, "Inicio de sesión exitoso."));
        }
    }
}