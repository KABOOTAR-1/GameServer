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
        public static void Start(int _maxplayers,int _port)
        {
            maxPlayers = _maxplayers;
            Port = _port;
            Tcplistener = new TcpListener(IPAddress.Any,Port);
        }


    }
}
