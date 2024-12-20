﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FileTransferTool.App.Models;

namespace FileTransferTool.Helpers
{
    public static class HashComputingHelper
    {
        public static async Task<string> ComputeFileSHA256HashAsync(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous))
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = await sha256.ComputeHashAsync(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public static string ComputeMD5Hash(byte[] buffer)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(buffer);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public async static Task<ChecksumResult> CompareChecksums(string sourceFilePath, string destinationFilePath)
        {
            string sourceHash = await ComputeFileSHA256HashAsync(sourceFilePath);
            string destinationHash = await ComputeFileSHA256HashAsync(destinationFilePath);
            var isMatch = sourceHash == destinationHash;

            return new ChecksumResult() { SourceHash = sourceHash, DestinationHash = destinationHash, IsMatch = isMatch };
        }
    }
}
