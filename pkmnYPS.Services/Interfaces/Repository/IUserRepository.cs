using pkmnYPS.Services.RepositoryModels;

namespace pkmnYPS.Services.Interfaces.Repository;

public interface IUserRepository
{
    Task<User?> GetUser(string email, CancellationToken ct);

    Task<bool> UserExists(string email, CancellationToken ct);

    Task CreateUser(User user, CancellationToken ct);

    Task Save(CancellationToken ct);
}
