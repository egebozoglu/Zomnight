using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServerHandle
{
    public static List<Rooms> rooms = new List<Rooms>();
    public static List<Users> usersRoomed = new List<Users>();
    public static List<Users> usersConnected = new List<Users>();

    public static System.Random rand = new System.Random();

    public static string RandomString(int length)
    {
        string[] chars = { "A","B", "C","D","E","F","G","H","I","J","K","L","M","N","O","P",
            "Q","R","S","T","U","V","W","X","Y","Z","a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p",
            "r","s","t","u","v","w","x","y","z","0","1","2","3","4","5","6","7","8","9", ".", ";", ":", "!", "," };
        string randomString = string.Empty;
        for (int i = 0; i < length; i++)
        {
            var intRandom = rand.Next(0, length);
            randomString = randomString + chars[intRandom];
        }

        return randomString;

    }

    public static void WelcomeReceived(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        string userName = packet.ReadString();

        Debug.Log($"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient} as {userName}.");

        if (fromClient != clientIdCheck)
        {
            Debug.Log("IDs don't match each other.");
        }

        for (int i = 0; i < 5; i++)
        {
            ServerSend.SpawnBossZombies(fromClient, NetworkManager.instance.zombiePositions[i]);
        }

        for (int i = 5; i < NetworkManager.instance.zombiePositions.Count; i++)
        {
            ServerSend.SpawnZombies(fromClient, NetworkManager.instance.zombiePositions[i]);
        }

        Users user = new Users(fromClient, userName, "null");
        usersConnected.Add(user);
    }

    public static void StatsRequest(int fromClient, Packet packet)
    {
        string username = packet.ReadString();

        int activePlayers = usersConnected.Count;

        ServerSend.StatsRequestResult(fromClient, activePlayers);
    }

    public static void RoomCellRequest(int fromClient, Packet packet)
    {
        fromClient = packet.ReadInt();

        var availableRooms = rooms.Where(x => x.PublicRoom == false).Where(x => x.PlayerCount < 3).Where(x => x.Available == true).ToList();

        if (availableRooms.Count!=0)
        {
            for (int i = 0; i < availableRooms.Count; i++)
            {
                ServerSend.RoomCellRequestResult(fromClient, availableRooms[i].RoomID, availableRooms[i].RoomName, availableRooms[i].RoomPassword);
            }
        }
        else
        {
            Debug.Log("No Available Room");
        }
    }

    public static void PlayerMovement(int fromClient, Packet packet)
    {
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();
        string roomID = packet.ReadString();
        bool walking = packet.ReadBool();
        var users = usersRoomed.Where(x => x.RoomID == roomID).ToList();

        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].ID != fromClient)
            {
                ServerSend.PlayerMovement(fromClient, position, rotation, walking, users[i].ID);
            }
        }
    }

    public static void PlayRequest(int fromClient, Packet packet)
    {
        var userName = packet.ReadString();
        string roomID = RandomString(30);

        Debug.Log($"{fromClient} {userName} is looking for room...");
        if (rooms.Count != 0)
        {
            List<Rooms> availableRooms = new List<Rooms>();
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].PlayerCount < 2 && rooms[i].Available == true && rooms[i].PublicRoom == true)
                {
                    availableRooms.Add(rooms[i]);
                }
            }
            if (availableRooms.Count == 0)
            {
                Rooms room = new Rooms(roomID, 1, true, true);
                rooms.Add(room);
                Debug.Log($"{fromClient}: {userName} was inserted to room. Room ID: {roomID}.");
                Users user = new Users(fromClient, userName, roomID);
                usersRoomed.Add(user);
            }
            else
            {
                availableRooms[0].PlayerCount = availableRooms[0].PlayerCount + 1;
                Debug.Log($"{fromClient}: {userName} was inserted to room. Room ID: {availableRooms[0].RoomID}.");
                Users user = new Users(fromClient, userName, availableRooms[0].RoomID);
                usersRoomed.Add(user);
                if (availableRooms[0].PlayerCount == 2)
                {
                    float areaX = rand.Next(-40, 40);
                    float areaY = rand.Next(-40, 40);
                    var toSendGameList = usersRoomed.Where(x => x.RoomID == availableRooms[0].RoomID).ToList();
                    for (int i = 0; i < toSendGameList.Count; i++)
                    {
                        float randX = rand.Next(-3, 3);
                        float randY = rand.Next(-3, 3);
                        Player player = new Player(toSendGameList[i].ID, toSendGameList[i].UserName, new Vector3(randX, randY, 0f));
                        ServerSend.SpawnCircleArea(toSendGameList[i].ID, new Vector3(areaX, areaY, 0f));
                        for (int z = 0; z < toSendGameList.Count; z++)
                        {
                            ServerSend.SpawnPlayer(player, toSendGameList[z].ID);
                        }
                        ServerSend.PlayRequestResult(toSendGameList[i].ID, availableRooms[0].RoomID);
                    }

                    //for (int i = 0; i < NetworkManager.instance.zombiePositions.Count; i++)
                    //{
                    //    for (int p = 0; p < toSendGameList.Count; p++)
                    //    {
                    //        ServerSend.SpawnZombies(toSendGameList[p].ID, NetworkManager.instance.zombiePositions[i]);
                    //    }
                    //}

                    availableRooms[0].Available = false;
                }
            }
        }
        else
        {
            Rooms room = new Rooms(roomID, 1, true, true);
            rooms.Add(room);
            Debug.Log($"{fromClient}: {userName} was inserted to room. Room ID: {roomID}.");
            Users user = new Users(fromClient, userName, roomID);
            usersRoomed.Add(user);
        }
    }

    public static void PlayCoopRequest(int fromClient, Packet packet)
    {
        string roomID = packet.ReadString();

        var room = rooms.Where(x => x.RoomID == roomID).FirstOrDefault();
        room.Available = false;
        var users = usersRoomed.Where(x => x.RoomID == roomID).ToList();

        float areaX = rand.Next(-40, 40);
        float areaY = rand.Next(-40, 40);
        for (int i = 0; i < users.Count; i++)
        {
            
            float randX = rand.Next(-3, 3);
            float randY = rand.Next(-3, 3);
            Player player = new Player(users[i].ID, users[i].UserName, new Vector3(randX, randY, 0f));
            ServerSend.SpawnCircleArea(users[i].ID, new Vector3(areaX, areaY, 0f));
            for (int z = 0; z < users.Count; z++)
            {
                ServerSend.SpawnPlayer(player, users[z].ID);
            }
            ServerSend.PlayCoopRequestResult(users[i].ID, true);
        }

        //for (int i = 0; i < NetworkManager.instance.zombiePositions.Count; i++)
        //{
        //    for (int p = 0; p < users.Count; p++)
        //    {
        //        ServerSend.SpawnZombies(users[p].ID, NetworkManager.instance.zombiePositions[i]);
        //    }
        //}
    }

    public static void CreateCoopRoomRequest(int fromClient, Packet packet)
    {
        string username = packet.ReadString();
        string roomName = packet.ReadString();
        string roomPassword = packet.ReadString();

        string roomID = RandomString(30);

        Rooms room = new Rooms(roomID, 1, true, false, roomName, roomPassword);
        rooms.Add(room);
        Users user = new Users(fromClient, username, roomID);
        usersRoomed.Add(user);

        Debug.Log($"{fromClient}: {username} was inserted to room. Room ID: {roomID}.");

        ServerSend.CreateCoopRoomRequestResult(fromClient, roomID);
    }

    public static void JoinPrivateRoomRequest(int fromClient, Packet packet)
    {
        string roomID = packet.ReadString();
        string username = packet.ReadString();

        var room = rooms.Where(x => x.RoomID == roomID).FirstOrDefault();
        if (room!=null)
        {
            if (room.Available == true)
            {
                Users user = new Users(fromClient, username, roomID);
                usersRoomed.Add(user);
                var users = usersRoomed.Where(x => x.RoomID == roomID).ToList();
                room.PlayerCount = room.PlayerCount + 1;
                Debug.Log($"{username}, was inserted to Room: {roomID}");
                if (room.PlayerCount==3)
                {
                    room.Available = false;
                    for (int i = 0; i < users.Count; i++)
                    {
                        ServerSend.RoomChanged(users[i].ID, users[0].UserName, users[1].UserName, users[2].UserName, users.Count);
                    }
                }
                else if (room.PlayerCount == 2)
                {
                    for (int i = 0; i < users.Count; i++)
                    {
                        ServerSend.RoomChanged(users[i].ID, users[0].UserName, users[1].UserName, string.Empty, users.Count);
                    }
                }
                else
                {
                    for (int i = 0; i < users.Count; i++)
                    {
                        ServerSend.RoomChanged(users[i].ID, users[0].UserName, string.Empty, string.Empty, users.Count);
                    }
                }

                ServerSend.JoinPrivateRoomRequestResult(fromClient, roomID);
            }
            else
            {
                ServerSend.ErrorMessage(fromClient, "Room is not available.", "Oda müsait değil.");
            }
        }
        else
        {
            ServerSend.ErrorMessage(fromClient, "Room is not available.", "Oda müsait değil.");
        }
    }

    public static void CancelRandomRequest(int fromClient, Packet packet)
    {
        string username = packet.ReadString();
        var user = usersRoomed.Where(x => x.ID == fromClient).FirstOrDefault();
        usersRoomed.Remove(user);
        Debug.Log($"{user.UserName} was removed from his/her room.");
        var room = rooms.Where(x => x.RoomID == user.RoomID).FirstOrDefault();
        room.PlayerCount = room.PlayerCount - 1;
        if (room.PlayerCount == 0)
        {
            rooms.Remove(room);
            Debug.Log($"{user.RoomID} was shut down.");
        }
    }

    public static void ExitRoomRequest(int fromClient, Packet packet)
    {
        string username = packet.ReadString();
        var user = usersRoomed.Where(x => x.ID == fromClient).FirstOrDefault();
        usersRoomed.Remove(user);
        Debug.Log($"{user.UserName} was removed from his/her room.");
        var room = rooms.Where(x => x.RoomID == user.RoomID).FirstOrDefault();
        room.PlayerCount = room.PlayerCount - 1;
        if (room.PlayerCount == 0)
        {
            rooms.Remove(room);
            Debug.Log($"{user.RoomID} was shut down.");
        }
        else
        {
            var users = usersRoomed.Where(x => x.RoomID == room.RoomID).ToList();
            for (int i = 0; i < users.Count; i++)
            {
                if (users.Count<2)
                {
                    ServerSend.RoomChanged(users[i].ID, users[0].UserName, string.Empty, string.Empty, users.Count);
                }
                else if (users.Count<3)
                {
                    ServerSend.RoomChanged(users[i].ID, users[0].UserName, users[1].UserName, string.Empty, users.Count);
                }
                
            }
            room.Available = true;
        }
    }

    public static void RoomMessage(int fromClient, Packet packet)
    {
        string messageOwner = packet.ReadString();
        string messageContent = packet.ReadString();
        string roomID = packet.ReadString();

        var users = usersRoomed.Where(x => x.RoomID == roomID).ToList();

        for (int i = 0; i < users.Count; i++)
        {
            ServerSend.RoomMessage(users[i].ID, messageOwner, messageContent);
        }
    }

    public static void Fire(int fromClient, Packet packet)
    {
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();
        int bulletType = packet.ReadInt();
        string roomID = packet.ReadString();

        var users = usersRoomed.Where(x => x.RoomID == roomID).ToList();

        for (int i = 0; i < users.Count; i++)
        {
            if (users[i].ID != fromClient)
            {
                ServerSend.Fire(position, rotation, bulletType, users[i].ID, fromClient);
            }
        }
    }

    public static void Killed(int fromClient, Packet packet)
    {
        string username = packet.ReadString();
        string roomID = packet.ReadString();
        int killerID = packet.ReadInt();

        var users = usersRoomed.Where(x => x.RoomID == roomID).ToList();

        Debug.Log($"{users.Where(x => x.ID == fromClient).FirstOrDefault().UserName} was killed by player {killerID}.");
        var user = users.Where(x => x.ID == fromClient).FirstOrDefault();
        usersRoomed.Remove(user);
        var room = rooms.Where(x => x.RoomID == roomID).FirstOrDefault();
        room.PlayerCount = room.PlayerCount - 1;

        users = usersRoomed.Where(x => x.RoomID == roomID).ToList();

        if (users.Count!=0)
        {
            for (int i = 0; i < users.Count; i++)
            {
                ServerSend.PlayerDestroy(fromClient, users[i].ID, killerID);
            }
        }

        if (room.PlayerCount == 0)
        {
            rooms.Remove(room);
            Debug.Log($"{user.RoomID} was shut down.");
        }
    }

    public static void QuitGame(int fromClient, Packet packet)
    {
        string roomID = packet.ReadString();

        var users = usersRoomed.Where(x => x.RoomID == roomID).ToList();
        
        Debug.Log($"{users.Where(x => x.ID == fromClient).FirstOrDefault().UserName} left the match.");
        var user = users.Where(x => x.ID == fromClient).FirstOrDefault();
        usersRoomed.Remove(user);
        var room = rooms.Where(x => x.RoomID == roomID).FirstOrDefault();
        room.PlayerCount = room.PlayerCount - 1;

        users = usersRoomed.Where(x => x.RoomID == roomID).ToList();

        if (users.Count != 0)
        {
            for (int i = 0; i < users.Count; i++)
            {
                ServerSend.PlayerDestroy(fromClient, users[i].ID, -1);
            }
        }
    }


}
