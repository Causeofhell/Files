using Files.Interface;
using Moq;

[TestFixture]
public class FileProcessorTests
{
    private Mock<IFileService> _fileServiceMock;
    private Mock<ILog> _loggerMock;
    private Mock<IMetadataExtractor> _metadataExtractorMock;
    private FileProcessor _fileProcessor;
    private string _directoryPath = "E:\\FilesTest";
    private string _fileExtension = ".mp3";

    [SetUp]
    public void Setup()
    {
        // Mock services
        _fileServiceMock = new Mock<IFileService>();
        _loggerMock = new Mock<ILog>();
        _metadataExtractorMock = new Mock<IMetadataExtractor>();

        // Initialize FileProcessor
        _fileProcessor = new FileProcessor(_loggerMock.Object, _metadataExtractorMock.Object, _fileServiceMock.Object, _directoryPath, _fileExtension);

        // Setup mocks
        _fileServiceMock.Setup(fs => fs.GetFiles(_directoryPath, "*" + _fileExtension))
            .Returns(new string[] { "file1.mp3", "file2.mp3", "file3.mp3" });

        _fileServiceMock.Setup(fs => fs.GetFileSize(It.IsAny<string>()))
            .Returns(1024 * 1024); // 1MB

        _fileServiceMock.Setup(fs => fs.ReadFileHeader(It.IsAny<string>(), 10))
            .Returns(new byte[] { 0x49, 0x44, 0x33 }); // ID3 tags
    }

    [Test]
    public async Task ProcessFilesAsync_ValidFilesAreProcessed()
    {
        // Arrange
        _metadataExtractorMock.Setup(m => m.ExtractMetadataAsync(It.IsAny<string>()))
            .ReturnsAsync("Metadata");

        // Act
        await _fileProcessor.ProcessFilesAsync();

        // Assert
        _metadataExtractorMock.Verify(m => m.ExtractMetadataAsync(It.IsAny<string>()), Times.Exactly(3));
        _loggerMock.Verify(l => l.Log(It.Is<string>(s => s.Contains("Processed:"))), Times.Exactly(3));
    }

    [Test]
    public async Task ProcessFilesAsync_InvalidSize_NotProcessed()
    {
        // Arrange
        _fileServiceMock.Setup(fs => fs.GetFileSize(It.IsAny<string>()))
            .Returns(9 * 1024 * 1024); // 9MB

        // Act
        await _fileProcessor.ProcessFilesAsync();

        // Assert
        _metadataExtractorMock.Verify(m => m.ExtractMetadataAsync(It.IsAny<string>()), Times.Never);
        _loggerMock.Verify(l => l.Log(It.Is<string>(s => s.Contains("invalid due to inappropriate size"))), Times.AtLeastOnce());
    }
}
