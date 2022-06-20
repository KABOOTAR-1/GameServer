using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
     class Client
    {
        public static int dataBufferSize = 4096;
        public int id;
        public TCP tcp;
       

        public Client(int _id)
        {
            this.id = _id;
            tcp = new TCP(id);
        }

        public class TCP
        {
            public TcpClient socket;
            private readonly int id;
            private NetworkStream stream;
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

                stream = socket.GetStream();      
                
                receiveBuffer= new byte[dataBufferSize];    
                stream.BeginRead(receiveBuffer, 0, dataBufferSize,ReceiveCallBacks,null);
            }

            private void ReceiveCallBacks(IAsyncResult _result)
            {
                try
                {
                    int byelength = stream.EndRead(_result);
                    if (byelength <= 0)
                    {
                        return;
                    }

                    byte[] _data=new byte[byelength];
                    Array.Copy(receiveBuffer, _data, byelength);
                    stream.BeginRead(receiveBuffer,0,dataBufferSize,ReceiveCallBacks,null); 
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Error receivng TCP data: {ex}");
                }

            }
        }
    }
}
