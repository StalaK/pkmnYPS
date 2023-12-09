using pkmnYPS.Services.DTO.PokeApi;

namespace pkmnYPS.Services.Interfaces.PokeAPI;

public interface IPokeAPI
{
    Task<PokeApiResponse?> GetPokemon(string searchTerm, CancellationToken ct);
}
