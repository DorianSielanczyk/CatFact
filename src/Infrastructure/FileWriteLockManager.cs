using CatFact.API.Models;
using Microsoft.Extensions.Options;

namespace CatFact.API.Infrastructure
{
    public class FileWriteLockManager : IDisposable
    {
        public SemaphoreSlim Lock { get; }

        public FileWriteLockManager(IOptions<FileStorageOptions> options)
        {
            var maxConcurrent = options.Value.MaxConcurrentWrites;
            Lock = new SemaphoreSlim(maxConcurrent, maxConcurrent);
        }

        public void Dispose()
        {
            Lock.Dispose();
        }
    }
}
