using System.Runtime.CompilerServices;

namespace WolfRayetStar.Front.Services;

public interface IApiAppClientService
{
    Task<List<ApiAppClient.WeatherForecast>> WeatherForecastAsync();
    Task<string> SummariseAsync(string url, string videoLanguageCode, string resultLanguageCode);
}

public class ApiAppClient : IApiAppClientService
{
    private readonly HttpClient _httpClient;

    public ApiAppClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<List<WeatherForecast>> WeatherForecastAsync()
    {
        using var response = await _httpClient.GetAsync("weatherforecast").ConfigureAwait(false);
        var forecasts  = await response.Content.ReadFromJsonAsync<List<WeatherForecast>>().ConfigureAwait(false);

        return forecasts ?? [];
    }

    public async Task<string> SummariseAsync(string url, string videoLanguageCode, string resultLanguageCode)
    {
        using var response = await _httpClient.PostAsJsonAsync("summarise", new { url, videoLanguageCode, resultLanguageCode }).ConfigureAwait(false);
        var summary = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        return summary;
    }

    public class WeatherForecast
    {
        public DateOnly Date { get; set; }
        public int TemperatureC { get; set; }
        public string? Summary { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
