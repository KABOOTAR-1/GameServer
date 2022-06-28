using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GameServer
{
    class Player
    {
        public int id;
        public string UserName;

        public Vector3 position;
        public Quaternion rotation;

        public Player(int _id, string userName, Vector3 _position)
        {
            id = _id;
            UserName = userName;
            position = _position;
            rotation = Quaternion.Identity;
        }
    }
}
