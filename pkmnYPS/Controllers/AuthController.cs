using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pkmnYPS.Services.DTO.ApiRequest;
using pkmnYPS.Services.Interfaces.Services;

namespace pkmnYPS.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    private readonly IAuthService _authService = authService;

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var response = await _authService.Login(request, ct);

        return response.Error
            ? StatusCode(response.HttpResponseCode, response)
            : Ok(response);
    }

    [HttpPut]
    [AllowAnonymous]
    public async Task<ActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        var response = await _authService.RegisterUser(request, ct);

        return response.Error
            ? StatusCode(response.HttpResponseCode, response)
            : Ok(response);
    }
}
