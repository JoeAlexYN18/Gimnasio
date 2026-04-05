using System.Security.Claims;
using Gimnasio.DTOs;
using Gimnasio.DTOs.Socios;
using Gimnasio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gimnasio.Controllers
{
    /// <summary>
    /// Controlador de socios del gimnasio.
    ///
    /// Permisos por endpoint:
    /// <list type="table">
    ///   <item>POST   /api/socios          → Solo ADMIN (registrar nuevo socio)</item>
    ///   <item>GET    /api/socios          → Solo ADMIN (listar todos)</item>
    ///   <item>GET    /api/socios/{id}     → ADMIN y ENTRENADOR</item>
    ///   <item>GET    /api/socios/mi-perfil→ Solo SOCIO (su propio perfil)</item>
    ///   <item>PUT    /api/socios/{id}     → Solo ADMIN</item>
    ///   <item>DELETE /api/socios/{id}     → Solo ADMIN (soft-delete)</item>
    /// </list>
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class SociosController : ControllerBase
    {
        private readonly ISocioService _socioService;

        public SociosController(ISocioService socioService) => _socioService = socioService;

        /// <summary>
        /// Registra un nuevo socio en el gimnasio.
        /// Crea la cuenta de usuario con rol SOCIO y el perfil extendido.
        /// </summary>
        /// <remarks>
        /// Solo ADMIN puede registrar socios.
        ///
        ///     POST /api/socios
        ///     Authorization: Bearer {token_admin}
        ///     {
        ///         "userName":           "juanperez",
        ///         "email":              "juan@email.com",
        ///         "password":           "Secure123!",
        ///         "phoneNumber":        "+51 999888777",
        ///         "fechaNacimiento":    "1995-06-15",
        ///         "genero":             "M",
        ///         "alturaCm":           175.5,
        ///         "pesoKg":             72.0,
        ///         "emergenciaNombre":   "María Pérez",
        ///         "emergenciaTelefono": "+51 999111222"
        ///     }
        /// </remarks>
        /// <param name="dto">Datos del nuevo socio.</param>
        /// <response code="201">Socio creado exitosamente.</response>
        /// <response code="400">Datos inválidos o nombre de usuario / email ya en uso.</response>
        /// <response code="403">El usuario autenticado no tiene rol ADMIN.</response>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(ApiResponse<SocioResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>),           StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Crear([FromBody] CrearSocioDto dto)
        {
            var socio = await _socioService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = socio.SocioId },
                ApiResponse<SocioResponseDto>.Ok(socio, "Socio registrado exitosamente."));
        }

        /// <summary>
        /// Obtiene la lista paginada de todos los socios registrados.
        /// </summary>
        /// <remarks>
        ///     GET /api/socios?page=1&amp;pageSize=20
        ///     Authorization: Bearer {token_admin}
        /// </remarks>
        /// <param name="paginacion">Parámetros de paginación (page, pageSize).</param>
        /// <response code="200">Lista paginada de socios.</response>
        /// <response code="403">Acceso denegado.</response>
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<SocioResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] PaginacionDto paginacion)
        {
            var result = await _socioService.GetAllAsync(paginacion);
            return Ok(ApiResponse<PagedResponse<SocioResponseDto>>.Ok(result));
        }

        /// <summary>
        /// Obtiene el detalle de un socio por su ID.
        /// ADMIN puede consultar cualquier socio; ENTRENADOR solo los suyos (recomendado usar /mis-socios).
        /// </summary>
        /// <remarks>
        ///     GET /api/socios/5
        ///     Authorization: Bearer {token_admin_o_entrenador}
        /// </remarks>
        /// <param name="id">SocioId del socio a consultar.</param>
        /// <response code="200">Datos del socio.</response>
        /// <response code="404">Socio no encontrado.</response>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        [ProducesResponseType(typeof(ApiResponse<SocioResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),           StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var socio = await _socioService.GetByIdAsync(id);
            return Ok(ApiResponse<SocioResponseDto>.Ok(socio));
        }

        /// <summary>
        /// Retorna el perfil del socio autenticado (solo para rol SOCIO).
        /// El ID se extrae automáticamente del claim userId del JWT.
        /// </summary>
        /// <remarks>
        ///     GET /api/socios/mi-perfil
        ///     Authorization: Bearer {token_socio}
        /// </remarks>
        /// <response code="200">Perfil del socio autenticado.</response>
        /// <response code="404">Perfil no encontrado para el usuario autenticado.</response>
        [HttpGet("mi-perfil")]
        [Authorize(Roles = "SOCIO")]
        [ProducesResponseType(typeof(ApiResponse<SocioResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> MiPerfil()
        {
            var userId = ObtenerUserId();
            var socio  = await _socioService.GetMiPerfilAsync(userId);
            return Ok(ApiResponse<SocioResponseDto>.Ok(socio));
        }

        /// <summary>
        /// Actualiza los datos de un socio existente.
        /// </summary>
        /// <remarks>
        ///     PUT /api/socios/5
        ///     Authorization: Bearer {token_admin}
        ///     {
        ///         "email":   "nuevo@email.com",
        ///         "pesoKg":  75.5,
        ///         "genero":  "M"
        ///     }
        ///
        /// Solo se actualizan los campos enviados (patch semántico parcial).
        /// </remarks>
        /// <param name="id">SocioId del socio a actualizar.</param>
        /// <param name="dto">Campos a actualizar.</param>
        /// <response code="200">Socio actualizado exitosamente.</response>
        /// <response code="400">Datos inválidos.</response>
        /// <response code="404">Socio no encontrado.</response>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(ApiResponse<SocioResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),           StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>),           StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarSocioDto dto)
        {
            var socio = await _socioService.ActualizarAsync(id, dto);
            return Ok(ApiResponse<SocioResponseDto>.Ok(socio, "Socio actualizado exitosamente."));
        }

        /// <summary>
        /// Desactiva (soft-delete) un socio. No elimina el registro de la base de datos.
        /// </summary>
        /// <remarks>
        ///     DELETE /api/socios/5
        ///     Authorization: Bearer {token_admin}
        /// </remarks>
        /// <param name="id">SocioId del socio a desactivar.</param>
        /// <response code="200">Socio desactivado exitosamente.</response>
        /// <response code="404">Socio no encontrado.</response>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Desactivar(int id)
        {
            await _socioService.DesactivarAsync(id);
            return Ok(ApiResponse<object>.Ok(null!, "Socio desactivado exitosamente."));
        }

        /// <summary>Extrae el UserId del claim JWT del usuario autenticado.</summary>
        private int ObtenerUserId() =>
            int.Parse(User.FindFirstValue("userId")
                ?? throw new UnauthorizedAccessException("No se pudo obtener el userId del token."));
    }
}