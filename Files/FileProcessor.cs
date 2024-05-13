using Files.Interface;

public class FileProcessor : IFileProcessor
{
    private const int MinSize = 50 * 1024; // 50KB
    private const int MaxSize = 8 * 1024 * 1024; // 8MB

    private readonly IFileService _fileService;
    private readonly ILog _logger;
    private readonly IMetadataExtractor _metadataExtractor;
    private readonly string _directoryPath;
    private readonly string _fileExtension;

    public FileProcessor(ILog logger, IMetadataExtractor metadataExtractor, IFileService fileService, string directoryPath, string fileExtension)
    {
        _logger = logger;
        _metadataExtractor = metadataExtractor;
        _fileService = fileService;
        _directoryPath = directoryPath;
        _fileExtension = fileExtension;
    }

    public async Task<bool> ProcessFilesAsync()
    {
        var files = _fileService.GetFiles(_directoryPath, $"*{_fileExtension}");
        if (!files.Any())
            return false; // No more files to process

        var validFiles = files.Where(file => IsValidSize(file) && IsValidMP3(file)).ToList();
        for (int i = 0; i < validFiles.Count; i += 3)
        {
            var tasks = validFiles.Skip(i).Take(3).Select(file => ProcessFileWithRetryAsync(file));
            await Task.WhenAll(tasks);
        }
        return true; // Continue processing if needed
    }

    private bool IsValidSize(string filePath)
    {
        long fileSize = _fileService.GetFileSize(filePath);
        if (fileSize < MinSize || fileSize > MaxSize)
        {
            _logger.Log($"File '{filePath}' is invalid due to inappropriate size: {fileSize} bytes.");
            return false;
        }
        return true;
    }

    private bool IsValidMP3(string filePath)
    {
        byte[] buffer = _fileService.ReadFileHeader(filePath, 10);
        if (buffer[0] == 'I' && buffer[1] == 'D' && buffer[2] == '3' || buffer[0] == 0xFF && (buffer[1] & 0xE0) == 0xE0)
        {
            return true;
        }
        _logger.Log($"File '{filePath}' does not contain valid MP3 headers.");
        return false;
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
