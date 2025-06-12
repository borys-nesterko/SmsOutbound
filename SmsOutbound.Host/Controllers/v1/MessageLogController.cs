using Microsoft.AspNetCore.Mvc;
using SmsOutbound.Core.Abstractions;

namespace SmsOutbound.Host.Controllers.v1;

[Route("api/v1/messages")]
public class MessageLogController(IMessageLogRepository messageLogService) : ControllerBase
{
	[HttpGet("{messageId:guid}")]
	public async Task<IActionResult> GetMessageLog(
		Guid messageId,
		CancellationToken cancellationToken = default)
	{
		var messageLog = await messageLogService.GetMessageLogAsync(messageId, cancellationToken);

		if (messageLog is null)
		{
			return NotFound();
		}

		//Here messageLog can be mapped to a response model
		return Ok(messageLog);
	}

	[HttpGet]
	public async Task<IActionResult> GetMessages(
		int page = 1,
		int pageSize = 100,
		CancellationToken cancellationToken = default)
	{
		var messages = await messageLogService.GetMessagesAsync(page, pageSize, cancellationToken);

		//Here messages can be mapped to a response model
		return Ok(messages);
	}
}