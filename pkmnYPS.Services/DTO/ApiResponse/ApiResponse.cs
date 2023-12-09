namespace pkmnYPS.Services.DTO.ApiResponse;

public sealed class ApiResponse<T>
{
    public T? Data { get; set; }

    public bool Error { get; set; }

    public int HttpResponseCode { get; set; }

    public string ErrorMessage { get; set; }

    public ApiResponse(T data)
    {
        Data = data;
        Error = false;
        HttpResponseCode = 200;
        ErrorMessage = string.Empty;
    }

    public ApiResponse(int httpResponse, string errorMessage)
    {
        Data = default;
        Error = true;
        HttpResponseCode = httpResponse;
        ErrorMessage = errorMessage;
    }
}
