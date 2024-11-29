using FileTransferTool.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileTransferTool.TestFileGenerator;

namespace FileTransferTool.Tests
{
    public class FileTransferHelperTests
    {
        private string _sourceFilePath;
        private string _destinationFilePath;

        [SetUp]
        public void Setup()
        {
            _sourceFilePath = @"C:\Users\Lenovo\Desktop\sourceTestFile.txt";
            _destinationFilePath = @"C:\Users\Lenovo\Desktop\Destination\destinationTestFile.txt";

            GenerateTestFile.Generate10MBTextFileLetters(_sourceFilePath);
        }

        [TearDown]
        public void Cleanup()
        {
            if (File.Exists(_sourceFilePath)) File.Delete(_sourceFilePath);
            if (File.Exists(_destinationFilePath)) File.Delete(_destinationFilePath);
        }

        [Test]
        public async Task TransferFile_ShouldSucceed_WhenPathsAreValid()
        {
            await FileTransferHelper.TransferFile(_sourceFilePath, _destinationFilePath);

            var result = await HashComputingHelper.CompareChecksums(_sourceFilePath, _destinationFilePath);
            Assert.IsTrue(File.Exists(_destinationFilePath));
            Assert.IsTrue(result.IsMatch);
        }

        [Test]
        public void TransferFile_ShouldThrowFileNotFoundException_WhenSourceFileDoesNotExist()
        {
            string invalidSourceFilePath = @"C:\NonExistentFile.txt";
            Assert.ThrowsAsync<FileNotFoundException>(async () => await FileTransferHelper.TransferFile(invalidSourceFilePath, _destinationFilePath));
        }

        [Test]
        public void TransferFile_ShouldThrowDirectoryNotFoundException_WhenDestinationDirectoryDoesNotExist()
        {
            string invalidDestinationFilePath = "nonExistentDirectory";
            Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await FileTransferHelper.TransferFile(_sourceFilePath, invalidDestinationFilePath));
        }
    }
}