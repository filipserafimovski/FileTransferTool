using FileTransferTool.Helpers;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework;
using System;

namespace FileTransferTool.Tests
{
    [TestFixture]
    public class FilePathValidationHelperTests
    {
        [Test]
        public void ValidatePaths_ValidPaths_DoesNotThrow()
        {
            string sourcePath = @"C:\Users\Lenovo\Desktop\10MBTestFile.txt";
            string destinationPath = @"C:\Users\Lenovo\Desktop\Destination\10MBTestFile.txt";

            Assert.DoesNotThrow(() => FilePathValidationHelper.ValidatePaths(sourcePath, destinationPath));
        }

        [Test]
        public void ValidatePaths_InvalidSourcePath_ThrowsArgumentException()
        {
            string invalidSourcePath = @"C:\NonExistentFile.txt";
            string destinationPath = @"C:\Users\Lenovo\Desktop\Destination\10MBTestFile.txt";

            var ex = Assert.Throws<FileNotFoundException>(() => FilePathValidationHelper.ValidatePaths(invalidSourcePath, destinationPath));
            Assert.That(ex.Message, Is.EqualTo($"The source file does not exist: {invalidSourcePath}"));
        }

        [Test]
        public void ValidatePaths_DestinationPathEmpty_ThrowsArgumentException()
        {
            string sourcePath = @"C:\Users\Lenovo\Desktop\10MBTestFile.txt";
            string invalidDestinationPath = string.Empty;

            var ex = Assert.Throws<ArgumentException>(() => FilePathValidationHelper.ValidatePaths(sourcePath, invalidDestinationPath));
            Assert.That(ex.Message, Is.EqualTo("Destination path cannot be empty."));
        }

        [Test]
        public void ValidatePaths_SourcePathEmpty_ThrowsArgumentException()
        {
            string sourcePath = string.Empty;
            string invalidDestinationPath = @"C:\Users\Lenovo\Desktop\Destination\10MBTestFile.txt";

            var ex = Assert.Throws<ArgumentException>(() => FilePathValidationHelper.ValidatePaths(sourcePath, invalidDestinationPath));
            Assert.That(ex.Message, Is.EqualTo("Source path cannot be empty."));
        }

        [Test]
        public void ValidatePaths_ShouldThrowArgumentException_WhenDestinationIsFolder()
        {
            string sourceFilePath = @"C:\Users\Lenovo\Desktop\10MBTestFile.txt";
            string destinationFolder = @"C:\Users\Lenovo\Desktop\Destination";

            var ex = Assert.Throws<ArgumentException>(() => FilePathValidationHelper.ValidatePaths(sourceFilePath, destinationFolder));
            Assert.That(ex.Message, Is.EqualTo($"The destination path '{destinationFolder}' is a directory. Please specify a file name."));
        }

    }
}
