#nullable enable
using System;
using System.Collections.Generic;

namespace Popcron.LuaScript
{
    public static class LuaScriptAPI
    {
        private static readonly HashSet<string> functionNames = new HashSet<string>();
        private static readonly Dictionary<string, object> namesToDelegates = new Dictionary<string, object>();

        static LuaScriptAPI()
        {
            LoadUnityConfiguration();
        }

        private static void LoadUnityConfiguration()
        {

        }

        public static void AddFunction(string name, Action callback)
        {
            if (functionNames.Add(name))
            {
                namesToDelegates.Add(name, callback);
            }
        }

        public static void AddFunction(string name, Func<object?, object?> function)
        {
            if (functionNames.Add(name))
            {
                namesToDelegates.Add(name, function);
            }
        }

        public static void AddFunction(string name, Func<object?, object?, object?> function)
        {
            if (functionNames.Add(name))
            {
                namesToDelegates.Add(name, function);
            }
        }

        public static void AddFunction(string name, Func<object?, object?, object?, object?> function)
        {
            if (functionNames.Add(name))
            {
                namesToDelegates.Add(name, function);
            }
        }

        public static void AddFunction(string name, Func<object?, object?, object?, object?, object?> function)
        {
            if (functionNames.Add(name))
            {
                namesToDelegates.Add(name, function);
            }
        }

        public static void RegisterFunctions(LuaScript luaScript)
        {
            foreach (string name in functionNames)
            {
                luaScript.SetObject(name, namesToDelegates[name]);
            }
        }
    }
}