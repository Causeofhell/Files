namespace Files.Interface
{
    public interface IFileService
    {
        string[] GetFiles(string path, string searchPattern);
        long GetFileSize(string filePath);
        byte[] ReadFileHeader(string filePath, int size);
    }

}
