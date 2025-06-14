namespace SmsOutbound.Infrastructure.Options;

public sealed class MessagingClientOptions
{
	public static string SectionName => "MessagingClient";
	
	public required Uri BaseUrl { get; init; }

	public required string ApiKey { get; init; }

	public required string SendSmsEndpoint { get; init; }

	public int TimeoutSeconds { get; init; } = 30;
}