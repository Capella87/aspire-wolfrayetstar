using Aliencube.YouTubeSubtitlesExtractor;
using Aliencube.YouTubeSubtitlesExtractor.Abstractions;
using Aliencube.YouTubeSubtitlesExtractor.Models;

using Azure;
using Azure.AI.OpenAI;

using Microsoft.AspNetCore.Mvc;

using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

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

app.MapGet("/", () => "Hello Wolf-Rayet Star!");

app.Run();
