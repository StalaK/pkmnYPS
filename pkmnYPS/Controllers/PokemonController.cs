using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pkmnYPS.Services.Interfaces.Services;
using System.Security.Claims;

namespace pkmnYPS.Controllers;

[ApiController]
[Authorize]
public class PokemonController(IPokemonFetcherService pokemonFetcherService) : ControllerBase
{
    private readonly IPokemonFetcherService _pokemonFetcherService = pokemonFetcherService;

    [HttpGet]
    [Route("[controller]/{pokedexNo:int}")]
    public async Task<IActionResult> GetByPokedexNo(int pokedexNo, CancellationToken ct)
    {
        var userEmail = HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        var response = await _pokemonFetcherService.GetPokemon(pokedexNo, userEmail, ct);

        return response.Error
            ? StatusCode(response.HttpResponseCode, response)
            : Ok(response);
    }

    [HttpGet]
    [Route("[controller]/{pokemonName}")]
    public async Task<IActionResult> GetByName(string pokemonName, CancellationToken ct)
    {
        var userEmail = HttpContext?.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        var response = await _pokemonFetcherService.GetPokemon(pokemonName, userEmail, ct);

        return response.Error
            ? StatusCode(response.HttpResponseCode, response)
            : Ok(response);
    }
}
