using CatFact.API.Clients;
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
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;
       public async Task<FactResponse> SaveToFileFactResponseAsync(CancellationToken cancellationToken = default)
        {
            var factResponse = await factResponseClient.GetFactResponseAsync(cancellationToken);

            var filePath = EnsureDirectoryAndGetFilePath();
            var contentToAppend = $"{factResponse.Fact}, {factResponse.Length}{Environment.NewLine}";

            await FileWriteLock.WaitAsync(cancellationToken);
            try
            {
                var fileInfo = new FileInfo(filePath);
                long currentSize = fileInfo.Exists ? fileInfo.Length : 0;
                
                long newContentSize = System.Text.Encoding.UTF8.GetByteCount(contentToAppend);

                if (currentSize + newContentSize > MaxFileSizeBytes)
                {
                    throw new InvalidOperationException("File size limit of 5 MB exceeded.");
                }

                await File.AppendAllTextAsync(filePath, contentToAppend, cancellationToken);
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
