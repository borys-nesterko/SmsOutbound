using SmsOutbound.Core.Models;

namespace SmsOutbound.Core.Abstractions.External;

public interface IMessagingClient
{
	/// <summary>
	/// Sends an SMS message to the specified phone number.
	/// </summary>
	/// <param name="phoneNumber">The phone number to send the SMS to.</param>
	/// <param name="message">The content of the SMS message.</param>
	/// <param name="cancellationToken"></param>
	/// <returns>Response with status code and error, if such</returns>
	Task<BaseResponse> SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}