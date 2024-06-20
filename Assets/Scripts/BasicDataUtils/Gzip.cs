using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/* Convenience wrapper functions to gzip compress and decompress byte arrays without blocking the main loop. Gzip itself is built-in natively in .NET.
 * by Marius Rubo, 2023
 * */

namespace BasicDataUtils
{
    public static class Gzip
    {
        /// <summary>
        /// Compresses a byte array asynchronously.
        /// </summary>
        /// <param name="bytes">The array to be compressed.</param>
        public static async Task<byte[]> CompressAsync(byte[] bytes)
        {
            return await Task.Run(() =>
            {
                using var memoryStream = new System.IO.MemoryStream();
                using var gzipStream = new System.IO.Compression.GZipStream(memoryStream, System.IO.Compression.CompressionLevel.Optimal);
                gzipStream.Write(bytes, 0, bytes.Length);
                gzipStream.Dispose();
                return memoryStream.ToArray();
            });
        }

        /// <summary>
        /// Decompresses a byte array asynchronously.
        /// </summary>
        /// <param name="bytes">The array to be decompressed.</param>
        public static async Task<byte[]> DecompressAsync(byte[] bytes)
        {
            return await Task.Run(() =>
            {
                using var memoryStream = new System.IO.MemoryStream(bytes);
                using var decompressStream = new System.IO.Compression.GZipStream(memoryStream, System.IO.Compression.CompressionMode.Decompress);
                using var outputStream = new System.IO.MemoryStream();
                decompressStream.CopyTo(outputStream);
                return outputStream.ToArray();
            });
        }
    }
}
