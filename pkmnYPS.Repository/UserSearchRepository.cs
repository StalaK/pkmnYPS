using pkmnYPS.Services.Interfaces.Repository;
using pkmnYPS.Services.RepositoryModels;

namespace pkmnYPS.Repository;

public class UserSearchRepository(PkmnContext context) : IUserSearchRepository
{
    private readonly PkmnContext _context = context;

    public async Task AddSearch(UserSearch search, CancellationToken ct) => await _context.UserSearches.AddAsync(search, ct);

    public async Task Save(CancellationToken ct) => await _context.SaveChangesAsync(ct);
}
