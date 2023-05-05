using System.Net.Http.Json;

namespace SlackNotifier;

public interface ISlackClient
{
    Task SendMessage(string message, string url);
}

public class SlackClient : ISlackClient
{
    private static readonly HttpClient httpClient = new ();

    public async Task SendMessage(string message, string url)
    {
        var payload = new
        {
            text = message
        };

        var response = await httpClient.PostAsJsonAsync(url, payload);

        response.EnsureSuccessStatusCode();
    }
}