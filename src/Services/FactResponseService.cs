using CatFact.API.Client;
using CatFact.API.DTOs;
using CatFact.API.Models;
using Microsoft.Extensions.Options;

namespace CatFact.API.Services
{
    public class FactResponseService(IFactResponseClient factResponseClient,
        IOptions<FileStorageOptions> fileStorageOptions,
        IWebHostEnvironment environment) : IFactResponseService
    {
        private static readonly SemaphoreSlim FileWriteLock = new(1, 1);
        public async Task<FactResponse> SaveToFileFactResponseAsync(CancellationToken cancellationToken = default)
        {
            var factResponse = await factResponseClient.GetFactResponseAsync(cancellationToken);

            var filePath = EnsureDirectoryAndGetFilePath();

            var content = $"{factResponse.Fact}, {factResponse.Length}";

            await FileWriteLock.WaitAsync(cancellationToken);
            try
            {
                await File.AppendAllTextAsync(filePath, $"{content}{Environment.NewLine}", cancellationToken);
            }
            finally
            {
                FileWriteLock.Release();
            }


            return factResponse;
        }

        private string EnsureDirectoryAndGetFilePath()
        {
            var options = fileStorageOptions.Value;

            if (string.IsNullOrWhiteSpace(options.DirectoryName))
            {
                throw new InvalidOperationException("Brakuje nazwy katalogu.");
            }

            if (string.IsNullOrWhiteSpace(options.FileName))
            {
                throw new InvalidOperationException("Brakuje nazwy pliku.");
            }

            var directoryPath = Path.GetFullPath(Path.Combine(environment.ContentRootPath, options.DirectoryName));

            Directory.CreateDirectory(directoryPath);

            return Path.Combine(directoryPath, options.FileName);
        }
    }
}
