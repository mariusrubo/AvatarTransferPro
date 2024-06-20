using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;
using OdinSerializer;
using BasicDataUtils;
using CharacterDataProcessing;

/* Simplest example for how data are loaded and applied to a character. Here, everything is done in the same script. in a real-world situation, 
 * this would be the case when a client already has the relevant character data stored on its own disk and the server tells it "Please load character data 102 
 * and apply it to character X". In other cases, the client may not have the relevant character data yet but needs to receive them from the server first. 
 * For this scenario, see "CharacterDataSender" and "CharacterDataReceiver" and how they are used in the example scene "ApplyCharacterDataMockNetwork".
 * by Marius Rubo, 2023
 * */

public class ApplyCharacterData : MonoBehaviour
{
    [Tooltip("The character gameobject. Ensure all its component (e.g., 'Body') are named as expected in CharacterReference")]
    [SerializeField] GameObject character;

    /// <summary>
    /// Loads character data from disk, deserializes them and applies them on a character.
    /// </summary>
    /// <param name="int">The id of the character data to be loaded. Ensure that there is a corresponding chardat-file in the StreamingAssets folder.</param>
    public async void LoadAndApplyCharacter(int id)
    {
        string filename = Path.Combine(Application.streamingAssetsPath, "CharacterData", id.ToString() + ".chardat");
        if (File.Exists(filename))
        {
            byte[] data = await IO.ReadFileAsync(filename); // asynchronous loading. May not make a difference for small files, but reduces fps drops in larger files
            data = await Gzip.DecompressAsync(data); // not too computationally demanding, so don't use Task.Run
            CharacterData characterData = new CharacterData();
            await Task.Run(() => characterData = SerializationUtility.DeserializeValue<CharacterData>(data, DataFormat.Binary)); // deserialize into CharacterData
            CharacterReference characterRef = new CharacterReference(character); // get the relevant references from the character (all names must be correct!)
            await CharacterData.ApplyCharacterDataToCharacter(characterData, characterRef); // apply to character step-by-step
        }
        else
        {
            Debug.LogWarning("File not found: " + filename);
        }
    }






}
