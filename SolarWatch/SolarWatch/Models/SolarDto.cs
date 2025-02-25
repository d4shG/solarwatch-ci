using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SolarWatch.Models;

public record SolarDto
{
	[JsonPropertyName("sunrise")]
	public required string Sunrise { get; init; }
	[JsonPropertyName("sunset")]
	public required string Sunset { get; init; }
}