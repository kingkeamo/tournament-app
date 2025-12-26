using System.Net.Http.Json;

namespace TournamentApp.Web.Extensions;

public static class HttpResponseMessageExtensions
{
    public static async Task<T> GetResponseData<T>(this HttpResponseMessage? response) where T : new()
    {
        if (response == null)
        {
            return new T();
        }

        try
        {
            // Deserialize response body regardless of HTTP status code
            // API returns ValidatedResponse in body for both 200 OK and 400 BadRequest
            var data = await response.Content.ReadFromJsonAsync<T>();
            return data ?? new T();
        }
        catch
        {
            // If deserialization fails, return empty instance
            // This ensures the response object is never null
            return new T();
        }
    }
}

