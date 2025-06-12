using SmsOutbound.Core.Abstractions;
using SmsOutbound.Core.Abstractions.External;
using SmsOutbound.Core.Commands;

namespace SmsOutbound.Host.BackgroundWorkers;

public class MessagesConsumer(
	ICommandQueue commandQueue,
	ICommandProcessingPipeline<SendSmsCommand> commandProcessingPipeline,
	ILogger<MessagesConsumer> logger) : BackgroundService, IAsyncDisposable
{
	private bool _disposed;

	protected override Task ExecuteAsync(CancellationToken cancellationToken)
	{
		logger.LogInformation("MessagesConsumer starting.");

		return Task.Run(async () =>
		{
			int count = 0;
			await foreach (var command in commandQueue.GetMessagesAsync(cancellationToken))
			{
				try
				{
					count++;
					var context = await commandProcessingPipeline.ProcessAsync(command);

					if (context.IsSuccessful)
					{
						logger.LogInformation("Command {CommandId} processed successfully at {ProcessedAt}.",
							context.CommandId, context.ProcessedAt);
					}
					else
					{
						// Failed message stays in the storage and can be picked up later for retry
						logger.LogError("Error processing command: {ErrorMessage}", context.ErrorMessage);
					}
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "An error occurred while processing messages.");
					await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken); // Delay on error to avoid tight loop
				}
			}
		}, cancellationToken);
	}
	
	public async ValueTask DisposeAsync()
	{
		await DisposeAsync(true);
		GC.SuppressFinalize(this); // Prevent the finalizer from running
	}

	protected virtual async ValueTask DisposeAsync(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				await commandQueue.DisposeAsync();
			}

			_disposed = true;
		}
	}
}