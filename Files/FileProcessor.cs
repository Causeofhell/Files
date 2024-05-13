using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class FileProcessor
{
    private const int MinSize = 50 * 1024; // 50KB
    private const int MaxSize = 8 * 1024 * 1024; // 8MB

    public async Task ProcessFilesAsync()
    {
        var files = Directory.GetFiles("E:\\FilesTest", "*.mp3");
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
        return fileInfo.Length >= MinSize && fileInfo.Length <= MaxSize;
    }

    private bool IsValidMP3(string filePath)
    {
        try
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[10];
                fileStream.Read(buffer, 0, 10);

                // Check for "ID3" tags which indicates metadata for MP3 files
                if (buffer[0] == 'I' && buffer[1] == 'D' && buffer[2] == '3')
                    return true;
                // Check for MPEG audio frame sync bits - a more complex check can be done here
                if (buffer[0] == 0xFF && (buffer[1] & 0xE0) == 0xE0)
                    return true;
            }
        }
        catch (Exception ex)
        {
            Logger.Log($"Error validating MP3 file {filePath}: {ex.Message}");
            return false;
        }

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
                var metadata = await MetadataExtractor.ExtractMetadataAsync(filePath);
                await File.WriteAllTextAsync(Path.ChangeExtension(filePath, ".txt"), metadata);
                Logger.Log($"Processed: {filePath}");
                success = true;
            }
            catch (Exception ex)
            {
                Logger.Log($"Attempt {attempt} failed for {filePath}: {ex.Message}");
                if (attempt == maxAttempts)
                    Logger.Log($"Failed to process {filePath} after {maxAttempts} attempts.");
                else
                    await Task.Delay(1000); // Delay between retries
            }
        }
    }
}
