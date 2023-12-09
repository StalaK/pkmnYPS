using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework.Legacy;
using pkmnYPS.Services;
using pkmnYPS.Services.DTO.PokeApi;
using pkmnYPS.Services.Interfaces.PokeAPI;
using pkmnYPS.Services.Interfaces.Repository;
using pkmnYPS.Services.Interfaces.Services;
using pkmnYPS.Services.RepositoryModels;

namespace UnitTests.pkmnYPS.Services;

public class Tests
{
    private ILogger<PokemonFetcherService> _logger;
    private IPokeAPI _pokeApi;
    private IUserRepository _userRepository;
    private IUserSearchRepository _userSearchRepository;
    private IPokemonFetcherService _sut;

    private PokeApiResponse _defaultPokeApiResponse;

    [SetUp]
    public void Setup()
    {
        _logger = Substitute.For<ILogger<PokemonFetcherService>>();
        _pokeApi = Substitute.For<IPokeAPI>();
        _userRepository = Substitute.For<IUserRepository>();
        _userSearchRepository = Substitute.For<IUserSearchRepository>();

        _sut = new PokemonFetcherService(_logger, _pokeApi, _userRepository, _userSearchRepository);

        _defaultPokeApiResponse = new()
        {
            name = "Pikachu",
            sprites = new() { front_default = "sprite.jpg" },
            height = 25,
            weight = 30,
            abilities =
            [
                new() { ability = new() { name = "static" } },
                new() { ability = new() { name = "lightning-rod" } }
            ],
            types =
            [
                new() { type = new() { name = "electric " } }
            ]
        };
    }

    [Test]
    public async Task GetPokemon_ValidSearchString_ReturnsPokemonSuccessfully()
    {
        // Arrange
        _userRepository.GetUser(Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(new User { Id = 1 });

        _pokeApi.GetPokemon(Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(_defaultPokeApiResponse);

        // Act
        var result = await _sut.GetPokemon("Pikachu", "test@test.co", CancellationToken.None);

        // Assert
        ClassicAssert.IsFalse(result.Error);
        ClassicAssert.IsTrue(
            result.Data!.Name.Equals(_defaultPokeApiResponse.name)
            && result.Data!.Sprite.Equals(_defaultPokeApiResponse.sprites.front_default)
            && result.Data!.Height.Equals(_defaultPokeApiResponse.height)
            && result.Data!.Weight.Equals(_defaultPokeApiResponse.weight)
            && result.Data!.Abilities.Count == _defaultPokeApiResponse.abilities.Count
            && result.Data!.Types.Count == _defaultPokeApiResponse.types.Count);
    }

    [Test]
    public async Task GetPokemon_ValidSearchInteger_ReturnsPokemonSuccessfully()
    {
        // Arrange
        _userRepository.GetUser(Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(new User { Id = 1 });

        _pokeApi.GetPokemon(Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(_defaultPokeApiResponse);

        // Act
        var result = await _sut.GetPokemon(25, "test@test.co", CancellationToken.None);

        // Assert
        ClassicAssert.IsFalse(result.Error);
        ClassicAssert.IsTrue(
            result.Data!.Name.Equals(_defaultPokeApiResponse.name)
            && result.Data!.Sprite.Equals(_defaultPokeApiResponse.sprites.front_default)
            && result.Data!.Height.Equals(_defaultPokeApiResponse.height)
            && result.Data!.Weight.Equals(_defaultPokeApiResponse.weight)
            && result.Data!.Abilities.Count == _defaultPokeApiResponse.abilities.Count
            && result.Data!.Types.Count == _defaultPokeApiResponse.types.Count);
    }

    [Test]
    public async Task GetPokemon_UserNotFound_ReturnsError()
    {
        // Arrange
        _userRepository.GetUser(Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(null as User);

        // Act
        var result = await _sut.GetPokemon("Pikachu", "test@test.co", CancellationToken.None);

        // Assert
        ClassicAssert.IsTrue(result.Error);
        ClassicAssert.AreEqual(500, result.HttpResponseCode);
        ClassicAssert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
    }

    [Test]
    public async Task GetPokemon_AllSearchesWithValidUser_AddNewUserSearchRecord()
    {
        // Arrange
        var userId = 1;
        var searchTerm = "Pikachu";

        _userRepository.GetUser(Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(new User { Id = userId });

        _pokeApi.GetPokemon(Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(_defaultPokeApiResponse);

        // Act
        var result = await _sut.GetPokemon(searchTerm, "test@test.co", CancellationToken.None);

        // Assert
        await _userSearchRepository
            .Received(1)
            .AddSearch(Arg.Is<UserSearch>(s =>
                s.User.Id == userId && s.SearchTerm == searchTerm),
                Arg.Any<CancellationToken>());

        await _userSearchRepository
            .Received(1)
            .Save(Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetPokemon_PokemonNotFound_ReturnsError()
    {
        // Arrange
        _userRepository.GetUser(Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(new User { Id = 1 });

        _pokeApi.GetPokemon(Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(null as PokeApiResponse);

        // Act
        var result = await _sut.GetPokemon("Pikachu", "test@test.co", CancellationToken.None);

        // Assert
        ClassicAssert.IsTrue(result.Error);
        ClassicAssert.AreEqual(404, result.HttpResponseCode);
        ClassicAssert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
    }

    [Test]
    public async Task GetPokemon_DatabaseException_ReturnsError()
    {
        // Arrange
        _userRepository.GetUser(Arg.Any<string>(), Arg.Any<CancellationToken>()).ThrowsAsync(new Exception("DB Error"));

        // Act
        var result = await _sut.GetPokemon("Pikachu", "test@test.co", CancellationToken.None);

        // Assert
        ClassicAssert.IsTrue(result.Error);
        ClassicAssert.AreEqual(500, result.HttpResponseCode);
        ClassicAssert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
    }

    [Test]
    public async Task GetPokemon_ExceptionAddingUserSearch_LogsAndContinues()
    {
        // Arrange
        _userRepository.GetUser(Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(new User { Id = 1 });

        _pokeApi.GetPokemon(Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(_defaultPokeApiResponse);

        _userSearchRepository.AddSearch(Arg.Any<UserSearch>(), Arg.Any<CancellationToken>()).ThrowsAsync(new Exception("DB Error"));

        // Act
        var result = await _sut.GetPokemon("Pikachu", "test@test.co", CancellationToken.None);

        // Assert
        _logger.Received(1).Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Any<object>(), Arg.Any<Exception>(), Arg.Any<Func<object, Exception?, string>>());

        ClassicAssert.IsFalse(result.Error);
        ClassicAssert.IsNotNull(result.Data);
    }


    [Test]
    public async Task GetPokemon_PokeApiThrowsArgumentException_Returns404Error()
    {
        // Arrange
        _userRepository.GetUser(Arg.Any<string>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(new User { Id = 1 });

        _pokeApi.GetPokemon(Arg.Any<string>(), Arg.Any<CancellationToken>()).ThrowsAsync(new ArgumentException("Pokemon not found"));

        // Act
        var result = await _sut.GetPokemon("Pikachu", "test@test.co", CancellationToken.None);

        // Assert
        ClassicAssert.IsTrue(result.Error);
        ClassicAssert.AreEqual(404, result.HttpResponseCode);
        ClassicAssert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
    }
}