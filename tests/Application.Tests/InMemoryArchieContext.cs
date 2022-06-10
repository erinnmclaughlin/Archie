using Archie.Application.Database;
using Microsoft.EntityFrameworkCore;

namespace Archie.Application.Tests;

public class InMemoryArchieContext : ArchieContext
{
    private static DbContextOptions<ArchieContext> Options
    {
        get
        {
            var builder = new DbContextOptionsBuilder<ArchieContext>();
            builder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            return builder.Options;
        }
    }

    public InMemoryArchieContext() : base(Options)
    {
        Database.EnsureCreated();
    }
}
