using SmsOutbound.Host.BackgroundWorkers;
using SmsOutbound.Host.Extensions;
using SmsOutbound.Infrastructure.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddLogging(config =>
{
	config.AddDebug();
	config.AddConsole();
});

var MessageClientSection = builder.Configuration.GetSection(MessagingClientOptions.SectionName);
builder.Services.Configure<MessagingClientOptions>(MessageClientSection);

builder.Services.AddMessagingServices();
builder.Services.AddHostedService<MessagesConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	app.UseHsts();
	app.MapOpenApi();
}

app.MapControllers();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseHttpsRedirection();

await app.RunAsync();
