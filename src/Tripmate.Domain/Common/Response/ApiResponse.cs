namespace Tripmate.Domain.Common.Response
{

    /// <summary>
    /// Represents a standardized API response structure.
    /// </summary>
    /// <typeparam name="TData">The type of the data returned in the response.</typeparam>
    public class ApiResponse<TData>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public TData? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public ApiResponse(bool success, int statusCode, string message = "", TData? data = default, List<string>? errors = null)
        {
            Success = success;
            StatusCode = statusCode;
            Message = message;
            Data = data;
            Errors = errors ?? new List<string>();
        }

        public ApiResponse(TData data)
        {
            Success = true;
            StatusCode = 200; // OK
            Message = string.Empty;
            Data = data;

        }





    }
}
