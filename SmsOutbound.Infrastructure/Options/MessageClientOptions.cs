namespace SmsOutbound.Infrastructure.Options;

public sealed class MessagingClientOptions
{
	public static string SectionName => "MessagingClient";
	
	public Uri BaseUrl { get; init; }

	public string ApiKey { get; init; }

	public string SendSmsEndpoint { get; init; }

	public int TimeoutSeconds { get; init; } = 30;
}