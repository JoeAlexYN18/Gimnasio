namespace Gimnasio.DTOs
{
    /// <summary>
    /// Envoltorio estándar para todas las respuestas de la API.
    /// Garantiza un formato consistente en éxito y error.
    /// </summary>
    /// <typeparam name="T">Tipo del dato devuelto en caso de éxito.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>Indica si la operación fue exitosa.</summary>
        public bool Success { get; set; }

        /// <summary>Mensaje descriptivo del resultado.</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Dato de respuesta. Null en caso de error.</summary>
        public T? Data { get; set; }

        /// <summary>Lista de errores de validación. Vacía en caso de éxito.</summary>
        public List<string> Errors { get; set; } = new();

        /// <summary>Crea una respuesta exitosa con datos.</summary>
        public static ApiResponse<T> Ok(T data, string message = "Operación exitosa") =>
            new() { Success = true, Message = message, Data = data };

        /// <summary>Crea una respuesta de error con mensajes detallados.</summary>
        public static ApiResponse<T> Fail(string message, List<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors ?? new() };
    }

    /// <summary>
    /// Respuesta paginada para endpoints de listado.
    /// </summary>
    /// <typeparam name="T">Tipo de los elementos de la lista.</typeparam>
    public class PagedResponse<T>
    {
        /// <summary>Lista de elementos de la página actual.</summary>
        public List<T> Items { get; set; } = new();

        /// <summary>Número de página actual (base 1).</summary>
        public int Page { get; set; }

        /// <summary>Cantidad de elementos por página.</summary>
        public int PageSize { get; set; }

        /// <summary>Total de elementos en la consulta sin paginar.</summary>
        public int TotalCount { get; set; }

        /// <summary>Total de páginas disponibles.</summary>
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        /// <summary>Indica si existe una página anterior.</summary>
        public bool HasPrevious => Page > 1;

        /// <summary>Indica si existe una página siguiente.</summary>
        public bool HasNext => Page < TotalPages;
    }

    /// <summary>
    /// Parámetros de paginación recibidos desde el query string.
    /// </summary>
    public class PaginacionDto
    {
        private int _page = 1;
        private int _pageSize = 20;
        private const int MaxPageSize = 100;

        /// <summary>Número de página (mínimo 1).</summary>
        public int Page
        {
            get => _page;
            set => _page = value < 1 ? 1 : value;
        }

        /// <summary>Tamaño de página (máximo 100).</summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 1 : value;
        }
    }
}