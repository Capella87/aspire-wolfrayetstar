using Aliencube.YouTubeSubtitlesExtractor.Abstractions;
using Aliencube.YouTubeSubtitlesExtractor.Models;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;

namespace WolfRayetStar.Backend.Services;

public record SummaryRequest(string? Url, string VideoLanguageCode, string? SummaryLanguageCode);

public class YouTubeSummariserService
{
    private readonly IYouTubeVideo _ytVideo;
    private readonly AzureOpenAIClient _openaiClient;
    private readonly IConfiguration _config;

    public YouTubeSummariserService(IYouTubeVideo ytVideo, AzureOpenAIClient openaiClient, IConfiguration config)
    {
        _ytVideo = ytVideo ?? throw new ArgumentNullException(nameof(ytVideo));
        _openaiClient = openaiClient ?? throw new ArgumentNullException(nameof(openaiClient));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public async Task<string> SummariseAsync(SummaryRequest rqst)
    {
        Subtitle subtitle = await this._ytVideo.ExtractSubtitleAsync(rqst.Url, rqst.VideoLanguageCode).ConfigureAwait(false);
        string caption = subtitle.Content
            .Select(p => p.Text)
            .Aggregate((a, b) => $"{a}\n{b}");
        var chat = this._openaiClient.GetChatClient(this._config["OpenAI:DeploymentName"]);
        var messages = new List<ChatMessage>()
        {
            new SystemChatMessage(this._config["Prompt:System"]),
            new SystemChatMessage($"Here's the transcript. Summarise it in 5 bullet point items in the given language code of \"{rqst.SummaryLanguageCode}\"."),
            new UserChatMessage(caption),
        };
        var options = new ChatCompletionOptions()
        {
            MaxTokens = int.TryParse(this._config["Prompt:MaxTokens"], out var maxTokens) ? maxTokens : 3000,
            Temperature = float.TryParse(this._config["Prompt:Temperature"], out var temperature) ? temperature : .7f
        };

        var response = await chat.CompleteChatAsync(messages, options).ConfigureAwait(false);
        var summary = response.Value.Content[0].Text;
        return summary;
    }
}
