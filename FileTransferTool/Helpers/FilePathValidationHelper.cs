using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferTool.Helpers
{
    public static class FilePathValidationHelper
    {
        public static void ValidatePaths(string sourceFilePath, string destinationFilePath)
        {
            ValidateSourceFilePath(sourceFilePath);
            ValidateDestinationFilePath(destinationFilePath);
        }

        private static void ValidateSourceFilePath(string sourceFilePath)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath))
            {
                throw new ArgumentException("Source path cannot be empty.");
            }

            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException($"The source file does not exist: {sourceFilePath}");
            }
        }

        private static void ValidateDestinationFilePath(string destinationFilePath)
        {
            if (string.IsNullOrWhiteSpace(destinationFilePath))
            {
                throw new ArgumentException("Destination path cannot be empty.");
            }

            string destinationDirectory = Path.GetDirectoryName(destinationFilePath) ?? string.Empty;
            if (string.IsNullOrEmpty(destinationDirectory) || !Directory.Exists(destinationDirectory))
            {
                throw new DirectoryNotFoundException($"The destination directory does not exist: {destinationDirectory}");
            }

            if (Directory.Exists(destinationFilePath))
            {
                throw new ArgumentException($"The destination path '{destinationFilePath}' is a directory. Please specify a file name.");
            }
        }
    }

}
