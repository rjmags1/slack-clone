using IdentityModel.Client;
using System.Text.Json;
using System.Text;

namespace RealtimeService.ApiServiceClient;

public class ApiClient
{
    public DateTime? LastRefresh { get; set; }
    public string? Token { get; set; }
    public double? ExpiresIn { get; set; }
    private static readonly double FIVE_MINUTES = 5 * 60;
    private HttpClient HttpClient { get; set; }

    public ApiClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
        FetchToken();
    }

    public async Task WorkspaceSignIn(Guid UserId, Guid WorkspaceId)
    {
        if (!await CheckToken())
        {
            throw new InvalidOperationException(
                "Could not obtain access token"
            );
        }
        var realtimeEvent = new RealtimeEventDetails
        {
            Type = RealtimeEvent.WorkspaceSignin,
            UserId = UserId.ToString(),
            WorkspaceId = WorkspaceId.ToString(),
        };
        var json = new StringContent(
            JsonSerializer.Serialize(realtimeEvent),
            Encoding.UTF8,
            "application/json"
        );
        HttpClient.SetBearerToken(Token);
        var response = await HttpClient.PostAsync(
            "https://localhost:6001/realtime",
            json
        );
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                "Workspace signin api call failed"
            );
        }
    }

    private async Task<bool> CheckToken()
    {
        if (Token is null || ExpiresIn is null || LastRefresh is null)
            throw new InvalidOperationException(
                "Access token was not initialized from constructor"
            );
        TimeSpan timeUsed = (TimeSpan)(DateTime.UtcNow - LastRefresh);
        int retries = 0;
        while (
            (timeUsed.TotalMinutes > ExpiresIn - FIVE_MINUTES) && retries < 3
        )
        {
            try
            {
                await FetchToken();
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await Task.Run(() => Thread.Sleep(300));
            }
            retries += 1;
        }

        return retries < 3;
    }

    private async Task FetchToken()
    {
        var disco = await HttpClient.GetDiscoveryDocumentAsync(
            "https://localhost:5001"
        );
        if (disco.IsError)
        {
            throw new InvalidOperationException(
                "discovery document retrieval failed"
            );
        }

        var tokenResponse = await HttpClient.RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "realtime service",
                ClientSecret = "secret",
                Scope = "realtime"
            }
        );

        LastRefresh = DateTime.UtcNow;
        Token = tokenResponse.AccessToken;
        ExpiresIn = tokenResponse.ExpiresIn;
    }

    public struct RealtimeEventDetails
    {
        public RealtimeEvent Type { get; set; }
        public string UserId { get; set; }
        public string WorkspaceId { get; set; }
        public string? GroupId { get; set; }
        public string? Content { get; set; }
        public List<string>? Headers { get; set; }
    }

    public enum RealtimeEvent
    {
        WorkspaceSignin,
        WorkspaceSignout,
    }
}
