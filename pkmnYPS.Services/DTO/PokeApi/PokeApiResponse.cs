namespace pkmnYPS.Services.DTO.PokeApi;

public sealed class PokeApiResponse
{
    public string name { get; set; }

    public Sprite sprites { get; set; }

    public int height { get; set; }

    public int weight { get; set; }

    public List<Ability> abilities { get; set; }

    public List<Type> types { get; set; }

    public PokeApiResponse()
    {
        name = "MISSINGNO";
        sprites = new();
        abilities = [];
        types = [];
    }

    public class Type
    {
        public TypeDetails type { get; set; }

        public Type()
        {
            type = new();
        }

        public class TypeDetails
        {
            public string name { get; set; }

            public TypeDetails()
            {
                name = string.Empty;
            }
        }
    }

    public class Ability
    {
        public AbilityDetails ability { get; set; }

        public Ability()
        {
            ability = new();
        }

        public class AbilityDetails
        {
            public string name { get; set; }

            public AbilityDetails()
            {
                name = string.Empty;
            }
        }
    }

    public class Sprite
    {
        public string front_default { get; set; }

        public Sprite()
        {
            front_default = string.Empty;
        }
    }
}
