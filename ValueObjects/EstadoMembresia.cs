namespace Gimnasio.ValueObjects
{
    /// <summary>
    /// Clase de constantes de dominio para el campo <c>Estado</c> en
    /// <see cref="Entities.SocioMembresia"/>.
    /// Refleja la restricción CHECK de PostgreSQL:
    /// <c>CHECK ("Estado" IN ('ACTIVA','VENCIDA','PAUSADA','CANCELADA'))</c>.
    ///
    /// <para>
    /// Uso: llamar a <see cref="IsValid"/> antes de asignar un valor a
    /// <see cref="Entities.SocioMembresia.Estado"/> para garantizar que solo estados
    /// de ciclo de vida aceptados lleguen a la BD.
    /// </para>
    /// </summary>
    public static class EstadoMembresia
    {
        /// <summary>La suscripción está actualmente activa.</summary>
        public const string Activa    = "ACTIVA";

        /// <summary>El periodo de suscripción ha expirado sin renovación.</summary>
        public const string Vencida   = "VENCIDA";

        /// <summary>La suscripción está temporalmente pausada (por ejemplo, licencia médica).</summary>
        public const string Pausada   = "PAUSADA";

        /// <summary>La suscripción fue cancelada explícitamente antes de su vencimiento.</summary>
        public const string Cancelada = "CANCELADA";

        private static readonly HashSet<string> _valid =
            new(StringComparer.Ordinal) { Activa, Vencida, Pausada, Cancelada };

        /// <summary>
        /// Devuelve <c>true</c> cuando <paramref name="value"/> es uno de los cuatro
        /// estados de ciclo de vida aceptados.
        /// </summary>
        /// <param name="value">Valor a validar.</param>
        public static bool IsValid(string? value) =>
            value is not null && _valid.Contains(value);

        /// <summary>
        /// Lanza una <see cref="ArgumentException"/> cuando <paramref name="value"/>
        /// no es un estado de ciclo de vida válido.
        /// </summary>
        /// <param name="value">Valor a validar.</param>
        /// <param name="paramName">
        /// Nombre opcional del parámetro usado en el mensaje de la excepción.
        /// Por defecto es <c>"Estado"</c>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Se lanza cuando <paramref name="value"/> no está en
        /// ('ACTIVA','VENCIDA','PAUSADA','CANCELADA').
        /// </exception>
        public static void Validate(string? value, string paramName = "Estado")
        {
            if (!IsValid(value))
                throw new ArgumentException(
                    $"Invalid Estado value '{value}'. " +
                    $"Allowed: '{Activa}', '{Vencida}', '{Pausada}', '{Cancelada}'.",
                    paramName);
        }
    }
}