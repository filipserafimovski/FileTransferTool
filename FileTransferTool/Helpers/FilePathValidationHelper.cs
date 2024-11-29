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
            if (string.IsNullOrWhiteSpace(sourceFilePath) || string.IsNullOrWhiteSpace(destinationFilePath))
            {
                throw new ArgumentException("Source and destination paths cannot be empty.");
            }

            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException($"The source file does not exist: {sourceFilePath}");
            }

            string destinationDirectory = Path.GetDirectoryName(destinationFilePath) ?? string.Empty;
            if (!Directory.Exists(destinationDirectory) || string.IsNullOrEmpty(destinationDirectory))
            {
                throw new DirectoryNotFoundException($"The destination directory does not exist: {destinationDirectory}");
            }

            // Check to see if user provided directoryName instead of fileName for the destination
            if (Directory.Exists(destinationFilePath))
            {
                throw new ArgumentException($"The destination path '{destinationFilePath}' is a directory. Please specify a file name.");
            }
        }
    }
}
