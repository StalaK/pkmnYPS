namespace pkmnYPS.Services.RepositoryModels;

public class UserSearch
{
    public int ID { get; set; }

    public User User { get; set; }

    public string SearchTerm { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public UserSearch()
    {
        User = new();
    }

    public UserSearch(User user, string searchTerm)
    {
        User = user;
        SearchTerm = searchTerm;
        Timestamp = DateTime.UtcNow;
    }
}
