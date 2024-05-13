namespace Files.Interface
{
    public interface IFileProcessor
    {
        Task<bool> ProcessFilesAsync();
    }
}
