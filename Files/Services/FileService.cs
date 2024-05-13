using Files.Interface;

namespace Files.Services
{
    public class FileService : IFileService
    {
        public string[] GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern);
        }

        public long GetFileSize(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        public byte[] ReadFileHeader(string filePath, int size)
        {
            byte[] buffer = new byte[size];
            using (var fileStream = File.OpenRead(filePath))
            {
                fileStream.Read(buffer, 0, size);
            }
            return buffer;
        }
    }
}
