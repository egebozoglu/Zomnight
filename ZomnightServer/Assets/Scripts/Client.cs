using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client
{
    public static int dataBufferSize = 4096;
    public int id;
    public TCP tcp;
    public UDP udp;
    public Player player;


    public Client(int clientID)
    {
        id = clientID;
        tcp = new TCP(id);
        udp = new UDP(id);
    }

    public class TCP
    {
        public TcpClient socket;
        private readonly int id;
        private NetworkStream stream;
        private byte[] receiveBuffer;
        private Packet receivedData;
        public TCP(int id)
        {
            this.id = id;
        }

        public void Connect(TcpClient socket)
        {
            this.socket = socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream();
            receivedData = new Packet();
            receiveBuffer = new byte[dataBufferSize];

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallBack, null);

            ServerSend.Welcome(id, "Welcome to Server..");
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Error Sending Data to Player {id} via TCP: {ex}..");
            }
        }

        private void ReceiveCallBack(IAsyncResult result)
        {
            try
            {
                int byteLength = stream.EndRead(result);
                if (byteLength <= 0)
                {
                    Server.clients[id].Disconnect();
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);

                receivedData.Reset(HandleData(data));

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallBack, null);
            }
            catch (Exception ex)
            {
                Debug.Log($"Error Receiving TCP data: " + ex);
                Server.clients[id].Disconnect();
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLenght = 0;

            receivedData.SetBytes(data);

            if (receivedData.UnreadLength() >= 4)
            {
                packetLenght = receivedData.ReadInt();
                if (packetLenght <= 0)
                {
                    return true;
                }
            }

            while (packetLenght > 0 && packetLenght <= receivedData.UnreadLength())
            {
                byte[] packetBytes = receivedData.ReadBytes(packetLenght);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        Server.packetHandlers[packetId](id, packet);
                    }
                });

                packetLenght = 0;
                if (receivedData.UnreadLength() >= 4)
                {
                    packetLenght = receivedData.ReadInt();
                    if (packetLenght <= 0)
                    {
                        return true;
                    }
                }
            }

            if (packetLenght <= 1)
            {
                return true;
            }

            return false;
        }

        public void Disconnect()
        {
            socket.Close();
            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public IPEndPoint endPoint;
        private int id;

        public UDP(int id)
        {
            this.id = id;
        }

        public void Connect(IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
        }

        public void SendData(Packet packet)
        {
            Server.SendUDPData(endPoint, packet);
        }

        public void HandleData(Packet packetData)
        {
            int packetLength = packetData.ReadInt();
            byte[] packetBytes = packetData.ReadBytes(packetLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(packetBytes))
                {
                    int packetId = packet.ReadInt();
                    Server.packetHandlers[packetId](id, packet);
                }
            });
        }

        public void Disconnect()
        {
            endPoint = null;
        }
    }

    //public void SendIntoGame(string userName)
    //{
    //    player = new Player(id, userName, new Vector3(0, 0, 0));

    //    foreach (Client client in Server.clients.Values)
    //    {
    //        if (client.player != null)
    //        {
    //            if (client.id != id)
    //            {
    //                ServerSend.SpawnPlayer(id, client.player);
    //            }
    //        }
    //    }

    //    foreach (Client client in Server.clients.Values)
    //    {
    //        if (client.player != null)
    //        {
    //            ServerSend.SpawnPlayer(client.id, player);
    //        }
    //    }
    //}

    public void Disconnect()
    {
        try
        {
            var user = ServerHandle.usersRoomed.Where(x => x.ID == id).FirstOrDefault();
            var users = ServerHandle.usersRoomed.Where(x => x.RoomID == user.RoomID).ToList();
            ServerHandle.usersRoomed.Remove(user);
            Debug.Log($"{user.UserName} was removed from his/her room.");
            var room = ServerHandle.rooms.Where(x => x.RoomID == user.RoomID).FirstOrDefault();
            room.PlayerCount = room.PlayerCount - 1;
            if (room.PlayerCount == 0)
            {
                ServerHandle.rooms.Remove(room);
                Debug.Log($"{user.RoomID} was shut down.");
            }

            users = ServerHandle.usersRoomed.Where(x => x.RoomID == user.RoomID).ToList();

            if (users.Count != 0)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    ServerSend.PlayerDestroy(id, users[i].ID, -1);
                    if (users.Count < 2)
                    {
                        ServerSend.RoomChanged(users[i].ID, users[0].UserName, string.Empty, string.Empty, users.Count);
                    }
                    else if (users.Count < 3)
                    {
                        ServerSend.RoomChanged(users[i].ID, users[0].UserName, users[1].UserName, string.Empty, users.Count);
                    }
                }
            }
        }
        catch
        {

        }

        var userConnected = ServerHandle.usersConnected.Where(x => x.ID == id).FirstOrDefault();
        ServerHandle.usersConnected.Remove(userConnected);

        Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has disconnected.");
        player = null;
        tcp.Disconnect();
        udp.Disconnect();
    }
}
