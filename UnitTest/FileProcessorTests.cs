using NUnit.Framework;
using Moq;
using Files.Interface;
using System.Threading.Tasks;
using System.IO;

[TestFixture]
public class FileProcessorTests
{
    private Mock<ILog> _loggerMock;
    private Mock<IMetadataExtractor> _metadataExtractorMock;
    private FileProcessor _fileProcessor;
    private string _directoryPath = @"E:\FilesTest";
    private string _fileExtension = ".mp3";

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILog>();
        _metadataExtractorMock = new Mock<IMetadataExtractor>();
        _fileProcessor = new FileProcessor(_loggerMock.Object, _metadataExtractorMock.Object, _directoryPath, _fileExtension);
    }

    [Test]
    public async Task ProcessFilesAsync_ProcessesOnlyValidFiles()
    {
        // Arrange
        // Assuming your test environment already has a mix of valid and invalid files.
        _metadataExtractorMock.Setup(m => m.ExtractMetadataAsync(It.IsAny<string>()))
            .ReturnsAsync("Metadata Extracted");

        // Act
        await _fileProcessor.ProcessFilesAsync();

        // Assert
        _metadataExtractorMock.Verify(m => m.ExtractMetadataAsync(It.Is<string>(path => IsValidFile(path))),
            Times.AtLeastOnce());
        _loggerMock.Verify(l => l.Log(It.Is<string>(s => s.Contains("Processed:"))), Times.AtLeastOnce());
    }

    private bool IsValidFile(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        if (fileInfo.Length >= 50 * 1024 && fileInfo.Length <= 8 * 1024 * 1024)
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                byte[] buffer = new byte[10];
                fileStream.Read(buffer, 0, 10);
                return buffer[0] == 'I' && buffer[1] == 'D' && buffer[2] == '3' ||
                       (buffer[0] == 0xFF && (buffer[1] & 0xE0) == 0xE0);
            }
        }
        return false;
    }
}
