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
        private float MoveSpeed = 5f / Constants.TICKS_PER_SEC;
        private bool[] input;
        public Player(int _id, string userName, Vector3 _position)
        {
            id = _id;
            UserName = userName;
            position = _position;
            rotation = Quaternion.Identity;
            input = new bool[4];
        }

        public void Update()
        {
            Vector2 _inputDirection = Vector2.Zero;

            if (input[0])           
                _inputDirection.Y +=1;            
            if (input[1])
                _inputDirection.Y -=1;
            if(input[2])
                _inputDirection.X +=1;
            if (input[3]) 
                _inputDirection.X -=1;

            Move(_inputDirection);
        }

        private void Move(Vector2 _inputDirection)
        {
            Vector3 forward = Vector3.Transform(new Vector3(0, 0, 1), rotation);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, new Vector3(0, 1, 0)));

            Vector3 _moveDirection = right * _inputDirection.X+forward*_inputDirection.Y;
            position += _moveDirection * MoveSpeed;

            ServerSend.PlayerPosition(this);
            ServerSend.PlayerRotation(this);
        }
        internal void SetInput(bool[] _inputs, Quaternion _rotation)
        {
            
            input = _inputs;
            rotation = _rotation;
        }
    }
}
