namespace Gimnasio.Helpers
{
    public static class PeruDateTimeHelper
    {
        // "SA Pacific Standard Time" = Windows  |  "America/Lima" = Linux/macOS
        private static readonly TimeZoneInfo PeruZone =
            TimeZoneInfo.FindSystemTimeZoneById(
                OperatingSystem.IsWindows()
                    ? "SA Pacific Standard Time"
                    : "America/Lima");

        private const string Fmt = "yyyy-MM-dd HH:mm:ss";

        /// <summary>Convierte DateTime UTC a string con hora local de Perú.</summary>
        public static string ToPeruString(DateTime utc) =>
            TimeZoneInfo.ConvertTimeFromUtc(utc, PeruZone).ToString(Fmt);

        /// <summary>Versión nullable.</summary>
        public static string? ToPeruString(DateTime? utc) =>
            utc.HasValue ? ToPeruString(utc.Value) : null;

        /// <summary>
        /// Parsea un string "YYYY-MM-DD HH:MM:SS" ingresado en hora Perú
        /// y lo convierte a UTC para guardar en BD.
        /// </summary>
        public static DateTime ParsePeruToUtc(string peruStr)
        {
            var local = DateTime.ParseExact(
                peruStr, Fmt,
                System.Globalization.CultureInfo.InvariantCulture);
            return TimeZoneInfo.ConvertTimeToUtc(
                DateTime.SpecifyKind(local, DateTimeKind.Unspecified), PeruZone);
        }
    }
}