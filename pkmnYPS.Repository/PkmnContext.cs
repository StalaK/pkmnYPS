using Microsoft.EntityFrameworkCore;
using pkmnYPS.Repository.Configurations;
using pkmnYPS.Services.RepositoryModels;

namespace pkmnYPS.Repository;

public class PkmnContext(DbContextOptions<PkmnContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    public DbSet<UserSearch> UserSearches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UserSearchConfiguration());
    }
}
