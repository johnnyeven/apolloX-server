using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Wooha.Objects
{
    class CGameObject
    {
        public String ObjectId;
        public String ResourceId;
        public int ObjectType;
        public int PosX = int.MinValue;
        public int PosY = int.MinValue;

        public Socket client;

        public CGameObject()
        {
        }
    }
}
