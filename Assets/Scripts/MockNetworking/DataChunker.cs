using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Functions to split larger byte[] arrays into chunks (which are small enough to be sent over a network) and to reassamble the chunks back to the original array.
 * by Marius Rubo, 2023
 * */

namespace MockNetworking
{
    public class DataChunker
    {
        private const int chunk_size = 256 * 1024; // 256kB // some networking environments will allow larger chunk sizes, some may need smaller chunk sizes for each message

        /// <summary>
        /// Split a byte array into chunks.
        /// </summary>
        /// <param name="data">The array to be split.</param>
        /// <returns>A list of byte arrays representing chunks of the original array.</returns>
        public static List<byte[]> SplitData(byte[] data)
        {
            List<byte[]> chunks = new List<byte[]>();
            int totalSize = data.Length;
            int fullChunks = totalSize / chunk_size;
            int remainingBytes = totalSize % chunk_size;

            for (int i = 0; i < fullChunks; i++)
            {
                byte[] chunk = new byte[chunk_size];
                System.Array.Copy(data, i * chunk_size, chunk, 0, chunk_size);
                chunks.Add(chunk);
            }

            if (remainingBytes > 0)
            {
                byte[] lastChunk = new byte[remainingBytes];
                System.Array.Copy(data, fullChunks * chunk_size, lastChunk, 0, remainingBytes);
                chunks.Add(lastChunk);
            }

            return chunks;
        }

        /// <summary>
        /// Reassamble list of byte arrays (chunks) into original byte array.
        /// </summary>
        /// <param name="chunks">The chunks which collectively hold the original array.</param>
        /// <returns>A byte array representing the original array before it was split.</returns>
        public static byte[] ReassembleData(List<byte[]> chunks)
        {
            int totalSize = 0;
            foreach (var chunk in chunks)
            {
                totalSize += chunk.Length;
            }

            byte[] data = new byte[totalSize];
            int offset = 0;
            foreach (var chunk in chunks)
            {
                System.Array.Copy(chunk, 0, data, offset, chunk.Length);
                offset += chunk.Length;
            }

            return data;
        }
    }
}
