using System;
using System.Threading.Tasks;
using FileTransferTool.Helpers;

namespace FileTransferApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter the source file path (e.g., c:\\source\\large_file.bin):");
            string sourceFilePath = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("Enter the destination file path (e.g., d:\\destination\\large_file_copy.bin):");
            string destinationFilePath = Console.ReadLine() ?? string.Empty;

            try
            {
                await FileTransferHelper.TransferFile(sourceFilePath, destinationFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}
