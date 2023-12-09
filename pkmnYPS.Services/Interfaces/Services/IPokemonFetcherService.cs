using pkmnYPS.Services.DTO.ApiResponse;

namespace pkmnYPS.Services.Interfaces.Services;

public interface IPokemonFetcherService
{
    Task<ApiResponse<PokemonResponse>> GetPokemon(string searchTerm, string userEmail, CancellationToken ct);

    Task<ApiResponse<PokemonResponse>> GetPokemon(int pokedexNumber, string userEmail, CancellationToken ct);
}
