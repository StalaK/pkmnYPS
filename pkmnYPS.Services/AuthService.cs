using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using pkmnYPS.Services.DTO.ApiRequest;
using pkmnYPS.Services.DTO.ApiResponse;
using pkmnYPS.Services.Interfaces.Repository;
using pkmnYPS.Services.Interfaces.Services;
using pkmnYPS.Services.RepositoryModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace pkmnYPS.Services;

public sealed class AuthService(
    ILogger<PokemonFetcherService> logger,
    IUserRepository userRepository,
    IConfiguration config)
    : IAuthService
{
    private readonly ILogger<PokemonFetcherService> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IConfiguration _config = config;

    public async Task<ApiResponse<LoginResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        try
        {
            var user = await _userRepository.GetUser(request.Email, ct);
            if (user is null)
                return new(400, "Invalid credentials");

            var userSalt = user.Salt;
            var passwordHash = HashPassword(request.Password, ref userSalt);

            if (user.Password != passwordHash)
                return new(400, "Invalid credentials");

            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(30);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, request.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Exp, accessTokenExpiry.Ticks.ToString()),
                new(ClaimTypes.NameIdentifier, request.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Authentication:JwtKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Authentication:JwtIssuer"],
                _config["Authentication:JwtAudience"],
                claims,
                expires: accessTokenExpiry,
                signingCredentials: creds);

            return new(new()
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiry = accessTokenExpiry.Ticks
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred whilst logging in. {loginRequest}", request);
            return new(500, "An unknown error occurred");
        }
    }

    public async Task<ApiResponse<string>> RegisterUser(RegisterRequest request, CancellationToken ct)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return new(400, "Please provide an email address");

            var emailRegex = new Regex(@"^\S{1,}@\S{1,}[.]\S{1,}$");
            if (!emailRegex.IsMatch(request.Email))
                return new(400, "Please provide a valid email address");

            if (string.IsNullOrWhiteSpace(request.Password))
                return new(400, "Please provide a password");

            if (await _userRepository.UserExists(request.Email, ct))
                return new(400, "User already exists. Please login.");

            var salt = string.Empty;
            var passwordHash = HashPassword(request.Password, ref salt);

            var newUser = new User(request.Email, passwordHash, salt);

            await _userRepository.CreateUser(newUser, ct);
            await _userRepository.Save(ct);

            return new("User registered successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred registering a user with the email address {email}", request.Email);
            return new(500, "Registration failed due to a server error");
        }
    }

    private static string HashPassword(string password, ref string salt)
    {
        const int SaltSize = 128 / 8;

        if (string.IsNullOrEmpty(salt))
        {
            var newSalt = new byte[SaltSize];
            RandomNumberGenerator.Fill(newSalt);

            salt = Encoding.UTF8.GetString(newSalt);
        }

        var saltBytes = Encoding.UTF8.GetBytes(salt);
        var key = KeyDerivation.Pbkdf2(password, saltBytes, KeyDerivationPrf.HMACSHA256, 100000, SaltSize);
        return Convert.ToBase64String(key);
    }
}
