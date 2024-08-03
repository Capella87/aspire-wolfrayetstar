using Aliencube.YouTubeSubtitlesExtractor;
using Aliencube.YouTubeSubtitlesExtractor.Abstractions;
using Aliencube.YouTubeSubtitlesExtractor.Models;

using Azure;
using Azure.AI.OpenAI;

using Microsoft.AspNetCore.Mvc;
using WolfRayetStar.Backend.Services;

using OpenAI.Chat;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddHttpClient<IYouTubeVideo, YouTubeVideo>();
builder.Services.AddScoped<AzureOpenAIClient>(s =>
{
    var config = s.GetRequiredService<IConfiguration>();
    var endpoint = new Uri(config["OpenAI:Endpoint"]);
    var credentialKey = new AzureKeyCredential(config["OpenAI:Key"]);
    var azClient = new AzureOpenAIClient(endpoint, credentialKey);

    return azClient;
});

builder.Services.AddScoped<YouTubeSummariserService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

var weatherSummaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecastEntries = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-30, 55),
            weatherSummaries[Random.Shared.Next(weatherSummaries.Length)]
        ))
    .ToArray();
    return forecastEntries;
})
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapPost("/summarise", async ([FromBody] SummaryRequest req, YouTubeSummariserService service) =>
{
    var summary = await service.SummariseAsync(req);
    return summary;
})
    .WithName("GetSummary")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / .5556);
}
