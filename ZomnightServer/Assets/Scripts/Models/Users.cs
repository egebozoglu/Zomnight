using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Users
{
    public int ID;
    public string UserName;
    public string RoomID;

    public Users()
    {

    }

    public Users(int id, string userName, string roomID)
    {
        this.ID = id;
        this.UserName = userName;
        this.RoomID = roomID;
    }
}
