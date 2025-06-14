using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmsOutbound.Core.Abstractions.External;

public class EventEnvelope
{
	[JsonPropertyName("eventId")]
	public Guid EventId { get; init; }

	[JsonPropertyName("correlationId")]
	public Guid CorrelationId { get; init; }

	[JsonPropertyName("createdAt")]
	public DateTime CreatedAt { get; init; }

	[JsonPropertyName("eventType")]
	public required string EventType { get; init; }

	[JsonPropertyName("content")]
	public required string Content { get; init; }

	// Generic version with specifying an exact IEvent object to deserialize should be provided.
	public object Unwrap()
	{
		return EventType switch
		{
			"SmsSent" => JsonSerializer.Deserialize<object>(Content)!,
			_ => throw new InvalidOperationException($"{EventType} event type is not supported"),
		};
	}
}