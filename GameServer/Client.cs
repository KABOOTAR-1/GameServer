using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace GameServer
{
    class Client
    {
        public static int dataBufferSize = 4096;
        public int id;
        public TCP tcp;
        public UDP udp;
        public Player player;
        public Client(int _id)
        {
            this.id = _id;
            tcp = new TCP(id);
            udp = new UDP(id);
        }

        public class TCP
        {
            public TcpClient socket;
            private readonly int id;
            private NetworkStream stream;
            private Packet receievedData;
            private byte[] receiveBuffer;

            public TCP(int _id)
            {
                this.id = _id;
            }

            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;
                receievedData = new Packet();
                stream = socket.GetStream();

                receiveBuffer = new byte[dataBufferSize];
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallBacks, null);
                ServerSend.Welcome(id, "Welcome to the server");
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($" Error sending the data to player{id} via TCP:{e}");
                }
            }

            private void ReceiveCallBacks(IAsyncResult _result)
            {
                try
                {
                    int byelength = stream.EndRead(_result);
                    if (byelength <= 0)
                    {
                        Server.clients[id].Disconnect();
                    }

                    byte[] _data = new byte[byelength];
                    Array.Copy(receiveBuffer, _data, byelength);

                    receievedData.Reset(HandleData(_data));
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallBacks, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receivng TCP data: {ex}");
                    Server.clients[id].Disconnect();
                }

            }

            private bool HandleData(byte[] _data)
            {
                int PacketLength = 0;
                receievedData.SetBytes(_data);

                if (receievedData.UnreadLength() >= 4)
                {
                    PacketLength = receievedData.ReadInt();
                    if (PacketLength <= 0)
                    {
                        return true;
                    }
                }

                while (PacketLength > 0 && PacketLength <= receievedData.UnreadLength())
                {
                    byte[] _packetBytes = receievedData.ReadBytes(PacketLength);
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetit = _packet.ReadInt();
                            Server.packetHandlers[_packetit](id, _packet);

                        }
                    });
                    PacketLength = 0;

                    if (receievedData.UnreadLength() >= 4)
                    {
                        PacketLength = receievedData.ReadInt();
                        if (PacketLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (PacketLength <= 1)
                    return true;


                return false;
            }

            public void Disconnect()
            {
                socket.Close();
                receievedData = null;
                receiveBuffer = null;
                stream = null;
                socket = null;
        }
        }


      
        public class UDP
        {
            public IPEndPoint endPoint;
            public int id;

            public UDP(int _id)
            {
                id = _id;
            }

            public void Connect(IPEndPoint _endpoint)
            {
                endPoint = _endpoint;
            }

            public void SendData(Packet _packet)
            {
                Server.SendUDPData(endPoint, _packet);
            }

            public void HandleData(Packet _packet)
            {
                int packetlength = _packet.ReadInt();
                byte[] packetData = _packet.ReadBytes(packetlength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(packetData))
                    {
                        int _packetId = _packet.ReadInt();
                        Server.packetHandlers[_packetId](id, _packet);
                    }
                });
            }

            public void Disconnect()
            {
                endPoint = null;
            }
        }
       

        public void SendIntoGame(string _playerName)
        {
            player = new Player(id, _playerName, new Vector3(0, 0, 0));

            foreach(Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    if (_client.id != id)
                    {
                        ServerSend.SpawnPlayer(id, _client.player);
                    }
                }
            }

            foreach(Client _client in Server.clients.Values)
            {
                if (_client.player != null)
                {
                    ServerSend.SpawnPlayer(_client.id,player);
                }
            }
        }

        private void Disconnect()
        {
            Console.WriteLine($"{tcp.socket.Client.RemoteEndPoint} has disconnected");
            player = null;
            tcp.Disconnect();
            udp.Disconnect();
        }
    }
    }

