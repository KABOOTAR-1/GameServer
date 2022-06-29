  using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
   public  class GameLogic
    {

        public static bool IsRunning = false;
        public static void Update()
        {
            foreach(Client client in Server.clients.Values)
            {
                if (client.player != null)
                {
                    client.player.Update(); 
                }
            }
            ThreadManager.UpdateMain();
        }

        public static void MainThread()
        {
            Console.WriteLine($"Main Thread started .Running at {Constants.TICKS_PER_SEC} ticks per second");
            DateTime _nextLoop = DateTime.Now;

            while (IsRunning)
            {
                while (_nextLoop < DateTime.Now)
                {
                    GameLogic.Update();

                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if (_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop-DateTime.Now);
                    }
                }
            }
        }
    }
}
