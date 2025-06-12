namespace SmsOutbound.Core.Entities;

/// <summary>
/// Represents a log entry for an SMS message sent or attempted to be sent.
/// Can be used to debug issues with sending messages or to track message history.
/// Also outbox pattern can be implemented using this entity.
/// </summary>
public sealed class MessageLog
{
	public Guid MessageId { get; init; }

	public required string PhoneNumber { get; init; }

	public required string Message { get; init; }

	public DateTime CreatedAt { get; init; }

	public DateTime? SentAt { get; private set; }

	public bool IsSent => SentAt is not null;

	public static MessageLog Create(Guid messageId, string phoneNumber, string message, DateTime createdAt)
	{
		return new MessageLog
		{
			MessageId = messageId,
			PhoneNumber = phoneNumber,
			Message = message,
			CreatedAt = createdAt,
		};
	}

	public void MarkAsSent()
	{
		SentAt = DateTime.UtcNow;
	}
}