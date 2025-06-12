using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SmsOutbound.Application.Pipeline;
using SmsOutbound.Application.Pipeline.Stages;
using SmsOutbound.Application.Validators;
using SmsOutbound.Core.Abstractions;
using SmsOutbound.Core.Abstractions.External;
using SmsOutbound.Core.Commands;
using SmsOutbound.Infrastructure.Clients;
using SmsOutbound.Infrastructure.MessageBus;
using SmsOutbound.Infrastructure.Options;
using SmsOutbound.Infrastructure.Repositories;

namespace SmsOutbound.Host.Extensions;

public static class ServiceCollectionExtensions
{
	private static readonly JsonSerializerOptions JsonSerializerOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		WriteIndented = true
	};

	public static IServiceCollection AddMessagingServices(this IServiceCollection services)
	{
		// Register infrastracture
		var httpClientBuild = services.AddHttpClient<IMessagingClient, MockMessagingClient>()
			.ConfigureHttpClient((serviceProvider, httpClient) =>
			{
				var options = serviceProvider.GetRequiredService<IOptionsMonitor<MessagingClientOptions>>().Get(MessagingClientOptions.SectionName);

				httpClient.BaseAddress = options.BaseUrl;
				httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
				httpClient.DefaultRequestHeaders.Add("ApiKey", options.ApiKey);
			})
			.AddStandardResilienceHandler(); // Default policy can be overridden in the appsettings.json
			
		services.AddSingleton<ICommandQueue, MockCommandQueue>();

		// Register the pipeline and stages
		services.AddSingleton<SendSmsValidationStage>();
		services.AddSingleton<SendSmsProcessingStage>();
		services.AddSingleton<ICommandProcessingPipeline<SendSmsCommand>>(
			serviceProvider =>
			{
				var validationStage = serviceProvider.GetRequiredService<SendSmsValidationStage>();
				// Additional stages can be added in between
				// i.e. templating stage that would replace placeholders in the message body and localize content for the user
				var processingStage = serviceProvider.GetRequiredService<SendSmsProcessingStage>();

				var stages = new IPipelineStage<SendSmsCommand>[]
				{
					validationStage,
					processingStage,
				};

				var pipelineLogger = serviceProvider.GetRequiredService<ILogger<CommandProcessingPipeline<SendSmsCommand>>>();

				return new CommandProcessingPipeline<SendSmsCommand>(stages, pipelineLogger);
			});

		// Register validators
		services.AddSingleton<IPhoneNumberValidator, PhoneNumberValidator>();

		// Register repositories
		// In real world applications, this would be a key-value store - Redis, DynamoDB, Mongo etc. 
		// or a SQL database, like PostgreSQL if we need to implement kind of poisoned queue with an outbox pattern
		services.AddScoped<IMessageLogRepository, MessageLogInMemoryRepository>(); 

		return services;
	}
}