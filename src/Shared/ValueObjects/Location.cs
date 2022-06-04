namespace Archie.Shared.ValueObjects;

public struct Location
{
    public string City { get; set; }
    public string Region { get; set; }
    public string Country { get; set; }

    public Location()
    {
        City = string.Empty;
        Region = string.Empty;
        Country = string.Empty;
    }

    public Location(string city, string region, string country)
    {
        City = city;
        Region = region;
        Country = country;
    }
}
