using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SmsOutbound.Application.Pipeline;
using SmsOutbound.Application.Pipeline.Stages;
using SmsOutbound.Core.Abstractions;
using SmsOutbound.Core.Abstractions.External;
using SmsOutbound.Core.Commands;
using SmsOutbound.Core.Entities;
using SmsOutbound.Core.Models;
using Xunit;

namespace SmsOutbound.UnitTests.Application;

public class CommandProcessingPipelineTests
{
	private readonly IFixture _fixture = new Fixture();

	private readonly IPhoneNumberValidator _phoneNumberValidator;

	private readonly IMessagingClient _messagingClient;

	private readonly IMessageLogRepository _messageLogRepository;

	private readonly CommandProcessingPipeline<SendSmsCommand> _sut;

	public CommandProcessingPipelineTests()
	{
		var loggerMockValidation = Substitute.For<ILogger<SendSmsValidationStage>>();
		var loggerMockProcessing = Substitute.For<ILogger<SendSmsProcessingStage>>();
		var loggerMockPipeline = Substitute.For<ILogger<CommandProcessingPipeline<SendSmsCommand>>>();

		_phoneNumberValidator = Substitute.For<IPhoneNumberValidator>();
		_messagingClient = Substitute.For<IMessagingClient>();
		_messageLogRepository = Substitute.For<IMessageLogRepository>();

		var validationStage = new SendSmsValidationStage(_phoneNumberValidator, loggerMockValidation);
		var processingStage = new SendSmsProcessingStage(_messagingClient, _messageLogRepository, loggerMockProcessing);

		_sut = new CommandProcessingPipeline<SendSmsCommand>([validationStage, processingStage], loggerMockPipeline);
	}

	[Fact]
	public async Task ProcessAsync_ShouldPassThroughStages_WhenCommandIsValid()
	{
		// Arrange
		var command = new SendSmsCommand
		{
			Id = Guid.NewGuid(),
			CreatedAt = DateTime.UtcNow,
			PhoneNumber = _fixture.Create<string>(),
			Message = _fixture.Create<string>(),
		};

		_phoneNumberValidator.IsValid(command.PhoneNumber).Returns(true);
		_messageLogRepository.TryAddMessageLogAsync(Arg.Any<MessageLog>(), out Arg.Any<MessageLog?>(), CancellationToken.None)
			.Returns(true);
		_messagingClient.SendSmsAsync(command.PhoneNumber, command.Message, CancellationToken.None)
			.Returns(new BaseResponse());

		// Act
		var result = await _sut.ProcessAsync(command, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.IsSuccessful.Should().BeTrue();
		result.CurrentStage.Should().Be("SendSmsProcessingStage");
		result.ErrorMessage.Should().BeNullOrEmpty();

		await _messagingClient.Received(1).SendSmsAsync(command.PhoneNumber, command.Message, CancellationToken.None);
	}

	[Fact]
	public async Task ProcessAsync_ShouldFailAfterValidationStage()
	{
		// Arrange
		var command = new SendSmsCommand
		{
			Id = Guid.NewGuid(),
			CreatedAt = DateTime.UtcNow,
			PhoneNumber = _fixture.Create<string>(),
			Message = _fixture.Create<string>(),
		};

		_phoneNumberValidator.IsValid(command.PhoneNumber).Returns(false);

		// Act
		var result = await _sut.ProcessAsync(command, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.IsSuccessful.Should().BeFalse();
		result.CurrentStage.Should().Be("SendSmsValidationStage");
		result.ErrorMessage.Should().Be($"Invalid phone number: {command.PhoneNumber}");
	}
	
	[Fact]
	public async Task ProcessAsync_ShouldFailAfterProcessingStage()
	{
		// Arrange
		var command = new SendSmsCommand
		{
			Id = Guid.NewGuid(),
			CreatedAt = DateTime.UtcNow,
			PhoneNumber = _fixture.Create<string>(),
			Message = _fixture.Create<string>(),
		};

		var existingMessageLog = new MessageLog
		{
			PhoneNumber = command.PhoneNumber,
			Message = command.Message,
		};
		existingMessageLog.MarkAsSent();

		_phoneNumberValidator.IsValid(command.PhoneNumber).Returns(true);
		_messageLogRepository.TryAddMessageLogAsync(Arg.Any<MessageLog>(), out Arg.Any<MessageLog?>(), CancellationToken.None)
			.Returns(x =>
			{
				x[1] = existingMessageLog;
				return false;
			});

		// Act
		var result = await _sut.ProcessAsync(command, CancellationToken.None);

		// Assert
		result.Should().NotBeNull();
		result.IsSuccessful.Should().BeFalse(); 
		result.CurrentStage.Should().Be("SendSmsProcessingStage");
		result.ErrorMessage.Should().Be($"Message with ID {command.Id} already exists. Is already send: True.");
	}
}