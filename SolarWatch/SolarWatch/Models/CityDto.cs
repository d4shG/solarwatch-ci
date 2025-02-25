using System.Text.Json.Serialization;

namespace SolarWatch.Models;

public record CityDto()
{
[JsonPropertyName("lat")]
public required double Latitude { get; init; }
[JsonPropertyName("lon")]
public required double Longitude { get; init; }
}