using Gimnasio.Entities;

namespace Gimnasio.Repositories
{
    /// <summary>
    /// Repositorio base con operaciones CRUD genéricas.
    /// Cada repositorio concreto extiende esta interfaz con consultas específicas.
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad de dominio.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>Obtiene una entidad por su clave primaria.</summary>
        Task<T?> GetByIdAsync(int id);

        /// <summary>Obtiene todas las entidades de la tabla.</summary>
        Task<List<T>> GetAllAsync();

        /// <summary>Agrega una nueva entidad al contexto (pendiente de commit).</summary>
        Task AddAsync(T entity);

        /// <summary>Marca una entidad como modificada (pendiente de commit).</summary>
        void Update(T entity);

        /// <summary>Marca una entidad para eliminación (pendiente de commit).</summary>
        void Delete(T entity);

        /// <summary>Persiste todos los cambios pendientes en la base de datos.</summary>
        Task<int> SaveChangesAsync();
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Repositorio de usuarios. Gestiona cuentas de autenticación.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>Busca un usuario por nombre de usuario (case-insensitive).</summary>
        Task<User?> GetByUserNameAsync(string userName);

        /// <summary>Busca un usuario por correo electrónico (case-insensitive).</summary>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Obtiene un usuario junto con sus roles asignados.
        /// Usado durante la autenticación para construir los claims del JWT.
        /// </summary>
        Task<User?> GetWithRolesAsync(int userId);

        /// <summary>Verifica si ya existe un usuario con el nombre dado.</summary>
        Task<bool> ExistsUserNameAsync(string userName);

        /// <summary>Verifica si ya existe un usuario con el email dado.</summary>
        Task<bool> ExistsEmailAsync(string email);
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Repositorio de roles del sistema.
    /// </summary>
    public interface IRoleRepository : IRepository<Role>
    {
        /// <summary>Busca un rol por nombre exacto.</summary>
        Task<Role?> GetByNameAsync(string name);
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Repositorio de socios. Incluye consultas paginadas y con navegación.
    /// </summary>
    public interface ISocioRepository : IRepository<Socio>
    {
        /// <summary>
        /// Obtiene un socio con su User incluido.
        /// Usado para devolver datos completos al cliente.
        /// </summary>
        Task<Socio?> GetWithUserAsync(int socioId);

        /// <summary>Obtiene el perfil de socio asociado a un UserId.</summary>
        Task<Socio?> GetByUserIdAsync(int userId);

        /// <summary>
        /// Lista socios con paginación.
        /// Incluye el User asociado para poblar el DTO de respuesta.
        /// </summary>
        Task<(List<Socio> Items, int Total)> GetPagedAsync(int page, int pageSize);

        /// <summary>
        /// Obtiene los socios asignados a un entrenador específico.
        /// Usado por el rol ENTRENADOR para ver sus socios.
        /// </summary>
        Task<List<Socio>> GetByEntrenadorIdAsync(int entrenadorId);
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Repositorio de entrenadores.
    /// </summary>
    public interface IEntrenadorRepository : IRepository<Entrenador>
    {
        /// <summary>Obtiene un entrenador con su User incluido.</summary>
        Task<Entrenador?> GetWithUserAsync(int entrenadorId);

        /// <summary>Obtiene el perfil de entrenador asociado a un UserId.</summary>
        Task<Entrenador?> GetByUserIdAsync(int userId);

        /// <summary>Lista entrenadores con paginación, incluyendo su User.</summary>
        Task<(List<Entrenador> Items, int Total)> GetPagedAsync(int page, int pageSize);
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Repositorio de planes de membresía (catálogo).
    /// </summary>
    public interface IMembresiaRepository : IRepository<Membresia>
    {
        /// <summary>Obtiene solo los planes activos del catálogo.</summary>
        Task<List<Membresia>> GetActivasAsync();

        /// <summary>Verifica si ya existe un plan con el nombre dado.</summary>
        Task<bool> ExistsNombreAsync(string nombre, int? excludeId = null);
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Repositorio de suscripciones de membresías por socio.
    /// </summary>
    public interface ISocioMembresiaRepository : IRepository<SocioMembresia>
    {
        /// <summary>
        /// Obtiene el historial completo de membresías de un socio,
        /// incluyendo el nombre del plan.
        /// </summary>
        Task<List<SocioMembresia>> GetBySocioIdAsync(int socioId);

        /// <summary>Obtiene la membresía activa actual de un socio. Null si no tiene.</summary>
        Task<SocioMembresia?> GetActivaBySocioIdAsync(int socioId);
    }

    // ─────────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Repositorio de registros de asistencia.
    /// </summary>
    public interface IAsistenciaRepository : IRepository<Asistencia>
    {
        /// <summary>
        /// Obtiene el historial de asistencias de un socio con paginación.
        /// Incluye el nombre del usuario que registró la asistencia.
        /// </summary>
        Task<(List<Asistencia> Items, int Total)> GetBySocioIdPagedAsync(
            int socioId, int page, int pageSize);

        /// <summary>
        /// Obtiene la sesión abierta (sin salida) de un socio.
        /// Null si el socio no está actualmente en el gimnasio.
        /// </summary>
        Task<Asistencia?> GetSesionAbiertaAsync(int socioId);

        /// <summary>Obtiene una asistencia con Socio, User y RegistradaPorUser incluidos.</summary>
        Task<Asistencia?> GetWithDetailsAsync(int asistenciaId);
    }
}