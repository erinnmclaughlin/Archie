namespace Archie.Api.Common;

public interface ICurrentUserService
{
    long Id { get; }
}

public class DumbCurrentUserService : ICurrentUserService
{
    // obviously this would be different IRL
    public long Id => 1;
}
