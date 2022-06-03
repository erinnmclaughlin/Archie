using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Archie.Api.Database.Entities;

public class User
{
    public long Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    public class Configuration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(100).IsRequired();

            builder.HasData(GetSeedData());
        }

        public static List<User> GetSeedData()
        {
            return new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "Frank",
                    LastName = "Stein"
                }
            };
        }
    }
}
