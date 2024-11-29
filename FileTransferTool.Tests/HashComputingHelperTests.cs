using FileTransferTool.Helpers;
using System.Text;
using FileTransferTool.TestFileGenerator;

namespace FileTransferTool.Tests
{
    [TestFixture]
    public class HashComputingHelperTests
    {
        private string _filePath1;
        private string _filePath2;
        private string _filePath3;

        [SetUp]
        public void Setup()
        {
            _filePath1 = @"C:\Users\Lenovo\Desktop\1MBTextFileLetters1";
            _filePath2 = @"C:\Users\Lenovo\Desktop\1MBTextFileLetters2";
            _filePath3 = @"C:\Users\Lenovo\Desktop\1MBTextFileNumbers";
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(_filePath1)) File.Delete(_filePath1);
            if (File.Exists(_filePath2)) File.Delete(_filePath2);
            if (File.Exists(_filePath3)) File.Delete(_filePath3);
        }

        [Test]
        public async Task CompareChecksums_ShouldReturnMatch_WhenFilesAreIdentical()
        {
            GenerateTestFile.Generate10MBTextFileLetters(_filePath1);
            GenerateTestFile.Generate10MBTextFileLetters(_filePath2);

            var result = await HashComputingHelper.CompareChecksums(_filePath1, _filePath2);
            Assert.IsTrue(result.IsMatch);
            Assert.That(result.DestinationHash, Is.EqualTo(result.SourceHash));
        }

        [Test]
        public async Task CompareChecksums_ShouldReturnNoMatch_WhenFilesAreDifferent()
        {
            GenerateTestFile.Generate10MBTextFileLetters(_filePath1);
            GenerateTestFile.Generate10MBTextFileNumbers(_filePath3);

            var result = await HashComputingHelper.CompareChecksums(_filePath1, _filePath3);
            Assert.IsFalse(result.IsMatch);
            Assert.That(result.DestinationHash, Is.Not.EqualTo(result.SourceHash));
        }

        [Test]
        public void ComputeMD5Hash_SameInput_ReturnsSameHash()
        {
            string input1 = "Filip's File Transfer Tool";
            string input2 = "Filip's File Transfer Tool";

            byte[] data1 = Encoding.UTF8.GetBytes(input1);
            byte[] data2 = Encoding.UTF8.GetBytes(input2);

            string hash1 = HashComputingHelper.ComputeMD5Hash(data1);
            string hash2 = HashComputingHelper.ComputeMD5Hash(data2);

            Assert.That(hash2, Is.EqualTo(hash1));
        }

        [Test]
        public void ComputeMD5Hash_DifferentInput_ReturnsDifferentHash()
        {
            // Arrange
            byte[] data1 = Encoding.UTF8.GetBytes("Filip's File Transfer Tool");
            byte[] data2 = Encoding.UTF8.GetBytes("Filip's Tool for Transfering Files");

            // Act
            string hash1 = HashComputingHelper.ComputeMD5Hash(data1);
            string hash2 = HashComputingHelper.ComputeMD5Hash(data2);

            // Assert
            Assert.That(hash2, Is.Not.EqualTo(hash1));
        }
    }
}
