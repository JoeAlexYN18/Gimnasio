namespace Gimnasio.DTOs.Asistencias
{
    /// <summary>
    /// Datos para registrar el ingreso (check-in) de un socio al gimnasio.
    /// Accesible por ENTRENADOR y ADMIN.
    /// </summary>
    public class RegistrarAsistenciaDto
    {
        /// <summary>Identificador del socio que ingresa.</summary>
        public int SocioId { get; set; }

        /// <summary>
        /// Fecha y hora de entrada en hora local de Perú, formato "YYYY-MM-DD HH:MM:SS".
        /// Si no se envía, el servicio usa la hora actual.
        /// </summary>
        public string? FechaHoraEntrada { get; set; }

        /// <summary>Observaciones opcionales sobre la visita.</summary>
        public string? Observaciones { get; set; }
    }

    /// <summary>
    /// Datos para registrar la salida (check-out) de un socio.
    /// Accesible por ENTRENADOR y ADMIN.
    /// </summary>
    public class RegistrarSalidaDto
    {
        /// <summary>
        /// Fecha y hora de salida en hora local de Perú, formato "YYYY-MM-DD HH:MM:SS".
        /// Si no se envía, el servicio usa la hora actual.
        /// </summary>
        public string? FechaHoraSalida { get; set; }

        /// <summary>Observaciones adicionales al cerrar la sesión.</summary>
        public string? Observaciones { get; set; }
    }

    /// <summary>
    /// Respuesta con los datos de un registro de asistencia.
    /// </summary>
    public class AsistenciaResponseDto
    {
        /// <summary>Identificador del registro de asistencia.</summary>
        public int AsistenciaId { get; set; }

        /// <summary>Identificador del socio.</summary>
        public int SocioId { get; set; }

        /// <summary>Nombre de usuario del socio.</summary>
        public string SocioUserName { get; set; } = string.Empty;

        public string  FechaHoraEntrada { get; set; } = string.Empty;
        public string? FechaHoraSalida  { get; set; }

        /// <summary>Duración de la visita en minutos. Null si no ha salido.</summary>
        public int? DuracionMinutos { get; set; }

        /// <summary>Observaciones del registro.</summary>
        public string? Observaciones { get; set; }

        /// <summary>Nombre del usuario que registró la asistencia. Null si fue automático.</summary>
        public string? RegistradaPor { get; set; }
    }
}