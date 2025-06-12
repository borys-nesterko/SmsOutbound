using System.Text.Json;
using SmsOutbound.Core.Abstractions;

namespace SmsOutbound.Core.Commands;

// When consuming commands from the message queue this model must be distributed as a shared contract, i.e Nuget package
public sealed class SendSmsCommand : ICommand
{
	public Guid Id { get; init; }

	public required string PhoneNumber { get; init; }

	public required string Message { get; init; }

	public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

	public static SendSmsCommand Create(Guid commandId, string phoneNumber, string message, DateTime createdAt)
	{
		return new SendSmsCommand
		{
			Id = commandId,
			PhoneNumber = phoneNumber,
			Message = message,
			CreatedAt = createdAt,
		};
	}

	public string? GetContent() => JsonSerializer.Serialize(
	new
	{ 
		PhoneNumber,
		Message,
	});
}