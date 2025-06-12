namespace SmsOutbound.Core.Abstractions;

public sealed class CommandContext<TCommand> where TCommand : ICommand
{
	public Guid CommandId => Command.Id;

	public TCommand Command { get; private set; }

	public DateTime CreatedAt => Command.CreatedAt;

	public DateTime ProcessedAt { get; private set; }

	public string? CurrentStage { get;  private set; }

	public string? ErrorMessage { get; private set; }

	public bool IsSuccessful => string.IsNullOrEmpty(ErrorMessage);

	public static CommandContext<TCommand> Create(TCommand command)
	{
		ArgumentNullException.ThrowIfNull(command, nameof(command));

		return new CommandContext<TCommand>
		{
			Command = command,
		};
	}

	public void EnterStage(string stageName)
	{
		CurrentStage = stageName;
	}

	public void MarkAsProcessed()
	{
		ProcessedAt = DateTime.UtcNow;
	}

	public void MarkAsFailed(string error)
	{
		ErrorMessage = error;
	}
}