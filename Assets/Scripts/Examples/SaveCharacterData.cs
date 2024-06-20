using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using OdinSerializer;
using BasicDataUtils;
using CharacterDataProcessing;

/* Saving CharacterData to disk. Relevant data in a specified character are references, transformed to CharacterData (which stores information by value), serialized, 
 * compressed and stored to disk in its own data format (chardat = character data). 
 * by Marius Rubo, 2023
 * */

public class SaveCharacterData : MonoBehaviour
{
    [Tooltip("The character gameobject. Ensure all its component (e.g., 'Body') are named as expected in CharacterReference")]
    [SerializeField] GameObject characterToSave;

    [Tooltip("The character's id. Will be the file's name in the StreamingAssets folder")]
    [SerializeField] int characterID;

    CharacterReference character1ref;
    CharacterData characterData1;

    async void Start()
    {
        character1ref = new CharacterReference(characterToSave, characterID); // automatically reference bones and skinnedmeshrenderers
        characterData1 = new CharacterData(character1ref); // hold all relevant data by value
        await SaveCharacterDataToDisk(characterData1);
    }

    /// <summary>
    /// Saves all data in characterData to disk.
    /// </summarycharacter
    /// <param name="characterData">The character data to be saved. It stored its id internally, which is used in naming the file.</param>
    async Task SaveCharacterDataToDisk(CharacterData characterData) 
    {
        byte[] data = new byte[0];
        await Task.Run(() => data = SerializationUtility.SerializeValue(characterData1, DataFormat.Binary));
        data = await Gzip.CompressAsync(data);                   
        string filename = Path.Combine(Application.streamingAssetsPath, "CharacterData", characterData.id.ToString() + ".chardat"); // chardat = character data

        if (File.Exists(filename))
        {
            Debug.LogWarning("File already exists and will not be overwritten: " + filename);
        }
        else
        {
            await IO.WriteFileAsync(filename, data);
            Debug.Log("File was written to disk: " + filename);
        }
    }

}
