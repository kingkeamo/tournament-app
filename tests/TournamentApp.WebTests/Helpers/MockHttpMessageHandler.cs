using System.Net;
using System.Text;

namespace TournamentApp.WebTests.Helpers;

// Shared helper class for mocking HTTP responses in service tests
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly object _response;
    private readonly HttpStatusCode _statusCode;

    public MockHttpMessageHandler(object response, HttpStatusCode statusCode)
    {
        _response = response;
        _statusCode = statusCode;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(_response);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        return await Task.FromResult(new HttpResponseMessage(_statusCode)
        {
            Content = content
        });
    }
}

