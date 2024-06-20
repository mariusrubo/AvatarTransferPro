using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/* Low-level functions which process texture data.
 * by Marius Rubo, 2023
 * */

namespace CharacterDataProcessing
{
    public class TextureProcessing
    {
        /// <summary>
        /// Obtains raw data from a Texture object. This works even when the Texture is set to "NonReadable".
        /// </summary>
        /// <param name="texture">The Texture from which to obtain data.</param>
        /// <param name="size">The size in width and height to which the texture should be scaled before data is obtained.</param>
        /// <returns>A raw byte arraw holding all texture data.</returns>
        public static byte[] GetPixelsFromTexture2D(Texture texture, int size = 0)
        {
            if (size == 0) size = texture.width; // use original size of not otherwise specified
            RenderTexture tmp = RenderTexture.GetTemporary(size, size, 0, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB); // careful: "Linear" leads to darkened color when used in conjunction with Texture2D
            Graphics.Blit(texture, tmp); // // Blit the pixels on texture to the RenderTexture. Blit rescales automatically! no need for Graphics.ConvertTexture or Texture2D.Reinitialize
            RenderTexture previous = RenderTexture.active; // Backup the currently set RenderTexture
            RenderTexture.active = tmp; // Set the current RenderTexture to the temporary one we created

            // Create a new readable Texture2D to copy the pixels to it
            Texture2D myTexture2D = new Texture2D(size, size, TextureFormat.RGBA32, false); // just always use RGBA32, because DXT1 is compressed and cannot easily be SetPixels
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0); // Copy the pixels from the RenderTexture to the new Texture
            myTexture2D.Apply();
            RenderTexture.active = previous; // Reset the active RenderTexture
            RenderTexture.ReleaseTemporary(tmp); // Release the temporary RenderTexture

            byte[] img = myTexture2D.EncodeToPNG();
            //byte[] img = myTexture2D.EncodeToJPG(95);

            return (img);
        }

        /// <summary>
        /// Duplicate a texture. May seem unnecessary at first, but is needed to bypass the texture's initial non-readability.
        /// </summary>
        /// <param name="source">The Texture2D to duplicate.</param>
        /// <returns>Another Texture2D which can then be used in subsequent steps.</returns>
        public static async Task<Texture2D> DuplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.sRGB); // linear leads to darker color in my case

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);

            await Task.Yield();
            return readableText;
        }
    }
}
