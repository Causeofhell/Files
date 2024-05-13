using System.Threading.Tasks;

public class MetadataExtractor
{
    public static async Task<string> ExtractMetadataAsync(string filePath)
    {
        // Simulate metadata extraction
        await Task.Delay(100); // Simulated delay
        return RandomTextGenerator.GenerateRandomText();
    }
}
