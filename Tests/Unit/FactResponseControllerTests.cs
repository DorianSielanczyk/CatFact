using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using CatFact.API.Controllers;
using CatFact.API.Services;
using CatFact.API.DTOs;

namespace Tests.Unit;

public class FactResponseControllerTests
{
    private readonly Mock<IFactResponseService> _mockService = new();
    private readonly FactResponseController _controller;

    public FactResponseControllerTests()
    {
        _controller = new FactResponseController(_mockService.Object);
    }

    [Fact]
    public async Task GetFactResponse_ReturnsOk_WhenServiceSucceeds()
    {
        // Arrange
        var expectedResponse = new FactResponse { Fact = "Cats can see in the dark", Length = 24 };
        _mockService.Setup(s => s.SaveToFileFactResponseAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetFactResponse(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualResponse = Assert.IsType<FactResponse>(okResult.Value);
        Assert.Equal(expectedResponse.Fact, actualResponse.Fact);
    }

    [Fact]
    public async Task GetFactResponse_ThrowsOperationCanceledException_WhenRequestCanceled()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockService.Setup(s => s.SaveToFileFactResponseAsync(cts.Token))
                    .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _controller.GetFactResponse(cts.Token));
    }
}
