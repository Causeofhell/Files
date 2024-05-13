using Files.Interface;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class FileProcessor : IFileProcessor
{
    private const int MinSize = 50 * 1024; // 50KB
    private const int MaxSize = 8 * 1024 * 1024; // 8MB

    private readonly ILog _logger;
    private readonly IMetadataExtractor _metadataExtractor;
    private readonly string _directoryPath;
    private readonly string _fileExtension;

    public FileProcessor(ILog logger, IMetadataExtractor metadataExtractor, string directoryPath, string fileExtension)
    {
        _logger = logger;
        _metadataExtractor = metadataExtractor;
        _directoryPath = directoryPath;
        _fileExtension = fileExtension;
    }

    public async Task ProcessFilesAsync()
    {
        var files = Directory.GetFiles(_directoryPath, $"*{_fileExtension}");
        var validFiles = files.Where(file => IsValidSize(file) && IsValidMP3(file)).ToList();

        for (int i = 0; i < validFiles.Count; i += 3)
        {
            var tasks = validFiles.Skip(i).Take(3).Select(file => ProcessFileWithRetryAsync(file));
            await Task.WhenAll(tasks);
        }
    }

    private bool IsValidSize(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        if (fileInfo.Length < MinSize || fileInfo.Length > MaxSize)
        {
            _logger.Log($"File '{filePath}' is invalid due to inappropriate size: {fileInfo.Length} bytes.");
            return false;
        }
        return true;
    }

    private bool IsValidMP3(string filePath)
    {
        try
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[10];
                fileStream.Read(buffer, 0, 10);

                if (buffer[0] == 'I' && buffer[1] == 'D' && buffer[2] == '3')
                {
                    return true;
                }
                if (buffer[0] == 0xFF && (buffer[1] & 0xE0) == 0xE0)
                {
                    return true;
                }

                _logger.Log($"File '{filePath}' does not contain valid MP3 headers.");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.Log($"Error reading from MP3 file '{filePath}': {ex.Message}");
            return false;
        }
    }

    private async Task ProcessFileWithRetryAsync(string filePath)
    {
        int attempt = 0;
        const int maxAttempts = 3;
        bool success = false;

        while (attempt < maxAttempts && !success)
        {
            attempt++;
            try
            {
                var metadata = await _metadataExtractor.ExtractMetadataAsync(filePath);
                await File.WriteAllTextAsync(Path.ChangeExtension(filePath, ".txt"), metadata);
                _logger.Log($"Processed: {filePath}");
                success = true;
            }
            catch (Exception ex)
            {
                _logger.Log($"Attempt {attempt} failed for {filePath}: {ex.Message}");
                if (attempt == maxAttempts)
                {
                    _logger.Log($"Failed to process {filePath} after {maxAttempts} attempts.");
                }
                else
                {
                    await Task.Delay(1000); // Delay between retries
                }
            }
        }
    }
}
