using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace GameServer
{
    class CServerPackage
    {
        public int success;
        public int controller;
        public int action;
        public ArrayList param;

        public CServerPackage()
        {
            param = new ArrayList();
        }
    }
}
