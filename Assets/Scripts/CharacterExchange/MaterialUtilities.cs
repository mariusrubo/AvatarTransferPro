using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

/* Functionality to handle data on materials by value. Here we actually only handle the textures themselves and assume that additional parameters (e.g., glossiness)
 * remain unchanged among different characters of the same type. Also here we limit ourselves to handling three textures on a material. Many high-quality characters
 * will come with more textures per material, but in my own experience all textures beyond the first three have only a marginal effect on the visual appearance, at least
 * they don't seem to justify the extended use of data processing and transmission. Extend if needed. This script makes use of the FreeImage project to perform texture 
 * processing operations async which would otherwise result in noticable fps hickups.
 * by Marius Rubo, 2023
 * */
namespace CharacterDataProcessing
{
    public class MaterialUtilities : MonoBehaviour
    {
        /// <summary>
        /// A class which holds texture data of a specific material. Note that an individual SkinnedMeshRenderer can have several materials.
        /// </summary>
        public class MaterialData
        {
            public byte[] map1;
            public byte[] map2;
            public byte[] map3; 

            //public int renderMode; // examples of additional data which may be transmitted
            //public int textureWidth;
            //public Color textureColor;
            //public float textureSmoothness;

            /// <summary>
            /// Just an empty constructor, sometimes used before deserializing a byte array into a MaterialData object. 
            /// </summary>
            public MaterialData()
            {

            }
        }

        /// <summary>
        /// A class which holds all materials of one SkinnedMeshRenderer.
        /// </summary>
        public class MaterialsData
        {
            public MaterialData[] materialData;

            public MaterialsData()
            {

            }

            /// <summary>
            /// The constructor for the case that all textures should be given the same size.
            /// </summary>
            /// <param name="materials">array of all the materials of a specific SkinnedMeshRenderer.</param>
            /// <param name="imgsize">Size of the textures. If left out or set to 0, the original size will be used.</param>
            /// <returns>A MaterialsData objects which holds all the relevant textures by value.</returns>
            public MaterialsData(Material[] materials, int imgsize = 0) // allow to just insert one value which is then used for all materials
            {
                int[] imgsizes = new int[materials.Length];
                for (int i = 0; i < imgsizes.Length; i++) imgsizes[i] = imgsize; // just copy the value i times and use function below for convenience
                InitializeMaterialTextures(materials, imgsizes);
            }

            /// <summary>
            /// The constructor for the case that textures should be given different sizes. E.g., when a SkinnedMeshRenderer holds both the face's skin and the leg's skin
            /// in separate materials, one may choose to store the face's skin in a high resolution but the leg's skin in a relatively low resolution to save ressources.
            /// </summary>
            /// <param name="materials">array of all the materials of a specific SkinnedMeshRenderer.</param>
            /// <param name="imgsizes">Array with the size of the textures in the order of the materials in the materials array.</param>
            /// <returns>A MaterialsData objects which holds all the relevant textures by value.</returns>
            public MaterialsData(Material[] materials, int[] imgsizes) // allow to specify size of each entry in material
            {
                InitializeMaterialTextures(materials, imgsizes);
            }

            /// <summary>
            /// The actual function that goes initiates the retreival of the individual textures.
            /// </summary>
            /// <param name="materials">array of all the materials of a specific SkinnedMeshRenderer.</param>
            /// <param name="imgsizes">Array with the size of the textures in the order of the materials in the materials array.</param>
            private void InitializeMaterialTextures(Material[] materials, int[] imgsizes)
            {
                materialData = new MaterialData[materials.Length];

                for (int i = 0; i < materials.Length; i++)
                {
                    string[] mapNames = Customization.GetMapNamesForShader(materials[i].shader.name);
                    materialData[i] = new MaterialData();
                    materialData[i].map1 = ProcessTexture(materials[i].GetTexture(mapNames[0]), imgsizes[i]);
                    materialData[i].map2 = ProcessTexture(materials[i].GetTexture(mapNames[1]), imgsizes[i]);
                    materialData[i].map3 = ProcessTexture(materials[i].GetTexture(mapNames[2]), imgsizes[i]);
                }
            }

            /// <summary>
            /// Initiates the processing of an individual texture.
            /// </summary>
            /// <param name="texture">Texture object retreived from a material.</param>
            /// <param name="size">The size in which the texture should be returned. Textures are always quadradic here.</param>
            /// <returns>A raw byte arraw holding all texture data.</returns>
            private byte[] ProcessTexture(Texture texture, int size)
            {
                return texture != null ? TextureProcessing.GetPixelsFromTexture2D(texture, size) : null;
            }
        }

        /// <summary>
        /// Applies MaterialsData, which store texture data by value, onto a renderer.
        /// </summary>
        /// <param name="matdat">The MaterialsData which holds texture data of all materials by value.</param>
        /// <param name="ren">The renderer on which to apply those data.</param>
        public static async Task ApplyMaterialsData(MaterialsData matdat, Renderer ren)
        {
            for (int i = 0; i < matdat.materialData.Length; i++)
            {
                string[] mapNames = Customization.GetMapNamesForShader(ren.materials[i].shader.name);

                AsyncTextureImport.TextureImporter importer = new AsyncTextureImport.TextureImporter();
                if (matdat.materialData[i].map1 != null)
                {
                    Texture2D tex = await importer.ImportTextureAsync(matdat.materialData[i].map1, AsyncTextureImport.FREE_IMAGE_FORMAT.FIF_PNG);
                    //Texture2D tex = await importer.ImportTextureAsync(matdat.materialData[i].map1, AsyncTextureImport.FREE_IMAGE_FORMAT.FIF_JPEG); //JPEG is smaller in file size but was not always read correctly in tests
                    tex = await TextureProcessing.DuplicateTexture(tex); // Texture must be duplicated to be readable. Often one of the slowest operations here
                    tex.Apply();
                    ren.sharedMaterials[i].SetTexture(mapNames[0], tex);
                }

                if (matdat.materialData[i].map2 != null)
                {
                    Texture2D tex = await importer.ImportTextureAsync(matdat.materialData[i].map2, AsyncTextureImport.FREE_IMAGE_FORMAT.FIF_PNG);
                    tex = await TextureProcessing.DuplicateTexture(tex);
                    tex.Apply();
                    ren.sharedMaterials[i].SetTexture(mapNames[1], tex);
                }

                if (matdat.materialData[i].map3 != null)
                {
                    Texture2D tex = await importer.ImportTextureAsync(matdat.materialData[i].map3, AsyncTextureImport.FREE_IMAGE_FORMAT.FIF_PNG);
                    tex = await TextureProcessing.DuplicateTexture(tex);
                    tex.Apply();
                    ren.sharedMaterials[i].SetTexture(mapNames[2], tex);
                }
            }
        }

    }
}
