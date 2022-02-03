using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.tcp.SendData(packet);
    }

    private static void SendUDPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.udp.SendData(packet);
    }

    // Packets

    public static void WelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(Client.instance.myId);
            packet.Write(PlayerPrefs.GetString("Nickname"));

            SendTCPData(packet);
        }
    }

    public static void StatsRequest()
    {
        using(Packet packet = new Packet((int)ClientPackets.statsRequest))
        {
            packet.Write(PlayerPrefs.GetString("Nickname"));

            SendTCPData(packet);
        }
    }

    public static void RoomCellRequest()
    {
        using(Packet packet = new Packet((int)ClientPackets.roomCellRequest))
        {
            packet.Write(Client.instance.myId);

            SendTCPData(packet);
        }
    }

    public static void PlayRequest()
    {
        using (Packet packet = new Packet((int)ClientPackets.playRequest))
        {
            packet.Write(PlayerPrefs.GetString("Nickname"));

            SendTCPData(packet);
        }
    }

    public static void PlayCoopRequest()
    {
        using(Packet packet = new Packet((int)ClientPackets.playCoopRequest))
        {
            packet.Write(GameManager.instance.RoomID);

            SendTCPData(packet);
        }
    }

    public static void CreateCoopRoomRequest()
    {
        using(Packet packet = new Packet((int)ClientPackets.createCoopRoomRequest))
        {
            packet.Write(PlayerPrefs.GetString("Nickname"));
            packet.Write(UIManager.instance.roomNameInputField.text);
            packet.Write(UIManager.instance.roomPasswordInputField.text);

            SendTCPData(packet);
        }
    }

    public static void JoinPrivateRoomRequest()
    {
        using(Packet packet = new Packet((int)ClientPackets.joinPrivateRoomRequest))
        {
            packet.Write(UIManager.instance.joinRoomRoomIDText.text);
            packet.Write(PlayerPrefs.GetString("Nickname"));

            SendTCPData(packet);
        }
    }

    public static void CancelRandomRequest()
    {
        using(Packet packet = new Packet((int)ClientPackets.cancelRandomRequest))
        {
            packet.Write(PlayerPrefs.GetString("Nickname"));

            SendTCPData(packet);
        }
    }

    public static void ExitRoomRequest()
    {
        using(Packet packet = new Packet((int)ClientPackets.exitRoomRequest))
        {
            packet.Write(PlayerPrefs.GetString("Nickname"));
            GameManager.instance.roomedCount = 0;

            SendTCPData(packet);
        }
    }

    public static void PlayerMovement()
    {
        using (Packet packet = new Packet((int)ClientPackets.playerMovement))
        {
            try
            {
                packet.Write(GameManager.players[Client.instance.myId].transform.position);
                packet.Write(GameManager.players[Client.instance.myId].transform.rotation);
                packet.Write(GameManager.instance.RoomID);
                packet.Write(CharController.instance.walking);

                SendUDPData(packet);
            }
            catch
            {

            }
            
        }
    }

    public static void RoomMessage(string messageContent)
    {
        using(Packet packet = new Packet((int)ClientPackets.roomMessage))
        {
            packet.Write(PlayerPrefs.GetString("Nickname"));
            packet.Write(messageContent);
            packet.Write(GameManager.instance.RoomID);

            SendTCPData(packet);
        }
    }

    public static void Fire(Vector3 bulletPosition, Quaternion bulletRotation, int bulletType)
    {
        using (Packet packet = new Packet((int)ClientPackets.fire))
        {
            packet.Write(bulletPosition);
            packet.Write(bulletRotation);
            packet.Write(bulletType);
            packet.Write(GameManager.instance.RoomID);

            SendUDPData(packet);
        }
    }

    public static void Killed()
    {
        using(Packet packet = new Packet((int)ClientPackets.killed))
        {
            packet.Write(PlayerPrefs.GetString("Nickname"));
            packet.Write(GameManager.instance.RoomID);
            packet.Write(CharController.instance.killerID);

            SendUDPData(packet);
        }
    }

    public static void QuitGame()
    {
        using (Packet packet = new Packet((int)ClientPackets.quitGame))
        {
            packet.Write(GameManager.instance.RoomID);
            SendUDPData(packet);
            Debug.Log("Quitting From Match..");
        }
    }
}
