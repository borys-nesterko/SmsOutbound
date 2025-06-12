using SmsOutbound.Core.Commands;

namespace SmsOutbound.Core.Abstractions.External;

public interface ICommandQueue : IAsyncDisposable
{
	IAsyncEnumerable<SendSmsCommand> GetMessagesAsync(
		CancellationToken cancellationToken = default);
}