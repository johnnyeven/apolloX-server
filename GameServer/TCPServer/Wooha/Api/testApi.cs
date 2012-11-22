using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WPFLuaFramework;

namespace GameServer
{
    class testApi
    {
        [LuaFunction("WriteLine")]
        public void WriteLine(String str)
        {
            Console.WriteLine(str);
        }
    }
}
