namespace Files.Interface
{
    public interface IMetadataExtractor
    {
        Task<string> ExtractMetadataAsync(string filePath);
    }
}
