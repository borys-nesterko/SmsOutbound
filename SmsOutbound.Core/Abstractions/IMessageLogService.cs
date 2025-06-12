using SmsOutbound.Core.Entities;

namespace SmsOutbound.Core.Abstractions;

public interface IMessageLogRepository
{
	Task<MessageLog?> GetMessageLogAsync(Guid messageId, CancellationToken cancellationToken = default);

	Task<MessageLog[]> GetMessagesAsync(int page, int pageSize, CancellationToken cancellationToken = default);

	Task<bool> TryAddMessageLogAsync(MessageLog newMessageLog, out MessageLog? existingMessageLog, CancellationToken cancellationToken = default);

	Task UpdateMessageLogAsync(MessageLog messageLog, CancellationToken cancellationToken = default);
}