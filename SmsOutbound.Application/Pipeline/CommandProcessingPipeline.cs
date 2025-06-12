using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using SmsOutbound.Core.Abstractions;
using SmsOutbound.Core.Abstractions.External;

namespace SmsOutbound.Application.Pipeline;

public sealed class CommandProcessingPipeline<TCommand>(
	IPipelineStage<TCommand>[] stages,
	ILogger<CommandProcessingPipeline<TCommand>> logger) 
	: ICommandProcessingPipeline<TCommand>
		where TCommand : ICommand
{
	private readonly Channel<EventEnvelope> eventChannel = Channel.CreateUnbounded<EventEnvelope>();

	public async Task<CommandContext<TCommand>> ProcessAsync(TCommand command, CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(command);

		var context = CommandContext<TCommand>.Create(command);
		var stopwatch = Stopwatch.StartNew();

		foreach (var stage in stages)
		{
			try
			{
				context.EnterStage(stage.StageName);
				context = await stage.ProcessAsync(context, cancellationToken);

				stopwatch.Reset(); // Metrics can be collected here and pushed to a monitoring system

				if (!context.IsSuccessful)
				{
					return context;
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error occurred while processing the command {CommandId} at stage {StageName}.",
					command.Id, stage.StageName);

				context.MarkAsFailed(ex.Message);

				return context;
			}
		}

		context.MarkAsProcessed();
		await PublishEventAsync(context);
		
		return context;
	}

	private ValueTask PublishEventAsync(CommandContext<TCommand> context)
	{
		var eventEnvelope = new EventEnvelope()
		{
			EventId = context.CommandId,
			CorrelationId = context.CommandId,
			CreatedAt = context.CreatedAt,
			EventType = typeof(TCommand).Name,
			Content = context.Command.GetContent() ?? string.Empty
		};

		// Writing the event to channel so that it can be processed by the event handlers
		// Channel reader can pass the events to an external message queue or event bus
		return eventChannel.Writer.WriteAsync(eventEnvelope);
	}
}