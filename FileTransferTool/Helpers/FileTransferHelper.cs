using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileTransferTool.App.Models;

namespace FileTransferTool.Helpers
{
    public static class FileTransferHelper
    {
        private const int BlockSize = 1 * 1024 * 1024; // 1MB
        private static readonly BlockingCollection<(byte[] Data, string Hash, long Position)> BlockQueue = new BlockingCollection<(byte[], string, long)>(boundedCapacity: 10);

        public static async Task TransferFile(string sourceFilePath, string destinationFilePath)
        {
            FilePathValidationHelper.ValidatePaths(sourceFilePath, destinationFilePath);

            Task producerTask = Task.Run(() => ProduceBlocks(sourceFilePath));
            Task consumerTask = Task.Run(() => ConsumeBlocks(destinationFilePath));

            await Task.WhenAll(producerTask, consumerTask);

            await VerifyFinalFileHash(sourceFilePath, destinationFilePath);
        }

        private static void ProduceBlocks(string sourceFilePath)
        {
            using (var sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                long fileSize = sourceStream.Length;
                long position = 0;

                while (position < fileSize)
                {
                    int bytesToRead = (int)Math.Min(BlockSize, fileSize - position);
                    byte[] buffer;
                    string hash, verificationHash;

                    // Added this part to ensure blocks are not read corruptly to prevent infinitely
                    // looping in the while in the ConsumeBlocks method (as pointed out by Joseph).
                    // Keeps on reading twice from source until the hashes match.
                    while (true) 
                    {
                        buffer = new byte[bytesToRead];

                        // Read the block
                        sourceStream.Seek(position, SeekOrigin.Begin);
                        sourceStream.Read(buffer, 0, bytesToRead);
                        hash = HashComputingHelper.ComputeMD5Hash(buffer);

                        // Read the block once again
                        byte[] verificationBuffer = new byte[bytesToRead];
                        sourceStream.Seek(position, SeekOrigin.Begin);
                        sourceStream.Read(verificationBuffer, 0, bytesToRead);
                        verificationHash = HashComputingHelper.ComputeMD5Hash(verificationBuffer);

                        // Compare hashes of both reads, if true break
                        if (hash == verificationHash)
                        {
                            break;
                        }
                    }

                    BlockQueue.Add((buffer, hash, position));

                    var positionOutput = position == 0 ? position : position / 1024;
                    Console.WriteLine($"position: {positionOutput}, hash: {hash}");

                    position += bytesToRead;
                }
            }

            BlockQueue.CompleteAdding();
        }

        private static async void ConsumeBlocks(string destinationFilePath)
        {
            using (var destinationStream = new FileStream(destinationFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                foreach (var (data, hash, position) in BlockQueue.GetConsumingEnumerable())
                {
                    while (true)
                    {
                        await destinationStream.WriteAsync(data, 0, data.Length);
                        await destinationStream.FlushAsync();

                        destinationStream.Seek(position, SeekOrigin.Begin);
                        var buffer = new byte[data.Length];
                        await destinationStream.ReadAsync(buffer, 0, buffer.Length);

                        string computedHash = HashComputingHelper.ComputeMD5Hash((buffer));

                        if (computedHash == hash)
                            break;

                        // Hashes are not the same, truncate the block
                        Console.WriteLine("Destination hash does not match source hash. Resubmitting...");
                        destinationStream.SetLength(position);
                        destinationStream.Seek(position, SeekOrigin.Begin);
                    }
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
