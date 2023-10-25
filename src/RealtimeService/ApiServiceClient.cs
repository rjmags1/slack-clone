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
    }
