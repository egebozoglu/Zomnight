using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rooms
{
    public string RoomID;
    public int PlayerCount;
    public bool Available;
    public bool PublicRoom;
    public string RoomName;
    public string RoomPassword;
    public int ReadyCount;
    public Rooms()
    {

    }

    public Rooms(string roomID, int playerCount, bool available, bool publicRoom)
    {
        this.RoomID = roomID;
        this.PlayerCount = playerCount;
        this.Available = available;
        this.PublicRoom = publicRoom;
    }

    public Rooms(string roomID, int playerCount, bool available, bool publicRoom, string roomName, string roomPassword)
    {
        this.RoomID = roomID;
        this.PlayerCount = playerCount;
        this.Available = available;
        this.PublicRoom = publicRoom;
        this.RoomName = roomName;
        this.RoomPassword = roomPassword;
    }
}