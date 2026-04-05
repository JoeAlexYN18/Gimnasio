using Gimnasio.DTOs;
using Gimnasio.DTOs.Socios;
using Gimnasio.Repositories;
using Gimnasio.Entities;

namespace Gimnasio.Services
{
    /// <summary>
    /// Servicio de socios. Orquesta la creación de User + Socio,
    /// aplica las reglas de dominio y mapea entidades a DTOs.
    /// </summary>
    public class SocioService : ISocioService
    {
        private readonly IUserRepository   _userRepo;
        private readonly ISocioRepository  _socioRepo;
        private readonly IRoleRepository   _roleRepo;

        public SocioService(
            IUserRepository  userRepo,
            ISocioRepository socioRepo,
            IRoleRepository  roleRepo)
        {
            _userRepo  = userRepo;
            _socioRepo = socioRepo;
            _roleRepo  = roleRepo;
        }

        /// <inheritdoc/>
        public async Task<SocioResponseDto> CrearAsync(CrearSocioDto dto)
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

            var roleSocio = await _roleRepo.GetByNameAsync("SOCIO")
                ?? throw new InvalidOperationException("Rol SOCIO no encontrado en la base de datos.");

            user.UserRoles.Add(new UserRole
            {
                User       = user,
                Role       = roleSocio,
                AssignedAt = DateTime.UtcNow
            });

            var socio = new Socio
            {
                User               = user,
                FechaNacimiento    = dto.FechaNacimiento,
                Genero             = dto.Genero,      
                AlturaCm           = dto.AlturaCm,    
                PesoKg             = dto.PesoKg,      
                EmergenciaNombre   = dto.EmergenciaNombre,
                EmergenciaTelefono = dto.EmergenciaTelefono,
                FechaRegistro      = DateOnly.FromDateTime(DateTime.UtcNow)
            };
            await _socioRepo.AddAsync(socio);
            await _socioRepo.SaveChangesAsync();

            return MapToDto(socio);
        }

        /// <inheritdoc/>
        public async Task<PagedResponse<SocioResponseDto>> GetAllAsync(PaginacionDto paginacion)
        {
            var (items, total) = await _socioRepo.GetPagedAsync(paginacion.Page, paginacion.PageSize);
            return new PagedResponse<SocioResponseDto>
            {
                Items     = items.Select(MapToDto).ToList(),
                Page      = paginacion.Page,
                PageSize  = paginacion.PageSize,
                TotalCount = total
            };
        }

        /// <inheritdoc/>
        public async Task<SocioResponseDto> GetByIdAsync(int socioId)
        {
            var socio = await _socioRepo.GetWithUserAsync(socioId)
                ?? throw new KeyNotFoundException($"Socio con ID {socioId} no encontrado.");
            return MapToDto(socio);
        }

        /// <inheritdoc/>
        public async Task<SocioResponseDto> GetMiPerfilAsync(int userId)
        {
            var socio = await _socioRepo.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Perfil de socio no encontrado para este usuario.");
            return MapToDto(socio);
        }

        /// <inheritdoc/>
        public async Task<SocioResponseDto> ActualizarAsync(int socioId, ActualizarSocioDto dto)
        {
            var socio = await _socioRepo.GetWithUserAsync(socioId)
                ?? throw new KeyNotFoundException($"Socio con ID {socioId} no encontrado.");

            if (dto.Email is not null && dto.Email != socio.User.Email)
            {
                if (await _userRepo.ExistsEmailAsync(dto.Email))
                    throw new InvalidOperationException($"El correo '{dto.Email}' ya está en uso.");
                socio.User.Email           = dto.Email;
                socio.User.NormalizedEmail = dto.Email.ToUpperInvariant();
                socio.User.UpdatedAt       = DateTime.UtcNow;
                _userRepo.Update(socio.User);
            }
            if (dto.PhoneNumber is not null)
            {
                socio.User.PhoneNumber = dto.PhoneNumber;
                socio.User.UpdatedAt   = DateTime.UtcNow;
                _userRepo.Update(socio.User);
            }

            if (dto.Genero             is not null) socio.Genero             = dto.Genero;
            if (dto.AlturaCm           is not null) socio.AlturaCm           = dto.AlturaCm;
            if (dto.PesoKg             is not null) socio.PesoKg             = dto.PesoKg;
            if (dto.EmergenciaNombre   is not null) socio.EmergenciaNombre   = dto.EmergenciaNombre;
            if (dto.EmergenciaTelefono is not null) socio.EmergenciaTelefono = dto.EmergenciaTelefono;

            socio.User.UpdatedAt = DateTime.UtcNow;
            _socioRepo.Update(socio);
            await _socioRepo.SaveChangesAsync();

            return MapToDto(socio);
        }

        /// <inheritdoc/>
        public async Task DesactivarAsync(int socioId)
        {
            var socio = await _socioRepo.GetWithUserAsync(socioId)
                ?? throw new KeyNotFoundException($"Socio con ID {socioId} no encontrado.");

            socio.IsActive        = false;
            socio.User.IsActive   = false;
            socio.User.UpdatedAt  = DateTime.UtcNow;

            _socioRepo.Update(socio);
            await _socioRepo.SaveChangesAsync();
        }

        // ── Mapeo privado ─────────────────────────────────────────────────────────

        /// <summary>Mapea una entidad Socio a su DTO de respuesta.</summary>
        private static SocioResponseDto MapToDto(Socio s) => new()
        {
            SocioId            = s.SocioId,
            UserId             = s.UserId,
            UserName           = s.User.UserName,
            Email              = s.User.Email,
            PhoneNumber        = s.User.PhoneNumber,
            FechaNacimiento    = s.FechaNacimiento,
            Genero             = s.Genero,
            AlturaCm           = s.AlturaCm,
            PesoKg             = s.PesoKg,
            EmergenciaNombre   = s.EmergenciaNombre,
            EmergenciaTelefono = s.EmergenciaTelefono,
            FechaRegistro      = s.FechaRegistro,
            IsActive           = s.IsActive
        };
    }
}