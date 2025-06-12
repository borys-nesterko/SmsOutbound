using SmsOutbound.Core.Entities;
using SmsOutbound.Core.Abstractions;
using Microsoft.Extensions.Logging;
using SmsOutbound.Core.Commands;
using SmsOutbound.Core.Abstractions.External;

namespace SmsOutbound.Application.Pipeline.Stages;

public class SendSmsProcessingStage(
	IMessagingClient messagingClient,
	IMessageLogRepository messageLogRepository,
	ILogger<SendSmsProcessingStage> logger) : IPipelineStage<SendSmsCommand>
{
	public string StageName => "SendSmsProcessingStage";

	public async ValueTask<CommandContext<SendSmsCommand>> ProcessAsync(
		CommandContext<SendSmsCommand> context,
		CancellationToken cancellationToken)
	{
		var messageLog = new MessageLog
		{
			MessageId = context.Command.Id,
			PhoneNumber = context.Command.PhoneNumber,
			Message = context.Command.Message,
			CreatedAt = context.CreatedAt
		};

		var messageAdded = await messageLogRepository.TryAddMessageLogAsync(messageLog, out var existingMesage, cancellationToken);

		if (!messageAdded)
		{
			context.MarkAsFailed($"Message with ID {context.Command.Id} already exists. Is already send: {existingMesage!.IsSent}.");
			logger.LogWarning("Message log already exists for command {CommandId}: {ErrorMessage}",
				context.CommandId, context.ErrorMessage);

			return context;
		}

		var result = await messagingClient.SendSmsAsync(context.Command.PhoneNumber, context.Command.Message,cancellationToken);

		if (result.IsSuccess)
		{
			messageLog.MarkAsSent();
			await messageLogRepository.UpdateMessageLogAsync(messageLog);
		}
		else
		{
			context.MarkAsFailed(result.ErrorMessage!);
			logger.LogError("Failed to send SMS for command {CommandId}: {ErrorMessage}",
				context.CommandId, context.ErrorMessage);
			// Optionally, we could update the message log with the error details, for debugging purposes    
			return context;
		}
		
		return context;
	}
}