using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SmsOutbound.Core.Abstractions.External;
using SmsOutbound.Core.Models;
using SmsOutbound.Infrastructure.Options;

namespace SmsOutbound.Infrastructure.Clients;

public class MockMessagingClient(
	IOptionsMonitor<MessagingClientOptions> options,
	HttpClient httpClient) : IMessagingClient
{
	public Task<BaseResponse> SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken)
	{
		var content = new SendSmsRequest
		{
			PhoneNumber = phoneNumber,
			Message = message,
		};

		var request = new HttpRequestMessage(HttpMethod.Post, options.CurrentValue.SendSmsEndpoint)
		{
			Content = new StringContent(
				JsonSerializer.Serialize(content),
				Encoding.UTF8,
				"application/json")
		};

		// var response = await httpClient.SendAsync(request);
		// Let's mock real HTTP call as just emulate successful/errorenous response

		if (DateTime.UtcNow.Second % 5 == 0)
		{
			var errorMessage = "Failed to send SMS due to an error from the provider";
			return Task.FromResult(new BaseResponse(System.Net.HttpStatusCode.InternalServerError, errorMessage));
		}
		else
		{
			return Task.FromResult(new BaseResponse());
		}
	}
}