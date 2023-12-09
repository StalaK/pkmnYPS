using Microsoft.Extensions.Logging;
using pkmnYPS.Services.DTO.PokeApi;
using pkmnYPS.Services.Interfaces.PokeAPI;
using System.Net;
using System.Net.Http.Json;

namespace pkmnYPS.PokeAPI;

public sealed class PokeAPIService(ILogger<PokeAPIService> logger, HttpClient httpClient) : IPokeAPI
{
    private readonly ILogger<PokeAPIService> _logger = logger;
    private readonly HttpClient _httpClient = httpClient;

    public async Task<PokeApiResponse?> GetPokemon(string searchTerm, CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.GetAsync($"https://pokeapi.co/api/v2/pokemon/{searchTerm.ToLower()}/", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new ArgumentException("The pokemon was not found");
            else
                response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<PokeApiResponse>(ct);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "The PokeAPI did not return a successful HTTP response");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred fetching data from the PokeAPI");
            throw;
        }
    }

}
