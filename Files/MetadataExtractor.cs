using Files.Interface;
using System.Threading.Tasks;

public class MetadataExtractor : IMetadataExtractor
{
    public async Task<string> ExtractMetadataAsync(string filePath)
    {
        // Simulate metadata extraction
        await Task.Delay(100); // Simulated delay
        return RandomTextGenerator.GenerateRandomText();
    }
}
