using pkmnYPS.Services.RepositoryModels;

namespace pkmnYPS.Services.Interfaces.Repository;

public interface IUserSearchRepository
{
    Task AddSearch(UserSearch search, CancellationToken ct);

    Task Save(CancellationToken ct);
}
