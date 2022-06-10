using Archie.Application.Database;
using Microsoft.EntityFrameworkCore;

namespace Archie.Application.Tests;

public class TestArchieContext : ArchieContext
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

    public TestArchieContext() : base(Options)
    {
        Database.EnsureCreated();
    }
}
