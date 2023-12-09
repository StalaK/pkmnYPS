namespace pkmnYPS.Services.DTO.ApiResponse;

public sealed class LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;

    public long Expiry { get; set; }
}
