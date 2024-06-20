using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;
using OdinSerializer;
using BasicDataUtils;
using CharacterDataProcessing;
using MockNetworking;

/* Character data are read from disk, split into chunks and transmitted to another script. This example shows in priniciple a use case where one
 * computer (typically the server) has character data stored on a disk and sends it to other computers (the clients) which then apply it. 
 * For a simpler version of applying new character data to a character, see "ApplyCharacterData" used in the example scene "ApplyCharacterDataSimple".
 * by Marius Rubo, 2023
 * */
public class CharacterDataSender : MonoBehaviour
{
    [SerializeField] CharacterDataReceiver characterDataReceiver; // the script receiving the data. In a networked environment, this will be on a different computer altogether.

    /// <summary>
    /// Loads character data from disk, sliceses them into chunks and transmits the chunks to a receiver. 
    /// </summary>
    /// <param name="int">The id of the character data to be loaded. Ensure that there is a corresponding chardat-file in the StreamingAssets folder.</param>
    public async void LoadAndSendCharacter(int id)
    {
        string filename = Path.Combine(Application.streamingAssetsPath, "CharacterData", id.ToString() + ".chardat");
        if (File.Exists(filename))
        {
            byte[] data = await IO.ReadFileAsync(filename); // load
            List<byte[]> dataToSend = DataChunker.SplitData(data); // split into chunks small enough to send in common netcode
            byte[] nChunks = SerializationUtility.SerializeValue(dataToSend.Count, DataFormat.Binary); // manually serialize this int into byte[] to keep "Message" more frugal across contexts (could also be an additional variable in "Message")
            Message msg = new Message(MessageType.CharacterDataAnnouncement, id, nChunks); // inform receiver how many chunks it should expect
            characterDataReceiver.ReceiveMessage(msg);

            for (int i = 0; i < dataToSend.Count; i++)
            {
                msg = new Message(MessageType.CharacterDataChunk, i, dataToSend[i]);
                characterDataReceiver.ReceiveMessage(msg); // send all the chunks. There is no need for a "We're finished" message since the receiver knows how many chunks to expect
                // Note that in networked environments, these info must be sent reliably (e.g., using TCP or related) because packet loss would result in the character never loading.
                // Note that it's also possible to send all data outside of the netcode via HTTPS or similar.
            }
        }
        else
        {
            Debug.LogWarning("File not found: " + filename);
        }
    }
}
