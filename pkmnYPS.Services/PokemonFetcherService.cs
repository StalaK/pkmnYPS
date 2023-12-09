using Microsoft.Extensions.Logging;
using pkmnYPS.Services.DTO.ApiResponse;
using pkmnYPS.Services.Interfaces.PokeAPI;
using pkmnYPS.Services.Interfaces.Repository;
using pkmnYPS.Services.Interfaces.Services;
using pkmnYPS.Services.RepositoryModels;

namespace pkmnYPS.Services;

public sealed class PokemonFetcherService(
    ILogger<PokemonFetcherService> logger,
    IPokeAPI pokeAPI,
    IUserRepository userRepository,
    IUserSearchRepository userSearchRepository) : IPokemonFetcherService
{
    private readonly ILogger<PokemonFetcherService> _logger = logger;
    private readonly IPokeAPI _pokeAPI = pokeAPI;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserSearchRepository _userSearchRepository = userSearchRepository;

    public async Task<ApiResponse<PokemonResponse>> GetPokemon(string searchTerm, string userEmail, CancellationToken ct)
    {
        User? user = null;

        try
        {
            user = await _userRepository.GetUser(userEmail, ct);
            if (user is null)
                return new(500, "Unable to retrieve the current logged in user");

            var pokemon = await _pokeAPI.GetPokemon(searchTerm, ct);

            return pokemon is null
                ? new(404, "The selected pokemon could not be found")
                : new(new()
                {
                    Name = pokemon.name,
                    Sprite = pokemon.sprites.front_default,
                    Height = pokemon.height,
                    Weight = pokemon.weight,
                    Abilities = pokemon.abilities.ConvertAll(a => a.ability.name),
                    Types = pokemon.types.ConvertAll(a => a.type.name)
                });
        }
        catch (ArgumentException)
        {
            return new(404, "The Pokemon could not be found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred retrieving the pokemon");
            return new(500, "An unknown error occurred retrieving the pokemon");
        }
        finally
        {
            if (user is not null)
            {
                try
                {
                    var userSearch = new UserSearch(user, searchTerm);
                    await _userSearchRepository.AddSearch(userSearch, ct);
                    await _userSearchRepository.Save(ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred writing the search term {searchTerm} for user {userEmail} to the UserSearch table", searchTerm, userEmail);
                }
            }
        }
    }

    public async Task<ApiResponse<PokemonResponse>> GetPokemon(int pokedexNo, string userEmail, CancellationToken ct) => await GetPokemon($"{pokedexNo}", userEmail, ct);
}
