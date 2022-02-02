using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMessages
{
    public string MessageOwner { get; set; }
    public string MessageContent { get; set; }

    public RoomMessages(string messageOwner, string messageContent)
    {
        MessageContent = messageContent;
        MessageOwner = messageOwner;
    }

}
