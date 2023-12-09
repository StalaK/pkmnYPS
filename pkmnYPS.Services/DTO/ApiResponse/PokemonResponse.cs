namespace pkmnYPS.Services.DTO.ApiResponse;

public sealed class PokemonResponse
{
    public string Name { get; set; }

    public string Sprite { get; set; }

    public int Height { get; set; }

    public int Weight { get; set; }

    public List<string> Abilities { get; set; }

    public List<string> Types { get; set; }

    public PokemonResponse()
    {
        Name = "MISSINGNO";
        Sprite = string.Empty;
        Abilities = [];
        Types = [];
    }
}
