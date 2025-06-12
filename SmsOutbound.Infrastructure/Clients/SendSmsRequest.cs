namespace SmsOutbound.Infrastructure.Clients;

public class SendSmsRequest
{
	public required string PhoneNumber { get; init; }

	public required string Message { get; init; }

	// Other fields can be added as needed, such as sender ID, timestamp, etc.
}