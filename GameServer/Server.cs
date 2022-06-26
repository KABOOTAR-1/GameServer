using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    class Server
    {
        public static int maxPlayers { get; private set; }
        public static int Port { get; private set; }
        private static TcpListener Tcplistener;
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public delegate void PacketHandler(int _fromCLient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;
        public static void Start(int _maxplayers, int _port)
        {
            maxPlayers = _maxplayers;
            Port = _port;
            Console.WriteLine("Starting Server");
            InitilizeServerData();
            Tcplistener = new TcpListener(IPAddress.Any, Port);
            Tcplistener.Start();
            Tcplistener.BeginAcceptTcpClient(new AsyncCallback(TPconnectCallback), null);
            Console.WriteLine($"Server started on {Port}.");
        }

        private static void TPconnectCallback(IAsyncResult _result)
        {
            TcpClient _client = Tcplistener.EndAcceptTcpClient(_result);
            Tcplistener.BeginAcceptTcpClient(new AsyncCallback(TPconnectCallback), null);
            Console.WriteLine($"Incoming Connection from {_client.Client.RemoteEndPoint}");

            for(int i=1; i <= maxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            Console.WriteLine($" {_client.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        private static void InitilizeServerData()
        {
            for(int i = 1; i <= maxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers=new Dictionary<int, PacketHandler>() {
                { 
                    (int)ClientPackets.welcomeReceived,ServerHandle.WelcomeReceived }
                };

            Console.WriteLine("Initilaized Packets");
        }
    }
}
