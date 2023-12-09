namespace pkmnYPS.Services.RepositoryModels;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Salt { get; set; } = string.Empty;

    public ICollection<UserSearch> UserSearches { get; set; } = [];

    public User()
    {
    }

    public User(string email, string passwordHash, string salt)
    {
        Email = email;
        Password = passwordHash;
        Salt = salt;
    }
}
