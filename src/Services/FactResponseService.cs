using CatFact.API.Client;
using CatFact.API.DTOs;
using CatFact.API.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CatFact.API.Services
{
    public class FactResponseService(IFactResponseClient factResponseClient,
        IOptions<FileStorageOptions> fileStorageOptions) : IFactResponseService
    {
        public async Task<FactResponse> SaveToFileFactResponseAsync(CancellationToken cancellationToken = default)
        {
            var factResponse = await factResponseClient.GetFactResponseAsync(cancellationToken);

            var filePath = EnsureDirectoryAndGetFilePath();

            var content = $"{factResponse.Fact}, {factResponse.Length}";

            await File.AppendAllTextAsync(filePath, $"{content}{Environment.NewLine}", cancellationToken);

            return factResponse;
        }

        private string EnsureDirectoryAndGetFilePath()
        {
            var parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName;
            var directoryPath = Path.Combine(parentDirectory!, fileStorageOptions.Value.DirectoryName);

            Directory.CreateDirectory(directoryPath);

            return Path.Combine(directoryPath, fileStorageOptions.Value.FileName);
        }
    }
}
