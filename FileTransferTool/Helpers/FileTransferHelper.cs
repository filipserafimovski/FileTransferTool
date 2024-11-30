using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileTransferTool.App.Models;

namespace FileTransferTool.Helpers
{
    public static class FileTransferHelper
    {
        private const int BlockSize = 1 * 1024 * 1024; // 1MB

        public static async Task TransferFile(string sourceFilePath, string destinationFilePath)
        {
            FilePathValidationHelper.ValidatePaths(sourceFilePath, destinationFilePath);

            var thread1Blocks = new List<(string hash, byte[] data)>();
            var thread2Blocks = new List<(string hash, byte[] data)>();

            // Process blocks in parallel
            using (var sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                long fileSize = sourceStream.Length;
                int totalBlocks = (int)Math.Ceiling((double)fileSize / BlockSize);

                var processingTasks = new[]
                {
                    Task.Run(() => ProcessBlocks(sourceStream, thread1Blocks, 0, totalBlocks)),
                    Task.Run(() => ProcessBlocks(sourceStream, thread2Blocks, 1, totalBlocks))
                };

                await Task.WhenAll(processingTasks);
            }

            // Write in serial, alternating between thread1Blocks and thread2Blocks
            using (var destinationStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                await WriteAndVerifyBlocks(destinationStream, thread1Blocks, thread2Blocks);
            }

            await VerifyFinalFileHash(sourceFilePath, destinationFilePath);
        }

        private static void ProcessBlocks(FileStream sourceStream, List<(string hash, byte[] data)> blocks, int startIndex, int totalBlocks)
        {
            for (int blockIndex = startIndex; blockIndex < totalBlocks; blockIndex += 2)
            {
                long blockStart = blockIndex * BlockSize;
                int bytesToRead = (int)Math.Min(BlockSize, sourceStream.Length - blockStart);

                var buffer = new byte[bytesToRead];

                lock (sourceStream)
                {
                    sourceStream.Seek(blockStart, SeekOrigin.Begin);
                    sourceStream.Read(buffer, 0, bytesToRead);
                }

                string hash = HashComputingHelper.ComputeMD5Hash(buffer);
                blocks.Add((hash, buffer));

                var position = blockStart == 0 ? blockStart : blockStart / 1024;
                Console.WriteLine($"position = {position}, hash = {hash}");
            }
        }

        private static async Task WriteAndVerifyBlocks(FileStream destinationStream, List<(string hash, byte[] data)> thread1Blocks, List<(string hash, byte[] data)> thread2Blocks)
        {
            int totalBlocks = thread1Blocks.Count + thread2Blocks.Count;
            int t1Index = 0, t2Index = 0;

            for (int i = 0; i < totalBlocks; i++)
            {
                var currentBlock = (i % 2 == 0 && t1Index < thread1Blocks.Count) ? thread1Blocks[t1Index++] : thread2Blocks[t2Index++];

                while (true)
                {
                    long startPosition = destinationStream.Position;
                    await destinationStream.WriteAsync(currentBlock.data, 0, currentBlock.data.Length);
                    await destinationStream.FlushAsync();

                    destinationStream.Seek(startPosition, SeekOrigin.Begin);
                    var buffer = new byte[currentBlock.data.Length];
                    await destinationStream.ReadAsync(buffer, 0, buffer.Length);

                    string computedHash = HashComputingHelper.ComputeMD5Hash(buffer);

                    if (computedHash == currentBlock.hash)
                        break;

                    // Hashes are not the same; truncate the block
                    Console.WriteLine("Destination hash does not match source hash. Resubmitting...");
                    destinationStream.SetLength(startPosition);
                    destinationStream.Seek(startPosition, SeekOrigin.Begin);
                }
            }
        }

        private static async Task VerifyFinalFileHash(string sourceFilePath, string destinationFilePath)
        {
            ChecksumResult result = await HashComputingHelper.CompareChecksums(sourceFilePath, destinationFilePath);

            Console.WriteLine("\nFile Transfer Complete.\r\n");
            Console.WriteLine($"Source File Checksum: {result.SourceHash}");
            Console.WriteLine($"Destination File Checksum: {result.DestinationHash}");

            if (result.IsMatch)
            {
                Console.WriteLine("File transfer verified successfully.");
            }
            else
            {
                Console.WriteLine("File transfer verification failed. Hashes do not match.");
            }
        }
    }
}
