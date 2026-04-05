namespace Gimnasio.ValueObjects
{
    /// <summary>
    /// Clase de constantes de dominio para el campo <c>Genero</c> en <see cref="Entities.Socio"/>.
    /// Refleja la restricción CHECK de PostgreSQL:
    /// <c>CHECK ("Genero" IN ('M','F','O') OR "Genero" IS NULL)</c>.
    ///
    /// <para>
    /// Uso: llamar a <see cref="IsValid"/> antes de asignar un valor a
    /// <see cref="Entities.Socio.Genero"/> para garantizar que solo códigos aceptados lleguen a la BD.
    /// </para>
    /// </summary>
    public static class Genero
    {
        /// <summary>Masculino.</summary>
        public const string Masculino = "M";

        /// <summary>Femenino.</summary>
        public const string Femenino = "F";

        /// <summary>Otro / prefiere no decir.</summary>
        public const string Otro = "O";

        private static readonly HashSet<string> _valid =
            new(StringComparer.Ordinal) { Masculino, Femenino, Otro };

        /// <summary>
        /// Devuelve <c>true</c> cuando <paramref name="value"/> es uno de los códigos de género
        /// aceptados ('M', 'F', 'O') o es <c>null</c> (no especificado).
        /// </summary>
        /// <param name="value">Valor a validar. <c>null</c> está explícitamente permitido.</param>
        public static bool IsValid(string? value) =>
            value is null || _valid.Contains(value);

        /// <summary>
        /// Lanza una <see cref="ArgumentException"/> cuando <paramref name="value"/>
        /// no es un código de género válido y no es <c>null</c>.
        /// </summary>
        /// <param name="value">Valor a validar.</param>
        /// <param name="paramName">
        /// Nombre opcional del parámetro usado en el mensaje de la excepción.
        /// Por defecto es <c>"Genero"</c>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Se lanza cuando <paramref name="value"/> no es nulo y no está en ('M','F','O').
        /// </exception>
        public static void Validate(string? value, string paramName = "Genero")
        {
            if (!IsValid(value))
                throw new ArgumentException(
                    $"Invalid Genero value '{value}'. Allowed: 'M', 'F', 'O', or null.",
                    paramName);
        }
    }
}
