using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class FileProcessor
{
    private const int MinSize = 50 * 1024; // 50KB
    private const int MaxSize = 3 * 1024 * 1024; // 3MB

    public async Task ProcessFilesAsync()
    {
        var files = Directory.GetFiles("E:\\FilesTest", "*.mp3");
        var validFiles = files.Where(file => IsValidSize(file)).ToList();

        for (int i = 0; i < validFiles.Count; i += 3)
        {
            var tasks = validFiles.Skip(i).Take(3).Select(file => ProcessFileAsync(file));
            await Task.WhenAll(tasks);
        }
    }

    private bool IsValidSize(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        return fileInfo.Length >= MinSize && fileInfo.Length <= MaxSize;
    }

    private async Task ProcessFileAsync(string filePath)
    {
        try
        {
            var metadata = await MetadataExtractor.ExtractMetadataAsync(filePath);
            await File.WriteAllTextAsync(Path.ChangeExtension(filePath, ".txt"), metadata);

            Logger.Log($"Processed: {filePath}");
        }
        catch (Exception ex)
        {
            Logger.Log($"Error processing {filePath}: {ex.Message}");
        }
    }
}
