using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TournamentApp.Web.Extensions;
using TournamentApp.Web.Responses;

namespace TournamentApp.WebTests.Extensions;

public class WhenTestingHttpResponseMessageExtensions
{
    [Fact]
    public async Task GetResponseData_ShouldReturnEmptyInstance_WhenResponseIsNull()
    {
        // Arrange
        HttpResponseMessage? response = null;

        // Act
        var result = await response.GetResponseData<Response>();

        // Assert
        result.Should().NotBeNull();
        result.ValidationErrors.Should().BeEmpty();
        result.ErrorMessage.Should().BeEmpty();
    }

    [Fact]
    public async Task GetResponseData_ShouldDeserializeResponse_WhenResponseIsValid()
    {
        // Arrange
        var expectedResponse = new DataResponse<string>
        {
            Data = "test data",
            ErrorMessage = string.Empty,
            ValidationErrors = new List<FluentValidation.Results.ValidationFailure>()
        };

        var json = System.Text.Json.JsonSerializer.Serialize(expectedResponse);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = content
        };

        // Act
        var result = await response.GetResponseData<DataResponse<string>>();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().Be("test data");
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task GetResponseData_ShouldDeserializeErrorResponse_WhenResponseHasValidationErrors()
    {
        // Arrange
        var validationErrors = new List<FluentValidation.Results.ValidationFailure>
        {
            new FluentValidation.Results.ValidationFailure("Name", "Name is required")
        };

        var errorResponse = new Response
        {
            ErrorMessage = string.Empty,
            ValidationErrors = validationErrors
        };

        var json = System.Text.Json.JsonSerializer.Serialize(errorResponse);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = content
        };

        // Act
        var result = await response.GetResponseData<Response>();

        // Assert
        result.Should().NotBeNull();
        result.IsFailure.Should().BeTrue();
        result.ValidationErrors.Should().HaveCount(1);
        result.ValidationErrors.First().PropertyName.Should().Be("Name");
        result.ValidationErrors.First().ErrorMessage.Should().Be("Name is required");
    }

    [Fact]
    public async Task GetResponseData_ShouldReturnEmptyInstance_WhenDeserializationFails()
    {
        // Arrange
        var invalidJson = "{ invalid json }";
        var content = new StringContent(invalidJson, System.Text.Encoding.UTF8, "application/json");
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = content
        };

        // Act
        var result = await response.GetResponseData<Response>();

        // Assert
        result.Should().NotBeNull();
        result.ValidationErrors.Should().BeEmpty();
        result.ErrorMessage.Should().BeEmpty();
    }
}




