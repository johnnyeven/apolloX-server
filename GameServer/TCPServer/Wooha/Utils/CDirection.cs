using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wooha.Utils
{
    class CDirection
    {
		public static int DOWN = 0;
		public static int RIGHT = 2;
		public static int TOP = 3;
		public static int LEFT = 1;
		public static int LEFT_DOWN = 4;
		public static int RIGHT_DOWN = 5;
		public static int LEFT_TOP = 6;
		public static int RIGHT_TOP = 7;

        public static int RandomDirection()
        {
            Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
            return rand.Next(8);
        }
    }
}
