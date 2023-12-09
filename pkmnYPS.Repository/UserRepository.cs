using Microsoft.EntityFrameworkCore;
using pkmnYPS.Services.Interfaces.Repository;
using pkmnYPS.Services.RepositoryModels;

namespace pkmnYPS.Repository;

public sealed class UserRepository(PkmnContext context) : IUserRepository
{
    private readonly PkmnContext _context = context;

    public async Task<User?> GetUser(string email, CancellationToken ct) => await _context.Users.FirstOrDefaultAsync(x => x.Email == email, ct);

    public async Task<bool> UserExists(string email, CancellationToken ct) => await _context.Users.AnyAsync(x => x.Email == email, ct);

    public async Task CreateUser(User user, CancellationToken ct) => await _context.Users.AddAsync(user, ct);

    public async Task Save(CancellationToken ct) => await _context.SaveChangesAsync(ct);
}
