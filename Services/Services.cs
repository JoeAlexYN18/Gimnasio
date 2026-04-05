using Gimnasio.DTOs;
using Gimnasio.DTOs.Asistencias;
using Gimnasio.DTOs.Entrenadores;
using Gimnasio.DTOs.Membresias;
using Gimnasio.DTOs.Socios;
using Gimnasio.Repositories;
using Gimnasio.Entities;
using Gimnasio.ValueObjects;
using Gimnasio.Helpers;

namespace Gimnasio.Services
{
    // =============================================================================
    // EntrenadorService
    // =============================================================================

    /// <summary>
    /// Servicio de entrenadores. Gestiona registro, actualización
    /// y asignación de socios a entrenadores.
    /// </summary>
    public class EntrenadorService : IEntrenadorService
    {
        private readonly IUserRepository       _userRepo;
        private readonly IEntrenadorRepository _entrenadorRepo;
        private readonly ISocioRepository      _socioRepo;
        private readonly IRoleRepository       _roleRepo;

        public EntrenadorService(
            IUserRepository       userRepo,
            IEntrenadorRepository entrenadorRepo,
            ISocioRepository      socioRepo,
            IRoleRepository       roleRepo)
        {
            _userRepo       = userRepo;
            _entrenadorRepo = entrenadorRepo;
            _socioRepo      = socioRepo;
            _roleRepo       = roleRepo;
        }

        /// <inheritdoc/>
        public async Task<EntrenadorResponseDto> CrearAsync(CrearEntrenadorDto dto)
        {
            if (await _userRepo.ExistsUserNameAsync(dto.UserName))
                throw new InvalidOperationException($"El nombre de usuario '{dto.UserName}' ya está en uso.");
            if (await _userRepo.ExistsEmailAsync(dto.Email))
                throw new InvalidOperationException($"El correo '{dto.Email}' ya está registrado.");

            var user = new User
            {
                UserName           = dto.UserName,
                NormalizedUserName = dto.UserName.ToUpperInvariant(),
                Email              = dto.Email,
                NormalizedEmail    = dto.Email.ToUpperInvariant(),
                PasswordHash       = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                PhoneNumber        = dto.PhoneNumber,
                CreatedAt          = DateTime.UtcNow
            };
            await _userRepo.AddAsync(user);

            var role = await _roleRepo.GetByNameAsync("ENTRENADOR")
                ?? throw new InvalidOperationException("Rol ENTRENADOR no encontrado.");

            user.UserRoles.Add(new UserRole { User = user, Role = role, AssignedAt = DateTime.UtcNow });

            var entrenador = new Entrenador
            {
                User             = user,
                Especialidad     = dto.Especialidad,
                Certificaciones  = dto.Certificaciones,
                FechaIngreso     = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            await _entrenadorRepo.AddAsync(entrenador);
            await _entrenadorRepo.SaveChangesAsync();

            return MapToDto(entrenador);
        }

        /// <inheritdoc/>
        public async Task<PagedResponse<EntrenadorResponseDto>> GetAllAsync(PaginacionDto paginacion)
        {
            var (items, total) = await _entrenadorRepo.GetPagedAsync(paginacion.Page, paginacion.PageSize);
            return new PagedResponse<EntrenadorResponseDto>
            {
                Items      = items.Select(MapToDto).ToList(),
                Page       = paginacion.Page,
                PageSize   = paginacion.PageSize,
                TotalCount = total
            };
        }

        /// <inheritdoc/>
        public async Task<EntrenadorResponseDto> GetByIdAsync(int entrenadorId)
        {
            var e = await _entrenadorRepo.GetWithUserAsync(entrenadorId)
                ?? throw new KeyNotFoundException($"Entrenador con ID {entrenadorId} no encontrado.");
            return MapToDto(e);
        }

        /// <inheritdoc/>
        public async Task<EntrenadorResponseDto> ActualizarAsync(int entrenadorId, ActualizarEntrenadorDto dto)
        {
            var e = await _entrenadorRepo.GetWithUserAsync(entrenadorId)
                ?? throw new KeyNotFoundException($"Entrenador con ID {entrenadorId} no encontrado.");

            if (dto.PhoneNumber   is not null) { e.User.PhoneNumber = dto.PhoneNumber; e.User.UpdatedAt = DateTime.UtcNow; }
            if (dto.Especialidad  is not null) e.Especialidad  = dto.Especialidad;
            if (dto.Certificaciones is not null) e.Certificaciones = dto.Certificaciones;

            e.User.UpdatedAt = DateTime.UtcNow;
            _userRepo.Update(e.User);

            _entrenadorRepo.Update(e);
            await _entrenadorRepo.SaveChangesAsync();
            return MapToDto(e);
        }

        /// <inheritdoc/>
        public async Task AsignarSocioAsync(AsignarEntrenadorDto dto)
        {
            var socio = await _socioRepo.GetByIdAsync(dto.SocioId)
                ?? throw new KeyNotFoundException($"Socio con ID {dto.SocioId} no encontrado.");
            var entrenador = await _entrenadorRepo.GetByIdAsync(dto.EntrenadorId)
                ?? throw new KeyNotFoundException($"Entrenador con ID {dto.EntrenadorId} no encontrado.");

            socio.SocioEntrenadores.Add(new SocioEntrenador
            {
                SocioId        = dto.SocioId,
                EntrenadorId   = dto.EntrenadorId,
                FechaAsignacion = DateOnly.FromDateTime(DateTime.UtcNow),
                Activo         = true
            });

            _socioRepo.Update(socio);
            await _socioRepo.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<List<SocioResponseDto>> GetMisSociosAsync(int userId)
        {
            var entrenador = await _entrenadorRepo.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Perfil de entrenador no encontrado.");

            var socios = await _socioRepo.GetByEntrenadorIdAsync(entrenador.EntrenadorId);
            return socios.Select(s => new SocioResponseDto
            {
                SocioId         = s.SocioId,
                UserId          = s.UserId,
                UserName        = s.User.UserName,
                Email           = s.User.Email,
                PhoneNumber     = s.User.PhoneNumber,
                FechaNacimiento = s.FechaNacimiento,
                Genero          = s.Genero,
                AlturaCm        = s.AlturaCm,
                PesoKg          = s.PesoKg,
                FechaRegistro   = s.FechaRegistro,
                IsActive        = s.IsActive
            }).ToList();
        }

        private static EntrenadorResponseDto MapToDto(Entrenador e) => new()
        {
            EntrenadorId    = e.EntrenadorId,
            UserId          = e.UserId,
            UserName        = e.User.UserName,
            Email           = e.User.Email,
            PhoneNumber     = e.User.PhoneNumber,
            Especialidad    = e.Especialidad,
            Certificaciones = e.Certificaciones,
            FechaIngreso    = e.FechaIngreso,
            IsActive        = e.IsActive
        };
    }

    // =============================================================================
    // MembresiaService
    // =============================================================================

    /// <summary>
    /// Servicio de membresías. Gestiona el catálogo de planes y las suscripciones
    /// de los socios, aplicando las reglas de dominio de Membresia y SocioMembresia.
    /// </summary>
    public class MembresiaService : IMembresiaService
    {
        private readonly IMembresiaRepository      _membresiaRepo;
        private readonly ISocioMembresiaRepository _socioMembresiaRepo;
        private readonly ISocioRepository          _socioRepo;

        public MembresiaService(
            IMembresiaRepository      membresiaRepo,
            ISocioMembresiaRepository socioMembresiaRepo,
            ISocioRepository          socioRepo)
        {
            _membresiaRepo      = membresiaRepo;
            _socioMembresiaRepo = socioMembresiaRepo;
            _socioRepo          = socioRepo;
        }

        /// <inheritdoc/>
        public async Task<MembresiaResponseDto> CrearAsync(CrearMembresiaDto dto)
        {
            if (await _membresiaRepo.ExistsNombreAsync(dto.Nombre))
                throw new InvalidOperationException($"Ya existe un plan con el nombre '{dto.Nombre}'.");

            var membresia = new Membresia
            {
                Nombre       = dto.Nombre,
                Descripcion  = dto.Descripcion,
                DuracionDias = dto.DuracionDias,
                Precio       = dto.Precio,
                EsRenovable  = dto.EsRenovable,
                CreatedAt    = DateTime.UtcNow
            };

            await _membresiaRepo.AddAsync(membresia);
            await _membresiaRepo.SaveChangesAsync();
            return MapToDto(membresia);
        }

        /// <inheritdoc/>
        public async Task<List<MembresiaResponseDto>> GetAllAsync() =>
            (await _membresiaRepo.GetAllAsync()).Select(MapToDto).ToList();

        /// <inheritdoc/>
        public async Task<MembresiaResponseDto> GetByIdAsync(int membresiaId)
        {
            var m = await _membresiaRepo.GetByIdAsync(membresiaId)
                ?? throw new KeyNotFoundException($"Membresía con ID {membresiaId} no encontrada.");
            return MapToDto(m);
        }

        /// <inheritdoc/>
        public async Task<MembresiaResponseDto> ActualizarAsync(int membresiaId, ActualizarMembresiaDto dto)
        {
            var m = await _membresiaRepo.GetByIdAsync(membresiaId)
                ?? throw new KeyNotFoundException($"Membresía con ID {membresiaId} no encontrada.");

            if (dto.Descripcion  is not null) m.Descripcion  = dto.Descripcion;
            if (dto.DuracionDias is not null) m.DuracionDias = dto.DuracionDias.Value;
            if (dto.Precio       is not null) m.Precio       = dto.Precio.Value;
            if (dto.EsRenovable  is not null) m.EsRenovable  = dto.EsRenovable.Value;
            if (dto.IsActive     is not null) m.IsActive     = dto.IsActive.Value;

            _membresiaRepo.Update(m);
            await _membresiaRepo.SaveChangesAsync();
            return MapToDto(m);
        }

        /// <inheritdoc/>
        public async Task<SocioMembresiaResponseDto> AsignarSocioAsync(AsignarMembresiaDto dto)
        {
            var socio = await _socioRepo.GetByIdAsync(dto.SocioId)
                ?? throw new KeyNotFoundException($"Socio con ID {dto.SocioId} no encontrado.");
            var plan = await _membresiaRepo.GetByIdAsync(dto.MembresiaId)
                ?? throw new KeyNotFoundException($"Plan con ID {dto.MembresiaId} no encontrado.");

            if (!plan.IsActive)
                throw new InvalidOperationException("El plan seleccionado no está activo.");

            // SocioMembresia.Estado y MontoPagado aplican reglas de dominio en sus setters
            var suscripcion = new SocioMembresia
            {
                SocioId     = dto.SocioId,
                MembresiaId = dto.MembresiaId,
                FechaInicio = dto.FechaInicio,
                FechaFin    = dto.FechaInicio.AddDays(plan.DuracionDias),
                Estado      = EstadoMembresia.Activa,
                MontoPagado = dto.MontoPagado,
                Notas       = dto.Notas,
                CreatedAt   = DateTime.UtcNow
            };
            suscripcion.Membresia = plan;   // para el mapeo del DTO

            await _socioMembresiaRepo.AddAsync(suscripcion);
            await _socioMembresiaRepo.SaveChangesAsync();
            return MapSocioMembresiaToDto(suscripcion);
        }

        /// <inheritdoc/>
        public async Task<List<SocioMembresiaResponseDto>> GetHistorialSocioAsync(int socioId)
        {
            var lista = await _socioMembresiaRepo.GetBySocioIdAsync(socioId);
            return lista.Select(MapSocioMembresiaToDto).ToList();
        }

        /// <inheritdoc/>
        public async Task<SocioMembresiaResponseDto?> GetMiMembresiaActivaAsync(int userId)
        {
            var socio = await _socioRepo.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Perfil de socio no encontrado.");
            var activa = await _socioMembresiaRepo.GetActivaBySocioIdAsync(socio.SocioId);
            return activa is null ? null : MapSocioMembresiaToDto(activa);
        }

        private static MembresiaResponseDto MapToDto(Membresia m) => new()
        {
            MembresiaId  = m.MembresiaId,
            Nombre       = m.Nombre,
            Descripcion  = m.Descripcion,
            DuracionDias = m.DuracionDias,
            Precio       = m.Precio,
            EsRenovable  = m.EsRenovable,
            IsActive     = m.IsActive,
            CreatedAt    = PeruDateTimeHelper.ToPeruString(m.CreatedAt)
        };

        private static SocioMembresiaResponseDto MapSocioMembresiaToDto(SocioMembresia sm) => new()
        {
            SocioMembresiaId = sm.SocioMembresiaId,
            SocioId          = sm.SocioId,
            NombreMembresia  = sm.Membresia.Nombre,
            FechaInicio      = sm.FechaInicio,
            FechaFin         = sm.FechaFin,
            Estado           = sm.Estado,
            MontoPagado      = sm.MontoPagado,
            Notas            = sm.Notas
        };
    }

    // =============================================================================
    // AsistenciaService
    // =============================================================================

    /// <summary>
    /// Servicio de asistencias. Registra entradas y salidas de socios,
    /// y expone el historial paginado respetando los permisos por rol.
    /// </summary>
    public class AsistenciaService : IAsistenciaService
    {
        private readonly IAsistenciaRepository _asistenciaRepo;
        private readonly ISocioRepository      _socioRepo;
        private readonly IUserRepository       _userRepo;

        public AsistenciaService(
            IAsistenciaRepository asistenciaRepo,
            ISocioRepository      socioRepo,
            IUserRepository       userRepo)
        {
            _asistenciaRepo = asistenciaRepo;
            _socioRepo      = socioRepo;
            _userRepo       = userRepo;
        }

        /// <inheritdoc/>
        public async Task<AsistenciaResponseDto> RegistrarEntradaAsync(
            RegistrarAsistenciaDto dto, int registradaPorUserId)
        {
            var socio = await _socioRepo.GetWithUserAsync(dto.SocioId)
                ?? throw new KeyNotFoundException($"Socio con ID {dto.SocioId} no encontrado.");

            var sesionAbierta = await _asistenciaRepo.GetSesionAbiertaAsync(dto.SocioId);
            if (sesionAbierta is not null)
                throw new InvalidOperationException(
                    $"El socio ya tiene una sesión abierta (AsistenciaId: {sesionAbierta.AsistenciaId}). " +
                    "Registre la salida antes de registrar una nueva entrada.");

            var registradaPorUser = await _userRepo.GetWithRolesAsync(registradaPorUserId)
            ?? throw new UnauthorizedAccessException("No se encontró el usuario que registra la asistencia.");

            var asistencia = new Asistencia
            {
                SocioId = dto.SocioId,
                FechaHoraEntrada = dto.FechaHoraEntrada is not null
                    ? PeruDateTimeHelper.ParsePeruToUtc(dto.FechaHoraEntrada)
                    : DateTime.UtcNow,
                Observaciones = dto.Observaciones,
                RegistradaPorUserId = registradaPorUserId,
                RegistradaPorUser = registradaPorUser
            };
            asistencia.Socio = socio;

            await _asistenciaRepo.AddAsync(asistencia);
            await _asistenciaRepo.SaveChangesAsync();
            return MapToDto(asistencia, socio.User.UserName);
        }

        /// <inheritdoc/>
        public async Task<AsistenciaResponseDto> RegistrarSalidaAsync(
            int asistenciaId, RegistrarSalidaDto dto)
        {
            var asistencia = await _asistenciaRepo.GetWithDetailsAsync(asistenciaId) // ← cambio
                ?? throw new KeyNotFoundException($"Registro de asistencia {asistenciaId} no encontrado.");

            if (asistencia.FechaHoraSalida is not null)
                throw new InvalidOperationException("Esta sesión ya tiene una salida registrada.");

            asistencia.FechaHoraSalida = dto.FechaHoraSalida is not null
                ? PeruDateTimeHelper.ParsePeruToUtc(dto.FechaHoraSalida)
                : DateTime.UtcNow;
            if (dto.Observaciones is not null) asistencia.Observaciones = dto.Observaciones;

            _asistenciaRepo.Update(asistencia);
            await _asistenciaRepo.SaveChangesAsync();

            return MapToDto(asistencia, asistencia.Socio.User.UserName);
        }

        /// <inheritdoc/>
        public async Task<PagedResponse<AsistenciaResponseDto>> GetHistorialAsync(
            int socioId, PaginacionDto paginacion)
        {
            var (items, total) = await _asistenciaRepo
                .GetBySocioIdPagedAsync(socioId, paginacion.Page, paginacion.PageSize);

            return new PagedResponse<AsistenciaResponseDto>
            {
                Items      = items.Select(a => MapToDto(a, a.Socio.User.UserName)).ToList(),
                Page       = paginacion.Page,
                PageSize   = paginacion.PageSize,
                TotalCount = total
            };
        }

        /// <inheritdoc/>
        public async Task<PagedResponse<AsistenciaResponseDto>> GetMiHistorialAsync(
            int userId, PaginacionDto paginacion)
        {
            var socio = await _socioRepo.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Perfil de socio no encontrado.");
            return await GetHistorialAsync(socio.SocioId, paginacion);
        }

        private static AsistenciaResponseDto MapToDto(Asistencia a, string socioUserName)
        {
            int? duracion = a.FechaHoraSalida.HasValue
                ? (int)(a.FechaHoraSalida.Value - a.FechaHoraEntrada).TotalMinutes
                : null;

            return new AsistenciaResponseDto
            {
                AsistenciaId      = a.AsistenciaId,
                SocioId           = a.SocioId,
                SocioUserName     = socioUserName,
                FechaHoraEntrada  = PeruDateTimeHelper.ToPeruString(a.FechaHoraEntrada),
                FechaHoraSalida   = PeruDateTimeHelper.ToPeruString(a.FechaHoraSalida),
                DuracionMinutos   = duracion,
                Observaciones     = a.Observaciones,
                RegistradaPor     = a.RegistradaPorUser?.UserName
            };
        }
    }
}
