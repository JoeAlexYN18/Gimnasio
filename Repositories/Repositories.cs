using Microsoft.EntityFrameworkCore;
using Gimnasio.Entities;
using Gimnasio.Data;

namespace Gimnasio.Repositories
{
    /// <summary>
    /// Implementación base genérica del repositorio usando EF Core.
    /// Todos los repositorios concretos heredan de esta clase.
    /// </summary>
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        /// <summary>Contexto de EF Core inyectado por DI.</summary>
        protected readonly GymDbContext _context;

        protected Repository(GymDbContext context) => _context = context;

        /// <inheritdoc/>
        public async Task<T?> GetByIdAsync(int id) =>
            await _context.Set<T>().FindAsync(id);

        /// <inheritdoc/>
        public async Task<List<T>> GetAllAsync() =>
            await _context.Set<T>().ToListAsync();

        /// <inheritdoc/>
        public async Task AddAsync(T entity) =>
            await _context.Set<T>().AddAsync(entity);

        /// <inheritdoc/>
        public void Update(T entity) =>
            _context.Set<T>().Update(entity);

        /// <inheritdoc/>
        public void Delete(T entity) =>
            _context.Set<T>().Remove(entity);

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync() =>
            await _context.SaveChangesAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Implementación del repositorio de usuarios.
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(GymDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<User?> GetByUserNameAsync(string userName) =>
            await _context.Users
                .FirstOrDefaultAsync(u => u.NormalizedUserName == userName.ToUpperInvariant());

        /// <inheritdoc/>
        public async Task<User?> GetByEmailAsync(string email) =>
            await _context.Users
                .FirstOrDefaultAsync(u => u.NormalizedEmail == email.ToUpperInvariant());

        /// <inheritdoc/>
        public async Task<User?> GetWithRolesAsync(int userId) =>
            await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

        /// <inheritdoc/>
        public async Task<bool> ExistsUserNameAsync(string userName) =>
            await _context.Users
                .AnyAsync(u => u.NormalizedUserName == userName.ToUpperInvariant());

        /// <inheritdoc/>
        public async Task<bool> ExistsEmailAsync(string email) =>
            await _context.Users
                .AnyAsync(u => u.NormalizedEmail == email.ToUpperInvariant());
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Implementación del repositorio de roles.
    /// </summary>
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(GymDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<Role?> GetByNameAsync(string name) =>
            await _context.Roles
                .FirstOrDefaultAsync(r => r.NormalizedName == name.ToUpperInvariant());
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Implementación del repositorio de socios.
    /// </summary>
    public class SocioRepository : Repository<Socio>, ISocioRepository
    {
        public SocioRepository(GymDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<Socio?> GetWithUserAsync(int socioId) =>
            await _context.Socios
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.SocioId == socioId);

        /// <inheritdoc/>
        public async Task<Socio?> GetByUserIdAsync(int userId) =>
            await _context.Socios
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserId == userId);

        /// <inheritdoc/>
        public async Task<(List<Socio> Items, int Total)> GetPagedAsync(int page, int pageSize)
        {
            var query = _context.Socios
                .Include(s => s.User)
                .OrderBy(s => s.SocioId);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        /// <inheritdoc/>
        public async Task<List<Socio>> GetByEntrenadorIdAsync(int entrenadorId) =>
            await _context.SocioEntrenadores
                .Where(se => se.EntrenadorId == entrenadorId && se.Activo)
                .Include(se => se.Socio)
                    .ThenInclude(s => s.User)
                .Select(se => se.Socio)
                .ToListAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Implementación del repositorio de entrenadores.
    /// </summary>
    public class EntrenadorRepository : Repository<Entrenador>, IEntrenadorRepository
    {
        public EntrenadorRepository(GymDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<Entrenador?> GetWithUserAsync(int entrenadorId) =>
            await _context.Entrenadores
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.EntrenadorId == entrenadorId);

        /// <inheritdoc/>
        public async Task<Entrenador?> GetByUserIdAsync(int userId) =>
            await _context.Entrenadores
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);

        /// <inheritdoc/>
        public async Task<(List<Entrenador> Items, int Total)> GetPagedAsync(int page, int pageSize)
        {
            var query = _context.Entrenadores
                .Include(e => e.User)
                .OrderBy(e => e.EntrenadorId);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Implementación del repositorio de planes de membresía.
    /// </summary>
    public class MembresiaRepository : Repository<Membresia>, IMembresiaRepository
    {
        public MembresiaRepository(GymDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<List<Membresia>> GetActivasAsync() =>
            await _context.Membresias
                .Where(m => m.IsActive)
                .OrderBy(m => m.Precio)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<bool> ExistsNombreAsync(string nombre, int? excludeId = null) =>
            await _context.Membresias
                .AnyAsync(m => m.Nombre == nombre && (excludeId == null || m.MembresiaId != excludeId));
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Implementación del repositorio de suscripciones de membresías.
    /// </summary>
    public class SocioMembresiaRepository : Repository<SocioMembresia>, ISocioMembresiaRepository
    {
        public SocioMembresiaRepository(GymDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<List<SocioMembresia>> GetBySocioIdAsync(int socioId) =>
            await _context.SocioMembresias
                .Include(sm => sm.Membresia)
                .Where(sm => sm.SocioId == socioId)
                .OrderByDescending(sm => sm.FechaInicio)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<SocioMembresia?> GetActivaBySocioIdAsync(int socioId) =>
            await _context.SocioMembresias
                .Include(sm => sm.Membresia)
                .Where(sm => sm.SocioId == socioId && sm.Estado == "ACTIVA")
                .OrderByDescending(sm => sm.FechaFin)
                .FirstOrDefaultAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Implementación del repositorio de asistencias.
    /// </summary>
    public class AsistenciaRepository : Repository<Asistencia>, IAsistenciaRepository
    {
        public AsistenciaRepository(GymDbContext context) : base(context) { }

        /// <inheritdoc/>
        public async Task<(List<Asistencia> Items, int Total)> GetBySocioIdPagedAsync(
            int socioId, int page, int pageSize)
        {
            var query = _context.Asistencias
                .Include(a => a.Socio).ThenInclude(s => s.User)
                .Include(a => a.RegistradaPorUser)
                .Where(a => a.SocioId == socioId)
                .OrderByDescending(a => a.FechaHoraEntrada);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        /// <inheritdoc/>
        public async Task<Asistencia?> GetSesionAbiertaAsync(int socioId) =>
            await _context.Asistencias
                .Where(a => a.SocioId == socioId && a.FechaHoraSalida == null)
                .OrderByDescending(a => a.FechaHoraEntrada)
                .FirstOrDefaultAsync();
        
        public async Task<Asistencia?> GetWithDetailsAsync(int asistenciaId) =>
            await _context.Asistencias
                .Include(a => a.Socio).ThenInclude(s => s.User)
                .Include(a => a.RegistradaPorUser)
                .FirstOrDefaultAsync(a => a.AsistenciaId == asistenciaId);
    }
}