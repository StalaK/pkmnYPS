using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pkmnYPS.Services.RepositoryModels;

namespace pkmnYPS.Repository.Configurations;

internal class UserSearchConfiguration : IEntityTypeConfiguration<UserSearch>
{
    public void Configure(EntityTypeBuilder<UserSearch> builder)
    {
        builder.Property(p => p.ID)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(p => p.SearchTerm)
            .HasColumnName("SearchTerm")
            .HasColumnType("NVARCHAR(256)")
            .IsRequired();

        builder.Property(p => p.Timestamp)
            .HasColumnName("Timestamp")
            .HasColumnType("TIMESTAMP")
            .IsRequired(true);
    }
}
