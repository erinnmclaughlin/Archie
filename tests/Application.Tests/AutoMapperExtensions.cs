using AutoMapper;

namespace Archie.Application.Tests;
public static class AutoMapperExtensions
{
    public static IMapper CreateMapper<T>(this T profile) where T : Profile
    {
        return new MapperConfiguration(cfg => cfg.AddProfile(profile)).CreateMapper();
    }
}
