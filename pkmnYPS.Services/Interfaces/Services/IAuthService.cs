using pkmnYPS.Services.DTO.ApiRequest;
using pkmnYPS.Services.DTO.ApiResponse;

namespace pkmnYPS.Services.Interfaces.Services;

public interface IAuthService
{
    Task<ApiResponse<LoginResponse>> Login(LoginRequest request, CancellationToken ct);

    Task<ApiResponse<string>> RegisterUser(RegisterRequest request, CancellationToken ct);
}
