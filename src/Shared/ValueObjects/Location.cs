 namespace Archie.Shared.ValueObjects;

public class Location
{
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? Country { get; set; }

    public Location()
    {
    }

    public Location(string? city, string? region, string? country)
    {
        City = city;
        Region = region;
        Country = country;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Location location) return false;
        return City == location.City && Region == location.Region && Country == location.Country;
    }

    public override int GetHashCode()
    {
        var city = City ?? string.Empty;
        var region = Region ?? string.Empty;
        var country = Country ?? string.Empty;

        return city.GetHashCode() ^ region.GetHashCode() ^ country.GetHashCode();
    }

    public override string ToString()
    {
        var words = new string?[] { City, Region, Country }.Where(w => !string.IsNullOrWhiteSpace(w));
        return string.Join(", ", words);
    }
}
