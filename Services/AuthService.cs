using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gimnasio.Data;
using Gimnasio.DTOs.Auth;
using Gimnasio.Repositories;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Gimnasio.Helpers;

namespace Gimnasio.Services
{
    /// <summary>
    /// Servicio de autenticación JWT.
    /// Valida credenciales, genera tokens firmados con HMAC-SHA256 y
    /// empaqueta los claims de userId, userName y roles.
    /// No mantiene estado de sesión — el token es la única fuente de verdad.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration  _config;
        private readonly GymDbContext _context;

        public AuthService(IUserRepository userRepo, IConfiguration config, GymDbContext context)
        {
            _userRepo = userRepo;
            _config   = config;
            _context  = context;
        }

        /// <inheritdoc/>
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var user = await _userRepo.GetByUserNameAsync(dto.UserName)
                ?? throw new UnauthorizedAccessException("Credenciales inválidas.");

            if (!user.IsActive)
                throw new UnauthorizedAccessException("La cuenta está desactivada.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Credenciales inválidas.");

            var userWithRoles = await _userRepo.GetWithRolesAsync(user.UserId)
                ?? throw new UnauthorizedAccessException("Error al cargar roles del usuario.");

            var token      = GenerateToken(userWithRoles);
            var expiration = DateTime.UtcNow.AddMinutes(GetExpirationMinutes());

            var entry = _context.Entry(user);
            entry.State = EntityState.Unchanged;   
            user.LastLoginAt = DateTime.UtcNow;        
            entry.Property(u => u.LastLoginAt).IsModified = true;
            await _userRepo.SaveChangesAsync();

            return new LoginResponseDto
            {
                Token      = token,
                Expiration = PeruDateTimeHelper.ToPeruString(expiration),
                UserId     = user.UserId,
                UserName   = user.UserName,
                Roles      = userWithRoles.UserRoles.Select(ur => ur.Role.Name).ToList()
            };
        }

        // ── Métodos privados ──────────────────────────────────────────────────────

        /// <summary>
        /// Genera un token JWT firmado con HMAC-SHA256.
        /// Incluye los claims: sub, jti, userId, userName y uno por cada rol.
        /// </summary>
        private string GenerateToken(Entities.User user)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            var key         = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub,  user.UserId.ToString()),
                new(JwtRegisteredClaimNames.Jti,  Guid.NewGuid().ToString()),
                new("userId",                     user.UserId.ToString()),
                new("userName",                   user.UserName)
            };

            foreach (var ur in user.UserRoles)
                claims.Add(new Claim(ClaimTypes.Role, ur.Role.Name));

            var token = new JwtSecurityToken(
                issuer:             jwtSettings["Issuer"],
                audience:           jwtSettings["Audience"],
                claims:             claims,
                expires:            DateTime.UtcNow.AddMinutes(GetExpirationMinutes()),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private int GetExpirationMinutes() =>
            int.TryParse(_config["JwtSettings:ExpirationMinutes"], out var min) ? min : 60;
    }
}