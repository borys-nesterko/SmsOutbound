using System.Net;

namespace SmsOutbound.Core.Models
{
	/// <summary>
	/// Represents a base response for operations in the SMS outbound system.
	/// May be extended to generic type with specific content.
	/// </summary>
	public record BaseResponse(HttpStatusCode StatusCode = HttpStatusCode.OK, string? ErrorMessage = null)
	{
		public bool IsSuccess => StatusCode == HttpStatusCode.OK && string.IsNullOrEmpty(ErrorMessage);
	}
}