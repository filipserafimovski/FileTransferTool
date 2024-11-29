using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FileTransferTool.TestFileGenerator
{
    public class GenerateTestFile
    {
        static void Main(string[] args)
        {
            int sizeInGB = 1;
            string outputFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string outputFile = Path.Combine(outputFolder, $"{sizeInGB}GBTestFile.txt"); // Using .txt extension for text data
            GenerateLargeTextFile(outputFile, sizeInGB);
        }

        // Generate a [sizeInGB]GB test file with random ASCII text data and save it on Desktop
        // Change the value of sizeInGB to specify file size
        static void GenerateLargeTextFile(string filePath, int sizeInGB)
        {
            Console.WriteLine($"Generating a {sizeInGB} GB test file at {filePath}...");

            Random rng = new Random();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=[]{}|;:,.<>?/~";
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fs))
            {
                for (int i = 0; i < sizeInGB * 1024 * 1024 * 1024; i++)
                {
                    char randomChar = chars[rng.Next(chars.Length)];
                    writer.Write(randomChar);
                }
            }

            Console.WriteLine($"Test file created successfully at: {filePath}");
        }

        // Generates 10MB file, 1MB worth of lowercase characters a-j each
        // Used this for initial testing to monitor the threads' order of execution
        // Update: using this once again for checksum tests, along with Generate10MBTextFileNumbers method below
        public static void Generate10MBTextFileLetters(string filePath)
        {
            Console.WriteLine($"Generating a 10 MB test file at {filePath}...");

            Random rng = new Random();
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fs))
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 1024 * 1024; j++)
                    {
                        if (i == 0)
                            writer.Write("a");
                        else if (i == 1)
                            writer.Write("b");
                        else if (i == 2)
                            writer.Write("c");
                        else if (i == 3)
                            writer.Write("d");
                        else if (i == 4)
                            writer.Write("e");
                        else if (i == 5)
                            writer.Write("f");
                        else if (i == 6)
                            writer.Write("g");
                        else if (i == 7)
                            writer.Write("h");
                        else if (i == 8)
                            writer.Write("i");
                        else
                            writer.Write("j");
                    }
                }
            }

            Console.WriteLine($"Test file created successfully at: {filePath}");
        }

        public static void Generate10MBTextFileNumbers(string filePath)
        {
            Console.WriteLine($"Generating a 10 MB test file at {filePath}...");

            Random rng = new Random();
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var writer = new StreamWriter(fs))
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 1024 * 1024; j++)
                    {
                        if (i == 0)
                            writer.Write("0");
                        else if (i == 1)
                            writer.Write("1");
                        else if (i == 2)
                            writer.Write("2");
                        else if (i == 3)
                            writer.Write("3");
                        else if (i == 4)
                            writer.Write("4");
                        else if (i == 5)
                            writer.Write("5");
                        else if (i == 6)
                            writer.Write("6");
                        else if (i == 7)
                            writer.Write("7");
                        else if (i == 8)
                            writer.Write("8");
                        else
                            writer.Write("9");
                    }
                }
            }

            Console.WriteLine($"Test file created successfully at: {filePath}");
        }
    }
}
