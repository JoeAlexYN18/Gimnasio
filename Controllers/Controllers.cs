using System.Security.Claims;
using Gimnasio.DTOs;
using Gimnasio.DTOs.Asistencias;
using Gimnasio.DTOs.Entrenadores;
using Gimnasio.DTOs.Membresias;
using Gimnasio.DTOs.Socios;
using Gimnasio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gimnasio.Controllers
{
    // =============================================================================
    // EntrenadoresController
    // =============================================================================

    /// <summary>
    /// Controlador de entrenadores del gimnasio.
    ///
    /// Permisos por endpoint:
    /// <list type="table">
    ///   <item>POST /api/entrenadores              → Solo ADMIN</item>
    ///   <item>GET  /api/entrenadores              → Solo ADMIN</item>
    ///   <item>GET  /api/entrenadores/{id}         → ADMIN y ENTRENADOR</item>
    ///   <item>PUT  /api/entrenadores/{id}         → Solo ADMIN</item>
    ///   <item>POST /api/entrenadores/asignar      → Solo ADMIN</item>
    ///   <item>GET  /api/entrenadores/mis-socios   → Solo ENTRENADOR</item>
    /// </list>
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class EntrenadoresController : ControllerBase
    {
        private readonly IEntrenadorService _service;
        public EntrenadoresController(IEntrenadorService service) => _service = service;

        /// <summary>
        /// Registra un nuevo entrenador en el sistema.
        /// </summary>
        /// <remarks>
        ///     POST /api/entrenadores
        ///     Authorization: Bearer {token_admin}
        ///     {
        ///         "userName":        "carlos_trainer",
        ///         "email":           "carlos@gym.com",
        ///         "password":        "Trainer123!",
        ///         "especialidad":    "Fuerza e Hipertrofia",
        ///         "certificaciones": "NSCA-CPT, ACSM"
        ///     }
        /// </remarks>
        /// <response code="201">Entrenador registrado exitosamente.</response>
        /// <response code="400">Datos inválidos o usuario ya existente.</response>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(ApiResponse<EntrenadorResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>),                StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Crear([FromBody] CrearEntrenadorDto dto)
        {
            var result = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.EntrenadorId },
                ApiResponse<EntrenadorResponseDto>.Ok(result, "Entrenador registrado exitosamente."));
        }

        /// <summary>
        /// Lista todos los entrenadores con paginación.
        /// </summary>
        /// <remarks>
        ///     GET /api/entrenadores?page=1&amp;pageSize=10
        ///     Authorization: Bearer {token_admin}
        /// </remarks>
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<EntrenadorResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] PaginacionDto paginacion)
        {
            var result = await _service.GetAllAsync(paginacion);
            return Ok(ApiResponse<PagedResponse<EntrenadorResponseDto>>.Ok(result));
        }

        /// <summary>
        /// Obtiene el detalle de un entrenador por su ID.
        /// </summary>
        /// <remarks>
        ///     GET /api/entrenadores/3
        ///     Authorization: Bearer {token_admin_o_entrenador}
        /// </remarks>
        /// <param name="id">EntrenadorId del entrenador.</param>
        [HttpGet("{id:int}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        [ProducesResponseType(typeof(ApiResponse<EntrenadorResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),                StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(ApiResponse<EntrenadorResponseDto>.Ok(result));
        }

        /// <summary>
        /// Actualiza los datos de un entrenador.
        /// </summary>
        /// <remarks>
        ///     PUT /api/entrenadores/3
        ///     Authorization: Bearer {token_admin}
        ///     {
        ///         "especialidad":    "Cardio y Resistencia",
        ///         "certificaciones": "ACSM, CrossFit L2"
        ///     }
        /// </remarks>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(ApiResponse<EntrenadorResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),                StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarEntrenadorDto dto)
        {
            var result = await _service.ActualizarAsync(id, dto);
            return Ok(ApiResponse<EntrenadorResponseDto>.Ok(result, "Entrenador actualizado."));
        }

        /// <summary>
        /// Asigna un entrenador a un socio. Crea la relación N:N SocioEntrenador.
        /// </summary>
        /// <remarks>
        ///     POST /api/entrenadores/asignar
        ///     Authorization: Bearer {token_admin}
        ///     {
        ///         "socioId":      5,
        ///         "entrenadorId": 2
        ///     }
        /// </remarks>
        /// <response code="200">Asignación realizada exitosamente.</response>
        [HttpPost("asignar")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AsignarSocio([FromBody] AsignarEntrenadorDto dto)
        {
            await _service.AsignarSocioAsync(dto);
            return Ok(ApiResponse<object>.Ok(null!, "Entrenador asignado al socio exitosamente."));
        }

        /// <summary>
        /// Retorna la lista de socios asignados al entrenador autenticado.
        /// El ID del entrenador se resuelve desde el claim userId del JWT.
        /// </summary>
        /// <remarks>
        ///     GET /api/entrenadores/mis-socios
        ///     Authorization: Bearer {token_entrenador}
        /// </remarks>
        [HttpGet("mis-socios")]
        [Authorize(Roles = "ENTRENADOR")]
        [ProducesResponseType(typeof(ApiResponse<List<SocioResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> MisSocios()
        {
            var userId = ObtenerUserId();
            var result = await _service.GetMisSociosAsync(userId);
            return Ok(ApiResponse<List<SocioResponseDto>>.Ok(result));
        }

        private int ObtenerUserId() =>
            int.Parse(User.FindFirstValue("userId")
                ?? throw new UnauthorizedAccessException("No se pudo obtener el userId del token."));
    }

    // =============================================================================
    // MembresiasController
    // =============================================================================

    /// <summary>
    /// Controlador de planes de membresía y suscripciones de socios.
    ///
    /// Permisos por endpoint:
    /// <list type="table">
    ///   <item>POST /api/membresias              → Solo ADMIN (crear plan)</item>
    ///   <item>GET  /api/membresias              → ADMIN, ENTRENADOR, SOCIO (ver catálogo)</item>
    ///   <item>GET  /api/membresias/{id}         → ADMIN, ENTRENADOR, SOCIO</item>
    ///   <item>PUT  /api/membresias/{id}         → Solo ADMIN</item>
    ///   <item>POST /api/membresias/asignar      → Solo ADMIN</item>
    ///   <item>GET  /api/membresias/socio/{id}   → ADMIN (historial de cualquier socio)</item>
    ///   <item>GET  /api/membresias/mi-membresia → Solo SOCIO (su plan activo)</item>
    /// </list>
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class MembresiasController : ControllerBase
    {
        private readonly IMembresiaService _service;
        public MembresiasController(IMembresiaService service) => _service = service;

        /// <summary>
        /// Crea un nuevo plan de membresía en el catálogo.
        /// </summary>
        /// <remarks>
        ///     POST /api/membresias
        ///     Authorization: Bearer {token_admin}
        ///     {
        ///         "nombre":       "Gold",
        ///         "descripcion":  "Acceso total + clases + nutricionista",
        ///         "duracionDias": 30,
        ///         "precio":       249.00,
        ///         "esRenovable":  true
        ///     }
        /// </remarks>
        /// <response code="201">Plan creado exitosamente.</response>
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(ApiResponse<MembresiaResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>),               StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Crear([FromBody] CrearMembresiaDto dto)
        {
            var result = await _service.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.MembresiaId },
                ApiResponse<MembresiaResponseDto>.Ok(result, "Plan de membresía creado exitosamente."));
        }

        /// <summary>
        /// Lista todos los planes del catálogo. Visible por todos los roles autenticados.
        /// </summary>
        /// <remarks>
        ///     GET /api/membresias
        ///     Authorization: Bearer {token_cualquier_rol}
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<MembresiaResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(ApiResponse<List<MembresiaResponseDto>>.Ok(result));
        }

        /// <summary>
        /// Obtiene el detalle de un plan por su ID.
        /// </summary>
        /// <param name="id">MembresiaId del plan.</param>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<MembresiaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),               StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return Ok(ApiResponse<MembresiaResponseDto>.Ok(result));
        }

        /// <summary>
        /// Actualiza los datos de un plan de membresía.
        /// </summary>
        /// <remarks>
        ///     PUT /api/membresias/2
        ///     Authorization: Bearer {token_admin}
        ///     {
        ///         "precio":   159.00,
        ///         "isActive": true
        ///     }
        /// </remarks>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(ApiResponse<MembresiaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),               StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarMembresiaDto dto)
        {
            var result = await _service.ActualizarAsync(id, dto);
            return Ok(ApiResponse<MembresiaResponseDto>.Ok(result, "Plan actualizado exitosamente."));
        }

        /// <summary>
        /// Asigna un plan de membresía a un socio (crea la suscripción).
        /// La fecha de fin se calcula automáticamente: FechaInicio + DuracionDias del plan.
        /// </summary>
        /// <remarks>
        ///     POST /api/membresias/asignar
        ///     Authorization: Bearer {token_admin}
        ///     {
        ///         "socioId":      5,
        ///         "membresiaId":  2,
        ///         "fechaInicio":  "2025-02-01",
        ///         "montoPagado":  149.00,
        ///         "notas":        "Descuento por referido"
        ///     }
        /// </remarks>
        [HttpPost("asignar")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(ApiResponse<SocioMembresiaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),                    StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AsignarSocio([FromBody] AsignarMembresiaDto dto)
        {
            var result = await _service.AsignarSocioAsync(dto);
            return Ok(ApiResponse<SocioMembresiaResponseDto>.Ok(result, "Membresía asignada al socio."));
        }

        /// <summary>
        /// Obtiene el historial completo de membresías de un socio específico.
        /// Solo accesible por ADMIN.
        /// </summary>
        /// <remarks>
        ///     GET /api/membresias/socio/5
        ///     Authorization: Bearer {token_admin}
        /// </remarks>
        /// <param name="socioId">SocioId del socio a consultar.</param>
        [HttpGet("socio/{socioId:int}")]
        [Authorize(Roles = "ADMIN")]
        [ProducesResponseType(typeof(ApiResponse<List<SocioMembresiaResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> HistorialSocio(int socioId)
        {
            var result = await _service.GetHistorialSocioAsync(socioId);
            return Ok(ApiResponse<List<SocioMembresiaResponseDto>>.Ok(result));
        }

        /// <summary>
        /// Retorna el plan de membresía activo del socio autenticado.
        /// El UserId se resuelve desde el claim del JWT.
        /// </summary>
        /// <remarks>
        ///     GET /api/membresias/mi-membresia
        ///     Authorization: Bearer {token_socio}
        /// </remarks>
        [HttpGet("mi-membresia")]
        [Authorize(Roles = "SOCIO")]
        [ProducesResponseType(typeof(ApiResponse<SocioMembresiaResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> MiMembresia()
        {
            var userId = ObtenerUserId();
            var result = await _service.GetMiMembresiaActivaAsync(userId);
            return Ok(ApiResponse<SocioMembresiaResponseDto?>.Ok(result,
                result is null ? "No tienes una membresía activa." : "Membresía activa encontrada."));
        }

        private int ObtenerUserId() =>
            int.Parse(User.FindFirstValue("userId")
                ?? throw new UnauthorizedAccessException("No se pudo obtener el userId del token."));
    }

    // =============================================================================
    // AsistenciasController
    // =============================================================================

    /// <summary>
    /// Controlador de asistencias (check-in / check-out) de socios.
    ///
    /// Permisos por endpoint:
    /// <list type="table">
    ///   <item>POST /api/asistencias/entrada        → ADMIN y ENTRENADOR</item>
    ///   <item>PUT  /api/asistencias/{id}/salida    → ADMIN y ENTRENADOR</item>
    ///   <item>GET  /api/asistencias/socio/{id}     → ADMIN y ENTRENADOR</item>
    ///   <item>GET  /api/asistencias/mi-historial   → Solo SOCIO</item>
    /// </list>
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class AsistenciasController : ControllerBase
    {
        private readonly IAsistenciaService _service;
        public AsistenciasController(IAsistenciaService service) => _service = service;

        /// <summary>
        /// Registra la entrada (check-in) de un socio al gimnasio.
        /// Si no se envía FechaHoraEntrada, se usa la hora UTC actual del servidor.
        /// No se permite registrar entrada si hay una sesión abierta sin salida.
        /// </summary>
        /// <remarks>
        ///     POST /api/asistencias/entrada
        ///     Authorization: Bearer {token_entrenador_o_admin}
        ///     {
        ///         "socioId":          5,
        ///         "fechaHoraEntrada": "2025-02-01T09:00:00Z",
        ///         "observaciones":    "Visita normal"
        ///     }
        /// </remarks>
        /// <response code="201">Asistencia registrada exitosamente.</response>
        /// <response code="400">El socio ya tiene una sesión abierta.</response>
        [HttpPost("entrada")]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        [ProducesResponseType(typeof(ApiResponse<AsistenciaResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>),                StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrarEntrada([FromBody] RegistrarAsistenciaDto dto)
        {
            var userId    = ObtenerUserId();
            var resultado = await _service.RegistrarEntradaAsync(dto, userId);
            return StatusCode(StatusCodes.Status201Created,
                ApiResponse<AsistenciaResponseDto>.Ok(resultado, "Entrada registrada exitosamente."));
        }

        /// <summary>
        /// Registra la salida (check-out) de un socio, cerrando la sesión abierta.
        /// Si no se envía FechaHoraSalida, se usa la hora UTC actual del servidor.
        /// </summary>
        /// <remarks>
        ///     PUT /api/asistencias/12/salida
        ///     Authorization: Bearer {token_entrenador_o_admin}
        ///     {
        ///         "fechaHoraSalida": "2025-02-01T11:30:00Z",
        ///         "observaciones":   "Entrenamiento completado"
        ///     }
        /// </remarks>
        /// <param name="id">AsistenciaId del registro a cerrar.</param>
        /// <response code="200">Salida registrada exitosamente.</response>
        /// <response code="400">La sesión ya tiene una salida registrada.</response>
        [HttpPut("{id:int}/salida")]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        [ProducesResponseType(typeof(ApiResponse<AsistenciaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>),                StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrarSalida(int id, [FromBody] RegistrarSalidaDto dto)
        {
            var resultado = await _service.RegistrarSalidaAsync(id, dto);
            return Ok(ApiResponse<AsistenciaResponseDto>.Ok(resultado, "Salida registrada exitosamente."));
        }

        /// <summary>
        /// Obtiene el historial paginado de asistencias de un socio específico.
        /// Accesible por ADMIN y ENTRENADOR.
        /// </summary>
        /// <remarks>
        ///     GET /api/asistencias/socio/5?page=1&amp;pageSize=20
        ///     Authorization: Bearer {token_admin_o_entrenador}
        /// </remarks>
        /// <param name="socioId">SocioId del socio a consultar.</param>
        /// <param name="paginacion">Parámetros de paginación.</param>
        [HttpGet("socio/{socioId:int}")]
        [Authorize(Roles = "ADMIN,ENTRENADOR")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<AsistenciaResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> HistorialSocio(int socioId, [FromQuery] PaginacionDto paginacion)
        {
            var resultado = await _service.GetHistorialAsync(socioId, paginacion);
            return Ok(ApiResponse<PagedResponse<AsistenciaResponseDto>>.Ok(resultado));
        }

        /// <summary>
        /// Retorna el historial de asistencias del socio autenticado.
        /// El UserId se obtiene del claim del JWT — el socio solo ve su propio historial.
        /// </summary>
        /// <remarks>
        ///     GET /api/asistencias/mi-historial?page=1&amp;pageSize=10
        ///     Authorization: Bearer {token_socio}
        /// </remarks>
        [HttpGet("mi-historial")]
        [Authorize(Roles = "SOCIO")]
        [ProducesResponseType(typeof(ApiResponse<PagedResponse<AsistenciaResponseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> MiHistorial([FromQuery] PaginacionDto paginacion)
        {
            var userId    = ObtenerUserId();
            var resultado = await _service.GetMiHistorialAsync(userId, paginacion);
            return Ok(ApiResponse<PagedResponse<AsistenciaResponseDto>>.Ok(resultado));
        }

        private int ObtenerUserId() =>
            int.Parse(User.FindFirstValue("userId")
                ?? throw new UnauthorizedAccessException("No se pudo obtener el userId del token."));
    }
}