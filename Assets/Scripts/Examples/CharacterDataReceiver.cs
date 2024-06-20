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
using System.Linq;

/* Character data are received in small chunks, reassambled and applied to a character. This example shows in priniciple a use case where one
 * computer (typically the server) has character data stored on a disk and sends it to other computers (the clients) which then apply it. 
 * For a simpler version of applying new character data to a character, see "ApplyCharacterData" used in the example scene "ApplyCharacterDataSimple".
 * by Marius Rubo, 2023
 * */
public class CharacterDataReceiver : MonoBehaviour
{
    [Tooltip("The character gameobject. Ensure all its component (e.g., 'Body') are named as expected in CharacterReference")]
    [SerializeField] GameObject character;

    private Dictionary<int, byte[]> receivedChunks = new Dictionary<int, byte[]>();
    private int totalChunksExpected = 0;
    private int chunksReceived = 0;
    private int ReceivedCharacterID;

    /// <summary>
    /// Receives individual messages which contain an announcement of character data chunks or those chunks themselves. Initiates reassembling and applying of data when complete.
    /// </summary>
    /// <param name="msg">The message received from the server. In this case a message can contain an announcement or a character data chunk.</param>
    public async void ReceiveMessage(Message msg)
    {
        if (msg.messageType == MessageType.CharacterDataAnnouncement)
        {
            receivedChunks = new Dictionary<int, byte[]>();
            totalChunksExpected = SerializationUtility.DeserializeValue<int>(msg.data, DataFormat.Binary); // a simple int indicating the expected number of chunks, stored as byte[] in the message
            chunksReceived = 0;
            ReceivedCharacterID = msg.id;
        }

        if (msg.messageType == MessageType.CharacterDataChunk)
        {
            receivedChunks[msg.id] = msg.data; // place the chunk into the dictionary
            chunksReceived++;

            if (chunksReceived == totalChunksExpected) 
            {
                List<byte[]> orderedChunks = receivedChunks.Values.ToList(); // when all chunks have arrived, assemble the data back into their original byte[] array
                byte[] data = DataChunker.ReassembleData(orderedChunks);
                //Debug.Log("Received data of character  id " + ReceivedCharacterID);

                data = await Gzip.DecompressAsync(data); // decompress as data were compressed in the storage process
                CharacterData characterData = new CharacterData();
                await Task.Run(() => characterData = SerializationUtility.DeserializeValue<CharacterData>(data, DataFormat.Binary)); // deserialize into CharacterData
                CharacterReference characterRef = new CharacterReference(character); // get the relevant references from the character (all names must be correct!)
                await CharacterData.ApplyCharacterDataToCharacter(characterData, characterRef); // apply to character step-by-step
            }
        }
    }
}
