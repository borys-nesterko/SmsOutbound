namespace SmsOutbound.Core.Abstractions;

public interface ICommandProcessingPipeline<TCommand>
	where TCommand : ICommand
{
	Task<CommandContext<TCommand>> ProcessAsync(TCommand command, CancellationToken cancellationToken = default);
}