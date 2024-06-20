using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* At least this one parameter in the process will likely need customization: the names of the individual textures on each material. Therefore, this info is stored
 * in this specific script. For more complex situations, an external database may be more appropriate. This script should work well without modification when using
 * Unity in its Standard Render Pipeline and with all materials using the Standard Shader. More high-quality character creation tools will, however, commonly provide 
 * different shaders for different materials (i.e., one shader for the skin, one for the eyes' cornea etc.). The trouble is that textures are named differently in 
 * different shaders and we want control over what textures we extract, store and apply. For instance, the albedo map is called "_MainTex" in the Standard Shader but 
 * may be called "_DiffuseMap", "_BaseMap" etc. in other shaders. 
 * Note that a second area of customization may be the names of all the character's parts which are used in CharacterReference. Since it may be most straightforward to
 * rename them manually on the character itself in most situations, I did not include this here under Customization, but that might be an appropriate extension for some.
 * by Marius Rubo, 2023
 * */
namespace CharacterDataProcessing
{
    public class Customization : MonoBehaviour
    {
        /// <summary>
        /// A function which looks up the names of up to 3 textures we would like to store, depending on the used shader.
        /// </summary>
        /// <param name="shaderName">The name of the used shader, e.g., "Standard" for the Standard shader.</param>
        /// <returns>An array of the names of the textures on that shader which we would like to retreive or adapt.</returns>
        public static string[] GetMapNamesForShader(string shaderName)
        {
            string[] mapNames = new string[3];

            // MakeHuman only uses one shader, the StandardShader. In contrast to more complex avatar creation software, it only comes with an albedo map.
            // This setup is designed to always get three maps. In this example we just transport the Albedo map three times to comply with the outer form. 
            if (shaderName.Contains("Standard"))
            {
                mapNames[0] = "_MainTex";
                mapNames[1] = "_MetallicGlossMap";
                mapNames[2] = "_BumpMap";
            }

            // the following code is not needed in this example, but I keep it here as example.
            // When importing iClone characters to Unity in URP, they come with a range of shaders which often have different names for
            // textures with relatively similar functions. The following handles those cases and ensures the correct textures are saved.
            /*
            if (shaderName.Contains("SkinShader_Variants_URP") || shaderName.Contains("RL_TeethShader_URP") || shaderName.Contains("RL_TongueShader_URP"))
            {
                mapNames[0] = "_DiffuseMap";
                mapNames[1] = "_MaskMap";
                mapNames[2] = "_NormalMap";
            }
            if (shaderName.Contains("Lit"))
            {
                mapNames[0] = "_BaseMap";
                mapNames[1] = "_MetallicGlossMap";
                mapNames[2] = "_BumpMap";
            }
            if (shaderName.Contains("RL_CorneaShader")) // complete name is "RL_CorneaShaderParallax_URP"
            {
                mapNames[0] = "_ScleraDiffuseMap";
                mapNames[1] = "_CorneaDiffuseMap";
                mapNames[2] = "_MaskMap";
            }
            if (shaderName.Contains("Hair"))
            {
                mapNames[0] = "_DiffuseMap";
                mapNames[1] = "_MaskMap";
                mapNames[2] = "_FlowMap";
            }
            */

            return mapNames;
        }
    }
}
