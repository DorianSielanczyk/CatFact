using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using CatFact.API.Services;
using CatFact.API.Client;
using CatFact.API.DTOs;
using CatFact.API.Models;

namespace CatFact.Tests.Integration;

public class FactResponseServiceTests : IDisposable
{
    private readonly string _testDirectoryName = "TestResponses_" + Guid.NewGuid();
    private readonly string _testFileName = "test_fact.txt";
    private readonly Mock<IFactResponseClient> _mockClient = new();
    private readonly Mock<IWebHostEnvironment> _mockEnvironment = new();
    private readonly FactResponseService _service;
    private readonly string _expectedDirPath;

    public FactResponseServiceTests()
    {
        var options = Options.Create(new FileStorageOptions
        {
            DirectoryName = _testDirectoryName,
            FileName = _testFileName
        });

        _mockEnvironment.Setup(e => e.ContentRootPath)
            .Returns(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName);

        _service = new FactResponseService(_mockClient.Object, options, _mockEnvironment.Object);
        _expectedDirPath = Path.Combine(_mockEnvironment.Object.ContentRootPath, _testDirectoryName);
    }

    [Fact]
    public async Task SaveToFileFactResponseAsync_CreatesFileAndAppendsContent_OnSuccess()
    {
        // Arrange
        _mockClient.Setup(c => c.GetFactResponseAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new FactResponse { Fact = "Cats have whiskers", Length = 18 });

        // Act
        await _service.SaveToFileFactResponseAsync();

        // Assert
        var filePath = Path.Combine(_expectedDirPath, _testFileName);
        Assert.True(File.Exists(filePath));

        var content = await File.ReadAllTextAsync(filePath);
        Assert.Contains("Cats have whiskers, 18", content);
    }

    [Fact]
    public async Task SaveToFileFactResponseAsync_DoesNotCreateFile_WhenClientThrowsException()
    {
        // Arrange
        _mockClient.Setup(c => c.GetFactResponseAsync(It.IsAny<CancellationToken>()))
                   .ThrowsAsync(new HttpRequestException("API is down"));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _service.SaveToFileFactResponseAsync());

        var filePath = Path.Combine(_expectedDirPath, _testFileName);
        Assert.False(File.Exists(filePath));
    }

    [Fact]
    public async Task SaveToFileFactResponseAsync_ThrowsException_WhenConfigurationIsInvalid()
    {
        // Arrange
        _mockClient.Setup(c => c.GetFactResponseAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new FactResponse { Fact = "Cats", Length = 4 });

        var badOptions = Options.Create(new FileStorageOptions
        {
            DirectoryName = "Bad|?<>Directory",
            FileName = "test.txt"
        });

        var serviceWithBadConfig = new FactResponseService(_mockClient.Object, badOptions, _mockEnvironment.Object);

        // Act & Assert
        await Assert.ThrowsAnyAsync<Exception>(() => serviceWithBadConfig.SaveToFileFactResponseAsync());
    }

    [Fact]
    public async Task SaveToFileFactResponseAsync_DoesNotWriteToFile_IfCanceledBeforeWrite()
    {
        // Arrange
        var cts = new CancellationTokenSource();

        _mockClient.Setup(c => c.GetFactResponseAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new FactResponse { Fact = "Cats", Length = 4 })
                   .Callback(() => cts.Cancel());

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            _service.SaveToFileFactResponseAsync(cts.Token));

        var filePath = Path.Combine(_expectedDirPath, _testFileName);
        Assert.False(File.Exists(filePath));
    }

    public void Dispose()
    {
        if (Directory.Exists(_expectedDirPath))
        {
            Directory.Delete(_expectedDirPath, recursive: true);
        }
    }
}
