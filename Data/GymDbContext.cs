using Gimnasio.Entities;
using Gimnasio.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Data
{
    /// <summary>
    /// DbContext central de EF Core para la aplicación del gimnasio.
    /// Todos los conjuntos de entidades y sus configuraciones Fluent API
    /// se registran aquí.
    ///
    /// <para><b>Decisiones de diseño:</b></para>
    /// <list type="bullet">
    ///   <item>
    ///     Cada entidad tiene una clase dedicada <see cref="IEntityTypeConfiguration{T}"/>
    ///     en la carpeta <c>Configurations</c>. Esto mantiene <see cref="GymDbContext"/>
    ///     liviano y cada archivo de mapeo enfocado en una sola entidad.
    ///   </item>
    ///   <item>
    ///     <see cref="OnModelCreating"/> llama a
    ///     <see cref="ModelBuilder.ApplyConfigurationsFromAssembly"/> para detectar automáticamente
    ///     todas las clases de configuración — no se necesita registro manual al agregar nuevas entidades.
    ///   </item>
    ///   <item>
    ///     El proveedor de PostgreSQL (Npgsql.EntityFrameworkCore.PostgreSQL) debe
    ///     registrarse en <c>Program.cs</c> vía
    ///     <c>AddDbContext&lt;GymDbContext&gt;(o => o.UseNpgsql(connectionString))</c>.
    ///   </item>
    /// </list>
    /// </summary>
    public class GymDbContext : DbContext
    {
        /// <summary>
        /// Inicializa una nueva instancia de <see cref="GymDbContext"/> con las
        /// opciones suministradas (inyectadas por el contenedor de DI).
        /// </summary>
        /// <param name="options">
        /// Opciones de EF Core que incluyen el proveedor Npgsql y la cadena de conexión.
        /// </param>
        public GymDbContext(DbContextOptions<GymDbContext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        // ── DbSets ────────────────────────────────────────────────────────────────
        // Un DbSet por agregado raíz / entidad.
        // Las entidades de unión (UserRole, SocioEntrenador, RutinaEjercicio) se exponen
        // como DbSets para poder consultarlas y manipularlas directamente cuando se requiera.

        /// <summary>Roles del sistema (ADMIN, ENTRENADOR, SOCIO, …).</summary>
        public DbSet<Role> Roles => Set<Role>();

        /// <summary>Cuentas de autenticación para todos los usuarios.</summary>
        public DbSet<User> Users => Set<User>();

        /// <summary>Enlace muchos a muchos entre <see cref="User"/> y <see cref="Role"/>.</summary>
        public DbSet<UserRole> UserRoles => Set<UserRole>();

        /// <summary>Perfiles de entrenadores (extensión 1:1 de <see cref="User"/>).</summary>
        public DbSet<Entrenador> Entrenadores => Set<Entrenador>();

        /// <summary>Perfiles de socios (extensión 1:1 de <see cref="User"/>).</summary>
        public DbSet<Socio> Socios => Set<Socio>();

        /// <summary>Asignaciones muchos a muchos entre socios y entrenadores.</summary>
        public DbSet<SocioEntrenador> SocioEntrenadores => Set<SocioEntrenador>();

        /// <summary>Catálogo de planes de membresía.</summary>
        public DbSet<Membresia> Membresias => Set<Membresia>();

        /// <summary>Historial y suscripciones activas por socio.</summary>
        public DbSet<SocioMembresia> SocioMembresias => Set<SocioMembresia>();

        /// <summary>Registros de asistencia / check-in.</summary>
        public DbSet<Asistencia> Asistencias => Set<Asistencia>();

        /// <summary>Catálogo de ejercicios.</summary>
        public DbSet<Ejercicio> Ejercicios => Set<Ejercicio>();

        /// <summary>Rutinas de entrenamiento asignadas a socios.</summary>
        public DbSet<Rutina> Rutinas => Set<Rutina>();

        /// <summary>
        /// Entradas de ejercicios ordenadas dentro de una rutina, incluyendo parámetros de entrenamiento.
        /// </summary>
        public DbSet<RutinaEjercicio> RutinaEjercicios => Set<RutinaEjercicio>();

        // ── Construcción del modelo ──────────────────────────────────────────────

        /// <inheritdoc />
        /// <summary>
        /// Aplica todas las clases <see cref="IEntityTypeConfiguration{T}"/> encontradas
        /// en el mismo ensamblado que <see cref="RoleConfiguration"/>.
        /// Agregar una nueva entidad solo requiere:
        ///   1. Crear la clase de entidad en <c>Entities/</c>.
        ///   2. Crear su clase de configuración en <c>Configurations/</c>.
        ///   3. Agregar una propiedad <see cref="DbSet{T}"/> arriba.
        /// No se necesitan cambios en este método.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /// <summary>
            /// Escanea el ensamblado de Configurations para todas las implementaciones
            /// de IEntityTypeConfiguration y las aplica en una sola llamada.
            /// Esto reemplaza la necesidad de llamar individualmente
            /// a modelBuilder.ApplyConfiguration(new XyzConfig()) por cada entidad.
            /// </summary>
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(RoleConfiguration).Assembly
            );
        }

        // ── Opcional: auto-set CreatedAt al guardar ──────────────────────────────

        /// <summary>
        /// Sobrescribe <see cref="DbContext.SaveChangesAsync"/> para establecer automáticamente
        /// <c>CreatedAt</c> en las entidades recién agregadas.
        ///
        /// <para>
        /// Se recorren las entradas del <see cref="ChangeTracker"/> en estado <see cref="EntityState.Added"/>,
        /// asignando la fecha actual a las entidades que exponen la propiedad <c>CreatedAt</c>.
        /// </para>
        ///
        /// <para>
        /// Esta es una capa de conveniencia. Los valores por defecto de <c>CreatedAt</c>
        /// también pueden definirse a nivel de base de datos (por ejemplo, con
        /// <c>DEFAULT CURRENT_TIMESTAMP</c>), de modo que la BD actúe como fuente de verdad
        /// final si la aplicación no los establece.
        /// </para>
        /// </summary>
        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State != EntityState.Added) 
                    continue;

                switch (entry.Entity)
                {
                    case User u:
                        u.CreatedAt = now;
                        break;
                    case Role r:
                        r.CreatedAt = now;
                        break;
                    case Membresia m:
                        m.CreatedAt = now;
                        break;
                    case SocioMembresia sm:
                        sm.CreatedAt = now;
                        break;
                    case Rutina ru:
                        ru.CreatedAt = now;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}