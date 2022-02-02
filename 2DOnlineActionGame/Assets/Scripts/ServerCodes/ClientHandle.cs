using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int id = packet.ReadInt();

        Debug.Log($"Message Received from Server: {msg}.");
        Client.instance.myId = id;
        UIManager.instance.connectionStatus.text = "Connection: Active";
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void StatsRequestResult(Packet packet)
    {
        int activePlayers = packet.ReadInt();

        UIManager.instance.InstantiateError($"Active Players: {activePlayers}");
    }

    public static void RoomCellRequestResult(Packet packet)
    {
        string roomID = packet.ReadString();
        string roomName = packet.ReadString();
        string roomPassword = packet.ReadString();

        GameObject roomCell;

        roomCell = Instantiate(UIManager.instance.roomCellPrefab);
        roomCell.transform.SetParent(UIManager.instance.roomListContainer.transform, false);

        roomCell.GetComponent<RoomListManager>().roomID = roomID;
        roomCell.GetComponent<RoomListManager>().roomNameText.text = roomName;
        roomCell.GetComponent<RoomListManager>().roomPassword = roomPassword;

        UIManager.instance.instantiatedRoomCells.Add(roomCell);
        UIManager.instance.roomList.Add(roomName);
    }

    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        string userName = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();
        GameManager.instance.SpawnPlayer(id, userName, position, rotation);
        Debug.Log($"{id} {userName} spawned.");
    }

    public static void SpawnZombies(Packet packet)
    {
        Vector3 position = packet.ReadVector3();

        GameManager.instance.zombiePositions.Add(position);
        GameManager.instance.InstantiateZombie(position);
    }

    public static void SpawnBossZombies(Packet packet)
    {
        Vector3 position = packet.ReadVector3();

        GameManager.instance.zombiePositions.Add(position);
        GameManager.instance.InstantiateBossZombie(position);
    }

    public static void SpawnCircleArea(Packet packet)
    {
        Vector3 position = packet.ReadVector3();

        GameManager.instance.InstantiateCircleArea(position);
    }

    public static void PlayRequestResult(Packet packet)
    {
        Destroy(GameManager.instance.instantiatedBackgroundSound[0], 0f);
        string roomID = packet.ReadString();

        GameManager.instance.RoomID = roomID;

        PlayerPrefs.SetString("Energy", (int.Parse(PlayerPrefs.GetString("Energy")) - 1).ToString());

        Debug.Log($"My Room ID: {roomID}");

        UIManager.instance.menuScene.SetActive(true);
        UIManager.instance.joinRandomGameScene.SetActive(false);
        UIManager.instance.menuCanvas.SetActive(false);
        

        GameManager.instance.inOnlineGame = true;
        GameManager.instance.gameStarted = true;
        UIManager.instance.inGameCanvas.SetActive(true);
    }

    public static void PlayCoopRequestResult(Packet packet)
    {
        Destroy(GameManager.instance.instantiatedBackgroundSound[0], 0f);
        bool started = packet.ReadBool();

        PlayerPrefs.SetString("Energy", (int.Parse(PlayerPrefs.GetString("Energy")) - 1).ToString());

        Debug.Log("The game is started");

        GameManager.instance.userRoomed.Clear();

        UIManager.instance.userRoomed1.text = string.Empty;
        UIManager.instance.userRoomed2.text = string.Empty;
        UIManager.instance.userRoomed3.text = string.Empty;

        UIManager.instance.menuScene.SetActive(true);
        UIManager.instance.joinRandomGameScene.SetActive(false);
        UIManager.instance.roomScene.SetActive(false);
        UIManager.instance.menuCanvas.SetActive(false);

        foreach (GameObject cell in UIManager.instance.instantiatedMessageCells)
        {
            Destroy(cell, 0f);
        }

        UIManager.instance.instantiatedMessageCells.Clear();
        UIManager.instance.roomMessages.Clear();

        GameManager.instance.inOnlineGame = started;
        GameManager.instance.gameStarted = started;
        UIManager.instance.inGameCanvas.SetActive(true);
    }

    public static void CreateCoopRoomRequestResult(Packet packet)
    {
        string roomID = packet.ReadString();

        GameManager.instance.RoomID = roomID;
        GameManager.instance.roomedCount = 1;

        PlayerPrefs.SetString("Coin", (int.Parse(PlayerPrefs.GetString("Coin")) - 200).ToString());

        GameManager.instance.userRoomed.Add(PlayerPrefs.GetString("Nickname"));
        UIManager.instance.createRoomScene.SetActive(false);
        UIManager.instance.roomScene.SetActive(true);
        UIManager.instance.userRoomed1.text = GameManager.instance.userRoomed[0];

        Debug.Log($"My Room ID: {roomID}");
    }

    public static void JoinPrivateRoomRequestResult(Packet packet)
    {
        string roomID = packet.ReadString();

        GameManager.instance.RoomID = roomID;

        UIManager.instance.joinRoomScene.SetActive(false);
        UIManager.instance.roomScene.SetActive(true);

        Debug.Log($"My Room ID: {roomID}");
    }

    public static void RoomChanged(Packet packet)
    {
        string player1 = packet.ReadString();
        string player2 = packet.ReadString();
        string player3 = packet.ReadString();
        int playerCount = packet.ReadInt();

        GameManager.instance.roomedCount = playerCount;
        GameManager.instance.userRoomed.Clear();
        GameManager.instance.userRoomed.Add(player1);
        GameManager.instance.userRoomed.Add(player2);
        GameManager.instance.userRoomed.Add(player3);

        UIManager.instance.userRoomed1.text = GameManager.instance.userRoomed[0];
        UIManager.instance.userRoomed2.text = GameManager.instance.userRoomed[1];
        UIManager.instance.userRoomed3.text = GameManager.instance.userRoomed[2];
    }

    public static void RoomMessage(Packet packet)
    {
        string messageOwner = packet.ReadString();
        string messageContent = packet.ReadString();

        RoomMessages roomMessage = new RoomMessages(messageOwner, messageContent);
        UIManager.instance.roomMessages.Add(roomMessage);


        foreach (GameObject cell in UIManager.instance.instantiatedMessageCells)
        {
            Destroy(cell, 0f);
        }

        UIManager.instance.instantiatedMessageCells.Clear();

        for (int i = UIManager.instance.roomMessages.Count - 1; i > -1 ; i--)
        {
            GameObject messageCell;

            messageCell = Instantiate(UIManager.instance.messageCellPrefab);
            messageCell.transform.SetParent(UIManager.instance.messageListContainer.transform, false);

            messageCell.GetComponent<MessageListManager>().contentText.text = UIManager.instance.roomMessages[i].MessageOwner + ": " + UIManager.instance.roomMessages[i].MessageContent;

            UIManager.instance.instantiatedMessageCells.Add(messageCell);
        }
    }

    public static void PlayerMovement(Packet packet)
    {
        try
        {
            int id = packet.ReadInt();
            Vector3 position = packet.ReadVector3();
            Quaternion rotation = packet.ReadQuaternion();
            bool walking = packet.ReadBool();

            GameManager.players[id].gameObject.transform.position = position;
            GameManager.players[id].gameObject.transform.rotation = rotation;
        }
        catch
        {

        }
    }

    public static void ErrorMessage(Packet packet)
    {
        string messageEng = packet.ReadString();
        string messageTr = packet.ReadString();

        if (LocalizationController.instance.localLang == "English")
        {
            UIManager.instance.InstantiateError(messageEng);
        }
        else
        {
            UIManager.instance.InstantiateError(messageTr);
        }
    }

    public static void Fire(Packet packet)
    {
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();
        int bulletType = packet.ReadInt();
        int ownerID = packet.ReadInt();

        GameManager.instance.InstantiateBullet(bulletType, position, rotation, ownerID);
    }

    public static void PlayerDestroy(Packet packet)
    {
        int id = packet.ReadInt();
        int killerID = packet.ReadInt();

        PlayerStats playerStat = new PlayerStats(killerID);
        GameManager.instance.playerStats.Add(playerStat);

        GameManager.instance.DestroyPlayer(id);
    }
}
