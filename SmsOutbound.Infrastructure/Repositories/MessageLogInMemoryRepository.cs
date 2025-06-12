using SmsOutbound.Core.Abstractions;
using SmsOutbound.Core.Entities;   
using System.Collections.Concurrent; 

namespace SmsOutbound.Infrastructure.Repositories;

public class MessageLogInMemoryRepository : IMessageLogRepository
{
	private readonly ConcurrentDictionary<Guid, MessageLog> _messageLogs = new();

	/// <summary>
	/// Retrieves a message log by its MessageId.
	/// If the message log does not exist, returns null.
	/// </summary>
	public Task<MessageLog?> GetMessageLogAsync(Guid messageId, CancellationToken cancellationToken = default)
	{
		return Task.FromResult(
			_messageLogs.TryGetValue(messageId, out var messageLog) ?
			messageLog : null);
	}

	/// <summary>
	/// Retrieves a paginated list of message logs.
	/// The page parameter is 1-based, meaning page 1 returns the first set of logs.
	/// </summary>
	public Task<MessageLog[]> GetMessagesAsync(int page, int pageSize, CancellationToken cancellationToken = default)
	{
		var messages = _messageLogs.Values
			.Skip((page - 1) * pageSize)
			.Take(pageSize).ToArray();

		return Task.FromResult(messages);
	}

	/// <summary>
	/// Attempts to add a new message log.    
	/// Returns true if the message log was added successfully, false if a log with the same MessageId already exists.
	/// </summary>  
	public Task<bool> TryAddMessageLogAsync(MessageLog newMessageLog, out MessageLog? existingMessageLog, CancellationToken cancellationToken = default)
	{
		var messageAdded = _messageLogs.TryAdd(newMessageLog.MessageId, newMessageLog);
		existingMessageLog = messageAdded ? null : _messageLogs[newMessageLog.MessageId];
		
		return Task.FromResult(messageAdded);
	}

	/// <summary>
	/// Updates an existing message log.
	/// </summary>  
	public Task UpdateMessageLogAsync(MessageLog messageLog, CancellationToken cancellationToken = default)
	{
		if (_messageLogs.ContainsKey(messageLog.MessageId))
		{
			_messageLogs[messageLog.MessageId] = messageLog;

			return Task.CompletedTask;
		}
		else
		{
			throw new KeyNotFoundException($"MessageLog with ID {messageLog.MessageId} not found.");
		}
	}
}