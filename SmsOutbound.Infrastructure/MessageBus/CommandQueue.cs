using System.Runtime.CompilerServices;
using SmsOutbound.Core.Abstractions.External;
using SmsOutbound.Core.Commands;

namespace SmsOutbound.Infrastructure.MessageBus;

public class MockCommandQueue : ICommandQueue
{
	public async IAsyncEnumerable<SendSmsCommand> GetMessagesAsync(
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		int count = 0;
		while (!cancellationToken.IsCancellationRequested)
		{
			count++;

			yield return await Task.FromResult(SendSmsCommand.Create(
				Guid.NewGuid(),
				$"+48{Random.Shared.Next(1000000, 9999999)}",
				$"Hello, this is a test message #{count}!", DateTime.UtcNow));

			if (count % 5 == 0)
			{
				await Task.Delay(Random.Shared.Next(500, 3000));
			}
		};
	}

	public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}