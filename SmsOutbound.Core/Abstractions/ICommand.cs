namespace SmsOutbound.Core.Abstractions;

public interface ICommand
{
	public Guid Id { get; init; }

	public DateTime CreatedAt { get; init; }

	public string? GetContent();
}