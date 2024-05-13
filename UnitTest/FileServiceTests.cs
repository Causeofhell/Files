using Files.Services;
using System.Text;

[TestFixture]
public class FileServiceTests
{
    private string _testDirectory;
    private FileService _fileService;

    [SetUp]
    public void Setup()
    {
        // Create a test directory
        _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);

        // Initialize FileService
        _fileService = new FileService();
    }

    [TearDown]
    public void TearDown()
    {
        // Clean up the test directory
        Directory.Delete(_testDirectory, true);
    }

    [Test]
    public void GetFiles_ReturnsCorrectFiles()
    {
        // Arrange
        string testFilePath = Path.Combine(_testDirectory, "testfile.txt");
        File.WriteAllText(testFilePath, "Hello World");

        // Act
        var files = _fileService.GetFiles(_testDirectory, "*.txt");

        // Assert
        Assert.That(files, Has.Exactly(1).EqualTo(testFilePath));
    }

    [Test]
    public void GetFileSize_ReturnsCorrectSize()
    {
        // Arrange
        string testFilePath = Path.Combine(_testDirectory, "testfile.txt");
        string content = "Hello World";
        File.WriteAllText(testFilePath, content);
        long expectedSize = Encoding.UTF8.GetByteCount(content);

        // Act
        long size = _fileService.GetFileSize(testFilePath);

        // Assert
        Assert.AreEqual(expectedSize, size);
    }

    [Test]
    public void ReadFileHeader_ReadsCorrectBytes()
    {
        // Arrange
        string testFilePath = Path.Combine(_testDirectory, "testfile.bin");
        byte[] expectedBytes = new byte[] { 1, 2, 3, 4, 5 };
        using (var fs = new FileStream(testFilePath, FileMode.Create, FileAccess.Write))
        {
            fs.Write(expectedBytes, 0, expectedBytes.Length);
        }

        // Act
        byte[] header = _fileService.ReadFileHeader(testFilePath, 5);

        // Assert
        Assert.That(header, Is.EqualTo(expectedBytes));
    }
}
