using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Archie.Api.Database.Entities;

public abstract class Audit
{
    public long Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public long UserId { get; set; }
    public User? User { get; set; }

    public abstract class Configuration<T> : IEntityTypeConfiguration<T> where T : Audit
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.Property(x => x.Description).HasMaxLength(200).IsRequired();
        }
    }
}
