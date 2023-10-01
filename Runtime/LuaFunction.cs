#nullable enable
using MoonSharp.Interpreter;

namespace Popcron.LuaScript
{
    public readonly struct LuaFunction
    {
        public readonly string name;
        public readonly DynValue functionVal;
        public readonly int parameterCount;

        public LuaFunction(string name, DynValue functionVal, int parameterCount)
        {
            this.name = name;
            this.functionVal = functionVal;
            this.parameterCount = parameterCount;
        }
    }
}