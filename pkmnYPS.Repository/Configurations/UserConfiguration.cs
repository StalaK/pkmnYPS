using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pkmnYPS.Services.RepositoryModels;

namespace pkmnYPS.Repository.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(p => p.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(p => p.Email)
            .HasColumnName("Email")
            .HasMaxLength(255)
            .HasColumnType("NVARCHAR(256)")
            .IsRequired();

        builder.Property(p => p.Password)
            .HasColumnName("Password")
            .HasColumnType("NVARCHAR(256)")
            .IsRequired();

        builder.Property(p => p.Salt)
            .HasColumnName("Salt")
            .HasColumnType("NVARCHAR(256)")
            .IsRequired();

        builder.HasMany<UserSearch>(u => u.UserSearches)
            .WithOne(s => s.User);
    }
}
