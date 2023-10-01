#nullable enable
using Popcron.LuaScript;
public interface ILuaFunctionCaller
{
    void InvokeFunction(LuaScript script);
}