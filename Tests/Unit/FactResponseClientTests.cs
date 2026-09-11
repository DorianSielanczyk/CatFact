using System.Net;
using System.Text.Json;
using Moq;
using Moq.Protected;
using Xunit;
using CatFact.API.Clients;
using CatFact.API.DTOs;

namespace CatFact.Tests.Unit;

public class FactResponseClientTests
{
    private readonly Mock<HttpMessageHandler> _mockHandler = new();
    private readonly FactResponseClient _client;

    public FactResponseClientTests()
    {
        var httpClient = new HttpClient(_mockHandler.Object)
        {
            BaseAddress = new Uri("https://catfact.ninja/")
        };
        _client = new FactResponseClient(httpClient);
    }

    [Fact]
    public async Task GetFactResponseAsync_ReturnsDeserializedFact_OnSuccess()
    {
        // Arrange
        var jsonResponse = JsonSerializer.Serialize(new FactResponse { Fact = "Cats purr", Length = 9 });

        SetupMockHandler(HttpStatusCode.OK, jsonResponse);

        // Act
        var result = await _client.GetFactResponseAsync();

        // Assert
        Assert.Equal("Cats purr", result.Fact);
        Assert.Equal(9, result.Length);
    }

    [Fact]
    public async Task GetFactResponseAsync_ThrowsHttpRequestException_OnApiError()
    {
        // Arrange
        SetupMockHandler(HttpStatusCode.InternalServerError, "");

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _client.GetFactResponseAsync());
    }

    [Fact]
    public async Task GetFactResponseAsync_ReturnsEmptyObject_WhenApiResponseIsNull()
    {
        // Arrange
        SetupMockHandler(HttpStatusCode.OK, "null");

        // Act
        var result = await _client.GetFactResponseAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Fact);
        Assert.Equal(0, result.Length);
    }

    [Fact]
    public async Task GetFactResponseAsync_ThrowsJsonException_WhenApiReturnsInvalidJson()
    {
        // Arrange
        SetupMockHandler(HttpStatusCode.OK, "<!DOCTYPE html><html>Error</html>");

        // Act & Assert
        await Assert.ThrowsAsync<JsonException>(() => _client.GetFactResponseAsync());
    }

    private void SetupMockHandler(HttpStatusCode statusCode, string content)
    {
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode,
                Content = new StringContent(content)
            });
    }
}
