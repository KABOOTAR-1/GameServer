using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
   class ServerSend
    {
        private static void SendTCPData(int _id,Packet _packet)
        {
            _packet.WriteLength();
            Server.clients[_id].tcp.SendData(_packet);

        }

        private static void SendTCPToAll(Packet _packets)
        {
            _packets.WriteLength();
            for(int i = 1; i <= Server.maxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packets);
            }
        }

        private static void SendTCPToAll(int _exceptionclint,Packet _packets)
        {
            _packets.WriteLength();
            for (int i = 1; i <= Server.maxPlayers; i++)
            {
                if(i!=_exceptionclint)
                Server.clients[i].tcp.SendData(_packets);
            }
        }

        public static void Welcome(int _toclientid,string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toclientid);

                SendTCPData(_toclientid,_packet);
            }
        }
    }
}
