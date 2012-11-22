using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TCPServer
{
    class Action
    {
		public static int STOP = 0;
		public static int SIT = 1;
		public static int MOVE = 2;
		public static int ATTACK = 6;
		public static int DIE = 10;
		public static int RELIVE = 13;

        public Action()
        {
        }
    }
}
