using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using LuaInterface;

namespace WPFLuaFramework
{
    public class LuaFunction : Attribute
    {
        private String functionName;

        public LuaFunction(String strFuncName)
        {
            functionName = strFuncName;
        }

        public String getFunctionName()
        {
            return functionName;
        }
    }

    class LuaFramework
    {
        private Lua luaVM = new Lua();

        public void BindLuaAPIFunction(Object LuaAPIFuntion)
        {
            foreach (MethodInfo mInfo in LuaAPIFuntion.GetType().GetMethods())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(mInfo))
                {
                    if (attr.GetType() == typeof(LuaFunction))
                    {
                        String LuaFunctionName = (attr as LuaFunction).getFunctionName();
                        luaVM.RegisterFunction(LuaFunctionName, LuaAPIFuntion, mInfo);
                    }
                }
            }
        }

        public void ExecuteFile(String luaFileName)
        {
            try
            {
                luaVM.DoFile(luaFileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void ExecuteString(String luaCommand)
        {
            try
            {
                luaVM.DoString(luaCommand);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
