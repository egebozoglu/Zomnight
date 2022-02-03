using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
    private static void SendTCPData(int toClient, Packet packet)
    {
        packet.WriteLength();

        Server.clients[toClient].tcp.SendData(packet);
    }

    private static void SendUDPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        Server.clients[toClient].udp.SendData(packet);
    }

    private static void SendTCPDatatoAll(Packet packet)
    {
        packet.WriteLength();

        for (int i = 1; i < Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(packet);
        }
    }

    private static void SendTCPDatatoAll(int exceptClient, Packet packet)
    {
        packet.WriteLength();

        for (int i = 1; i < Server.MaxPlayers; i++)
        {
            if (i != exceptClient)
            {
                Server.clients[i].tcp.SendData(packet);
            }
        }
    }

    private static void SendUDPDataToAll(Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(packet);
        }
    }
    private static void SendUDPDataToAll(int exceptClient, Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != exceptClient)
            {
                Server.clients[i].udp.SendData(packet);
            }
        }
    }

    public static void Welcome(int toClient, string msg)
    {
        using (Packet packet = new Packet((int)ServerPackets.welcome))
        {
            packet.Write(msg);
            packet.Write(toClient);

            SendTCPData(toClient, packet);
        }
    }

    public static void StatsRequestResult(int toClient, int activePlayers)
    {
        using(Packet packet = new Packet((int)ServerPackets.statsRequestResult))
        {
            packet.Write(activePlayers);

            SendTCPData(toClient, packet);
        }
    }

    public static void RoomCellRequestResult(int toClient, string roomID, string roomName, string roomPassword)
    {
        using (Packet packet = new Packet((int)ServerPackets.roomCellRequestResult))
        {
            packet.Write(roomID);
            packet.Write(roomName);
            packet.Write(roomPassword);

            SendTCPData(toClient, packet);
        }
    }

    public static void PlayRequestResult(int toClient, string roomID)
    {
        using (Packet packet = new Packet((int)ServerPackets.playRequestResult))
        {
            packet.Write(roomID);


            SendTCPData(toClient, packet);
        }
    }

    public static void PlayCoopRequestResult(int toClient, bool started)
    {
        using(Packet packet = new Packet((int)ServerPackets.playCoopRequestResult))
        {
            packet.Write(started);

            SendTCPData(toClient, packet);
        }
    }

    public static void CreateCoopRoomRequestResult(int toClient, string roomID)
    {
        using(Packet packet = new Packet((int)ServerPackets.createCoopRoomRequestResult))
        {
            packet.Write(roomID);

            SendTCPData(toClient, packet);
        }
    }

    public static void JoinPrivateRoomRequestResult(int toClient, string roomID)
    {
        using(Packet packet = new Packet((int)ServerPackets.joinPrivateRoomRequestResult))
        {
            packet.Write(roomID);

            SendTCPData(toClient, packet);
        }
    }

    public static void RoomChanged(int toClient, string player1, string player2, string player3, int playerCount)
    {
        using(Packet packet = new Packet((int)ServerPackets.roomChanged))
        {
            packet.Write(player1);
            packet.Write(player2);
            packet.Write(player3);
            packet.Write(playerCount);

            SendTCPData(toClient, packet);
        }
    }

    public static void SpawnPlayer(Player player, int toClient)
    {
        using(Packet packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            packet.Write(player.id);
            packet.Write(player.userName);
            packet.Write(player.position);
            packet.Write(player.rotation);

            SendTCPData(toClient, packet);
        }
    }

    public static void SpawnZombies(int toClient, Vector3 position)
    {
        using(Packet packet = new Packet((int)ServerPackets.spawnZombies))
        {
            packet.Write(position);

            SendTCPData(toClient, packet);
        }
    }

    public static void SpawnBossZombies(int toClient, Vector3 position)
    {
        using (Packet packet = new Packet((int)ServerPackets.spawnBossZombies))
        {
            packet.Write(position);

            SendTCPData(toClient, packet);
        }
    }

    public static void SpawnCircleArea(int toClient, Vector3 position)
    {
        using(Packet packet = new Packet((int)ServerPackets.spawnCircleArea))
        {
            packet.Write(position);

            SendTCPData(toClient, packet);
        }
    }

    public static void PlayerMovement(int id, Vector3 position, Quaternion rotation, bool walking, int toClient)
    {
        using(Packet packet = new Packet((int)ServerPackets.playerMovement))
        {
            packet.Write(id);
            packet.Write(position);
            packet.Write(rotation);
            packet.Write(walking);

            SendTCPData(toClient, packet);
        }
    }

    public static void RoomMessage(int toClient, string messageOwner, string messageContent)
    {
        using(Packet packet = new Packet((int)ServerPackets.roomMessage))
        {
            packet.Write(messageOwner);
            packet.Write(messageContent);

            SendTCPData(toClient, packet);
        }
    }

    public static void ErrorMessage(int toClient, string messageEng, string messageTr)
    {
        using(Packet packet = new Packet((int)ServerPackets.errorMessage))
        {
            packet.Write(messageEng);
            packet.Write(messageTr);

            SendTCPData(toClient, packet);
        }
    }

    public static void Fire(Vector3 position, Quaternion rotation, int bulletType, int toClient, int bulletOwner)
    {
        using (Packet packet = new Packet((int)ServerPackets.fire))
        {
            packet.Write(position);
            packet.Write(rotation);
            packet.Write(bulletType);
            packet.Write(bulletOwner);

            SendTCPData(toClient, packet);
        }
    }

    public static void PlayerDestroy(int id, int toClient, int killerID)
    {
        using (Packet packet = new Packet((int)ServerPackets.playerDestroy))
        {

            packet.Write(id);
            packet.Write(killerID);

            SendTCPData(toClient, packet);
        }
    }
}