using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* A mock version of "Messages" as they are used in networking. Used here to show in priniciple how character data can be sent within networking solutions.
 * Alternatively, one can also send the entire byte[] array outside of the netcode, e.g., via HTTPS which is likewise secure and likewise does not block the main thread. 
 * Using an established network may just be more convenient in some situations. 
 * by Marius Rubo, 2023
 * */

namespace MockNetworking
{
    /* "Messages" are units of data that are transmitted between server and clients.
     * The concept can have different names in different networking environments. For instance, in Fish-Net they are called "Broadcasts".
     * */

    public class Message
    {
        public MessageType messageType;
        public int id; // differing meaning: for announcement, this tells us name of id of character; for dataChunk, this tells us id of chunk
        public byte[] data;

        public Message(MessageType messageType, int id, byte[] data)
        {
            this.messageType = messageType;
            this.id = id;
            this.data = data;
        }
    }

    /* It is often needed to specify in a message what kind of message this is so that the reader can handle it. In netcode such as Fish-Net,
     * one may define different messages or broadcasts (or, alternatively, RCPs). Alternatively, one can use the same message definition but use a specifier
     * within the message to highlight to the reader what sort of information is arriving.
     * */

    public enum MessageType
    {
        CharacterDataAnnouncement,
        CharacterDataChunk
    }

}

