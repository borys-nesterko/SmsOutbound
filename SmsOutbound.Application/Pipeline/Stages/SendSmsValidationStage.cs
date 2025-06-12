using SmsOutbound.Core.Abstractions;
using Microsoft.Extensions.Logging;
using SmsOutbound.Core.Commands;

namespace SmsOutbound.Application.Pipeline.Stages;

public class SendSmsValidationStage(
	IPhoneNumberValidator phoneNumberValidator,
	ILogger<SendSmsValidationStage> logger) : IPipelineStage<SendSmsCommand>
{
	public string StageName => "SendSmsValidationStage";
	
	public ValueTask<CommandContext<SendSmsCommand>> ProcessAsync(
		CommandContext<SendSmsCommand> context,
		CancellationToken cancellationToken)
	{
		if (context.Command.Message is null or { Length: 0 })
		{
			context.MarkAsFailed("Message cannot be null or empty.");
			logger.LogWarning("Validation failed for command {CommandId}: {ErrorMessage}",
				context.CommandId, context.ErrorMessage);

			return ValueTask.FromResult(context);
		}
		if (!phoneNumberValidator.IsValid(context.Command.PhoneNumber))
		{
			context.MarkAsFailed($"Invalid phone number: {context.Command.PhoneNumber}");
			logger.LogWarning("Phone number validation failed for command {CommandId}: {ErrorMessage}",
				context.CommandId, context.ErrorMessage);
		}

		return ValueTask.FromResult(context);
	}
}