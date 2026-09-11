using CatFact.API.Clients;
using CatFact.API.DTOs;
using CatFact.API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace CatFact.API.Services
{
    public class FactResponseService(
        IFactResponseClient factResponseClient,
        IOptions<FileStorageOptions> fileStorageOptions,
        IWebHostEnvironment environment) : IFactResponseService
    {
        private static readonly SemaphoreSlim FileWriteLock = new(1, 1);
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public async Task<FactResponse> SaveToFileFactResponseAsync(CancellationToken cancellationToken = default)
        {
            var factResponse = await factResponseClient.GetFactResponseAsync(cancellationToken);

            var contentToAppend = FormatContent(factResponse);

            await WriteContentSafelyAsync(contentToAppend, cancellationToken);

            return factResponse;
        }

        private static string FormatContent(FactResponse response) =>
            $"{response.Fact}, {response.Length}{Environment.NewLine}";

        private async Task WriteContentSafelyAsync(string content, CancellationToken cancellationToken)
        {
            var options = fileStorageOptions.Value;
            ValidateOptions(options);

            var directoryPath = EnsureAndGetDirectoryPath(options.DirectoryName);
            long newContentSize = System.Text.Encoding.UTF8.GetByteCount(content);

            await FileWriteLock.WaitAsync(cancellationToken);
            try
            {
                var targetFilePath = GetAvailableFilePath(directoryPath, options.FileName, newContentSize);
                await File.AppendAllTextAsync(targetFilePath, content, cancellationToken);
            }
            finally
            {
                FileWriteLock.Release();
            }
        }

        private void ValidateOptions(FileStorageOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.DirectoryName))
                throw new InvalidOperationException("Brakuje nazwy katalogu w konfiguracji.");

            if (string.IsNullOrWhiteSpace(options.FileName))
                throw new InvalidOperationException("Brakuje nazwy pliku w konfiguracji.");
        }

        private string EnsureAndGetDirectoryPath(string directoryName)
        {
            var directoryPath = Path.GetFullPath(Path.Combine(environment.ContentRootPath, directoryName));
            Directory.CreateDirectory(directoryPath);
            return directoryPath;
        }

        private string GetAvailableFilePath(string directoryPath, string baseFileName, long newContentSize)
        {
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(baseFileName);
            var extension = Path.GetExtension(baseFileName);
            int fileIndex = 0;

            while (true)
            {
                var currentFileName = fileIndex == 0
                    ? baseFileName
                    : $"{fileNameWithoutExt}_{fileIndex}{extension}";

                var filePath = Path.Combine(directoryPath, currentFileName);
                var fileInfo = new FileInfo(filePath);

                long currentSize = fileInfo.Exists ? fileInfo.Length : 0;

                if (currentSize + newContentSize <= MaxFileSizeBytes)
                {
                    return filePath;
                }

                fileIndex++;
            }
        }
    }
}
