using System.ComponentModel.DataAnnotations;

namespace CatFact.API.Models
{
    public class FileStorageOptions
    {
        public string DirectoryName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;

        public long MaxFileSizeBytes { get; init; } = 5 * 1024 * 1024;

        public int MaxConcurrentWrites { get; init; } = 1;
    }
}
