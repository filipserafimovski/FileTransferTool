using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FileTransferTool.TestFileGenerator
{
    public class GenerateTestFile
    {
        const int fileSizeInMB = 10;
        const int bytesPerMB = 1024 * 1024;

        static void Main(string[] args)
        {
            int sizeInGB = 1;
            string outputFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputFile = Path.Combine(outputFolder, $"{sizeInGB}GBTestFile.txt"); // Using .txt extension for text data
            GenerateLargeTextFile(outputFile, sizeInGB);
        }

        static void GenerateLargeTextFile(string filePath, int sizeInGB)
        {
            Console.WriteLine($"Generating a {sizeInGB} GB test file at {filePath}...");

            Random rng = new Random();
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[1024 * 1024];
                for (long i = 0; i < sizeInGB * 1024L; i++)
                {
                    rng.NextBytes(buffer);
                    fs.Write(buffer, 0, buffer.Length);
                }
            }

            Console.WriteLine($"Test file created successfully at: {filePath}");
        }

        public static void Generate10MBTextFile(string filePath, string characterType)
        {
            char[] letters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j' };
            char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] toWriteValues;

            if (characterType != "letters" && characterType != "digits")
                return;
            else if (characterType == "letters")
                toWriteValues = letters;
            else
                toWriteValues = digits;

            Console.WriteLine($"Generating a 10 MB test file at {filePath}...");

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fs))
            {
                for (int i = 0; i < fileSizeInMB; i++)
                {
                    char charToWrite = toWriteValues[i % toWriteValues.Length];
                    string chunk = new string(charToWrite, bytesPerMB);
                    writer.Write(chunk);
                }
            }

            Console.WriteLine($"Test file created successfully at: {filePath}");
        }
    }
}
