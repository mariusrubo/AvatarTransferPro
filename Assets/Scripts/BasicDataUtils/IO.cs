using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;

/* Convenience wrapper functions to read files from disk and write without blocking the main loop.
 * by Marius Rubo, 2023
 * */

namespace BasicDataUtils
{
    public static class IO
    {
        /// <summary>
        /// Reads all bytes from a file asynchronously.
        /// </summary>
        /// <param name="filename">The path to the file.</param>
        public static async Task<byte[]> ReadFileAsync(string filename)
        {
            try
            {
                using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    byte[] fileData = new byte[fileStream.Length];
                    await fileStream.ReadAsync(fileData, 0, fileData.Length);
                    return fileData;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"An error occurred: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Writes bytes to a file asynchronously.
        /// </summary>
        /// <param name="filename">The path where the file should be written.</param>
        /// <param name="data">The byte array to write to the file.</param>
        public static async Task WriteFileAsync(string filename, byte[] data)
        {
            // Ensure the directory exists
            string directory = Path.GetDirectoryName(filename);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Open a file stream for writing with the 'Create' mode which will create the file if it doesn't exist or overwrite it if it does
            using (FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                // Write the data asynchronously to the file
                await fileStream.WriteAsync(data, 0, data.Length);
            }
        }
    }
}
