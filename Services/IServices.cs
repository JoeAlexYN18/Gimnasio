using Gimnasio.DTOs;
using Gimnasio.DTOs.Auth;
using Gimnasio.DTOs.Asistencias;
using Gimnasio.DTOs.Entrenadores;
using Gimnasio.DTOs.Membresias;
using Gimnasio.DTOs.Socios;

namespace Gimnasio.Services
{
    /// <summary>
    /// Servicio de autenticación. Gestiona login y generación de tokens JWT.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Valida las credenciales y retorna un JWT firmado si son correctas.
        /// Lanza <see cref="UnauthorizedAccessException"/> si las credenciales son inválidas.
        /// </summary>
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Servicio de gestión de socios. Operaciones CRUD y consultas por rol.
    /// </summary>
    public interface ISocioService
    {
        /// <summary>
        /// Registra un nuevo socio creando su User y perfil de Socio.
        /// Solo ADMIN puede invocar este método desde el controlador.
        /// </summary>
        Task<SocioResponseDto> CrearAsync(CrearSocioDto dto);

        /// <summary>
        /// Obtiene la lista paginada de todos los socios.
        /// Solo ADMIN.
        /// </summary>
        Task<PagedResponse<SocioResponseDto>> GetAllAsync(PaginacionDto paginacion);

        /// <summary>Obtiene el detalle de un socio por su SocioId.</summary>
        Task<SocioResponseDto> GetByIdAsync(int socioId);

        /// <summary>
        /// Obtiene el perfil del socio autenticado desde su UserId (claim JWT).
        /// Usado por el rol SOCIO para consultar sus propios datos.
        /// </summary>
        Task<SocioResponseDto> GetMiPerfilAsync(int userId);

        /// <summary>Actualiza el perfil de un socio. Solo ADMIN.</summary>
        Task<SocioResponseDto> ActualizarAsync(int socioId, ActualizarSocioDto dto);

        /// <summary>Desactiva (soft-delete) un socio. Solo ADMIN.</summary>
        Task DesactivarAsync(int socioId);
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Servicio de gestión de entrenadores.
    /// </summary>
    public interface IEntrenadorService
    {
        /// <summary>
        /// Registra un nuevo entrenador creando su User y perfil de Entrenador.
        /// Solo ADMIN.
        /// </summary>
        Task<EntrenadorResponseDto> CrearAsync(CrearEntrenadorDto dto);

        /// <summary>Lista paginada de entrenadores. Solo ADMIN.</summary>
        Task<PagedResponse<EntrenadorResponseDto>> GetAllAsync(PaginacionDto paginacion);

        /// <summary>Detalle de un entrenador. ADMIN y el propio ENTRENADOR.</summary>
        Task<EntrenadorResponseDto> GetByIdAsync(int entrenadorId);

        /// <summary>Actualiza el perfil de un entrenador. Solo ADMIN.</summary>
        Task<EntrenadorResponseDto> ActualizarAsync(int entrenadorId, ActualizarEntrenadorDto dto);

        /// <summary>Asigna un entrenador a un socio. Solo ADMIN.</summary>
        Task AsignarSocioAsync(AsignarEntrenadorDto dto);

        /// <summary>
        /// Obtiene los socios asignados al entrenador autenticado.
        /// Usado por el rol ENTRENADOR.
        /// </summary>
        Task<List<SocioResponseDto>> GetMisSociosAsync(int userId);
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Servicio de gestión de membresías (catálogo y suscripciones).
    /// </summary>
    public interface IMembresiaService
    {
        /// <summary>Crea un nuevo plan en el catálogo. Solo ADMIN.</summary>
        Task<MembresiaResponseDto> CrearAsync(CrearMembresiaDto dto);

        /// <summary>Lista todos los planes del catálogo. Accesible por todos los roles.</summary>
        Task<List<MembresiaResponseDto>> GetAllAsync();

        /// <summary>Detalle de un plan. Accesible por todos los roles.</summary>
        Task<MembresiaResponseDto> GetByIdAsync(int membresiaId);

        /// <summary>Actualiza un plan del catálogo. Solo ADMIN.</summary>
        Task<MembresiaResponseDto> ActualizarAsync(int membresiaId, ActualizarMembresiaDto dto);

        /// <summary>Asigna una membresía a un socio (crea suscripción). Solo ADMIN.</summary>
        Task<SocioMembresiaResponseDto> AsignarSocioAsync(AsignarMembresiaDto dto);

        /// <summary>
        /// Obtiene el historial de membresías de un socio.
        /// ADMIN accede a cualquier socio; SOCIO solo al suyo.
        /// </summary>
        Task<List<SocioMembresiaResponseDto>> GetHistorialSocioAsync(int socioId);

        /// <summary>
        /// Retorna la membresía activa del socio autenticado.
        /// Usado por el rol SOCIO.
        /// </summary>
        Task<SocioMembresiaResponseDto?> GetMiMembresiaActivaAsync(int userId);
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Servicio de gestión de asistencias (check-in / check-out).
    /// </summary>
    public interface IAsistenciaService
    {
        /// <summary>
        /// Registra el ingreso de un socio al gimnasio.
        /// ENTRENADOR y ADMIN. Registra el userId del usuario autenticado como registrador.
        /// </summary>
        Task<AsistenciaResponseDto> RegistrarEntradaAsync(RegistrarAsistenciaDto dto, int registradaPorUserId);

        /// <summary>
        /// Registra la salida de un socio (cierra la sesión abierta).
        /// ENTRENADOR y ADMIN.
        /// </summary>
        Task<AsistenciaResponseDto> RegistrarSalidaAsync(int asistenciaId, RegistrarSalidaDto dto);

        /// <summary>
        /// Obtiene el historial paginado de asistencias de un socio.
        /// ADMIN accede a cualquier socio; SOCIO y ENTRENADOR solo a los asignados.
        /// </summary>
        Task<PagedResponse<AsistenciaResponseDto>> GetHistorialAsync(int socioId, PaginacionDto paginacion);

        /// <summary>
        /// Retorna el historial de asistencias del socio autenticado.
        /// Usado por el rol SOCIO.
        /// </summary>
        Task<PagedResponse<AsistenciaResponseDto>> GetMiHistorialAsync(int userId, PaginacionDto paginacion);
    }
}