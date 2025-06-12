namespace SmsOutbound.Core.Abstractions;

public interface IPipelineStage<TCommand> where TCommand : ICommand
{
	string StageName { get; }

	ValueTask<CommandContext<TCommand>> ProcessAsync(
		CommandContext<TCommand> context,
		CancellationToken cancellationToken = default);
}
