#nullable enable
using NLua;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Popcron.LuaScript
{
    public class LuaScript : IDisposable
    {
        private static readonly StringBuilder sourceCodeBuilder = new StringBuilder();
        private static readonly List<(string, int)> functionDeclarations = new List<(string, int)>();

        private readonly string name;
        private readonly HashSet<string> tags = new HashSet<string>();
        private readonly HashSet<string> functionNames = new HashSet<string>();
        private readonly Dictionary<(string name, int parameterCount), LuaFunction> functions = new Dictionary<(string name, int parameterCount), LuaFunction>();
        private readonly Dictionary<string, HashSet<string>> functionToTags = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> tagToFunctions = new Dictionary<string, HashSet<string>>();

        public Lua Interpreter { get; }
        public ReadOnlySpan<char> Name => name.AsSpan();
        public IReadOnlyCollection<string> Tags => tags;
        public IReadOnlyCollection<string> Functions => functionNames;

        public LuaScript(string name, ReadOnlySpan<char> source, bool openLibs = true)
        {
            this.name = name;
            Interpreter = GetNLuaState(source, openLibs);
        }

        private Lua GetNLuaState(ReadOnlySpan<char> sourceCode, bool openLibs)
        {
            string sourceCodeText = sourceCode.ToString();
            sourceCodeText = sourceCodeText.Replace("\r\n", "\n");
            string lastLine = string.Empty;
            sourceCodeBuilder.Clear();
            functionDeclarations.Clear();
            foreach (ReadOnlySpan<char> line in sourceCodeText.Split('\n'))
            {
                int functionIndex = line.IndexOf("function ");
                int bracketsStartIndex = line.IndexOf("(");
                int bracketsEndIndex = line.IndexOf(")");
                if (functionIndex != -1 && bracketsStartIndex != -1 && bracketsEndIndex != -1)
                {
                    int length = bracketsStartIndex - functionIndex - 9;
                    string functionName = line.Slice(functionIndex + 9, length).ToString();
                    if (lastLine.StartsWith("#"))
                    {
                        ReadOnlySpan<char> lastLineTrimmed = lastLine.AsSpan().Slice(1);
                        foreach (ReadOnlySpan<char> split in lastLineTrimmed.ToString().Split(','))
                        {
                            string tagId = split.ToString();
                            if (!functionToTags.TryGetValue(functionName, out HashSet<string> tags))
                            {
                                tags = new HashSet<string>();
                                functionToTags.Add(functionName, tags);
                            }

                            if (!tagToFunctions.TryGetValue(tagId, out HashSet<string> functions))
                            {
                                functions = new HashSet<string>();
                                tagToFunctions.Add(tagId, functions);
                            }

                            tags.Add(tagId);
                            functions.Add(functionName);
                        }
                    }

                    int parameterCount = 0;
                    ReadOnlySpan<char> parameterText = line.Slice(bracketsStartIndex, bracketsEndIndex - bracketsStartIndex + 1);
                    int p = 0;
                    bool readingText = false;
                    while (p < parameterText.Length)
                    {
                        char parameterChar = parameterText[p];
                        if (char.IsLetter(parameterChar))
                        {
                            readingText = true;
                        }
                        else if (parameterChar == ' ' || parameterChar == ',' || parameterChar == ')')
                        {
                            if (readingText)
                            {
                                parameterCount++;
                                readingText = false;
                            }
                        }

                        p++;
                    }

                    functionDeclarations.Add((functionName, parameterCount));
                }
                else if (line.Length > 0 && line[0] == '#')
                {
                    tags.Add(line.Slice(1).ToString());
                    lastLine = line.ToString();
                    continue;
                }

                lastLine = line.ToString();
                if (!line.StartsWith("#"))
                {
                    sourceCodeBuilder.AppendLine(line.ToString());
                }
            }

            Lua state = new Lua(openLibs);
            state.DoString(sourceCodeBuilder.ToString());
            foreach ((string name, int parameterCount) function in functionDeclarations)
            {
                functions.Add(function, state.GetFunction(function.name));
                functionNames.Add(function.name);
            }

            functionDeclarations.Clear();
            return state;
        }

        public bool TryGetFunction(string functionName, int parameterCount, out LuaFunction function)
        {
            if (functions.TryGetValue((functionName, parameterCount), out function))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object? Call(string functionName)
        {
            if (TryGetFunction(functionName, 0, out LuaFunction function))
            {
                return function.Call();
            }
            else throw new NullReferenceException($"Function {functionName} without parameters was not found to call in {name}");
        }

        public object? Call(string functionName, object p1)
        {
            if (TryGetFunction(functionName, 1, out LuaFunction function))
            {
                return function.Call(p1);
            }
            else throw new NullReferenceException($"Function {functionName} with 1 parameters was not found to call in {name}");
        }

        public object? Call(string functionName, object p1, object p2)
        {
            if (TryGetFunction(functionName, 2, out LuaFunction function))
            {
                return function.Call(p1, p2);
            }
            else throw new NullReferenceException($"Function {functionName} with 2 parameters was not found to call in {name}");
        }

        public object? Call(string functionName, object p1, object p2, object p3)
        {
            if (TryGetFunction(functionName, 3, out LuaFunction function))
            {
                return function.Call(p1, p2, p3);
            }
            else throw new NullReferenceException($"Function {functionName} with 3 parameters was not found to call in {name}");
        }

        public object? Call(string functionName, object p1, object p2, object p3, object p4)
        {
            if (TryGetFunction(functionName, 4, out LuaFunction function))
            {
                return function.Call(p1, p2, p3, p4);
            }
            else throw new NullReferenceException($"Function {functionName} with 4 parameters was not found to call in {name}");
        }

        public bool TryCall(string functionName)
        {
            if (TryGetFunction(functionName, 0, out LuaFunction function))
            {
                try
                {
                    function.Call();
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("Exception when trying to invoke {0}.{1}\n{2}", name, functionName, ex);
                }
            }

            return false;
        }

        public bool TryCall(string functionName, object p1)
        {
            if (TryGetFunction(functionName, 1, out LuaFunction function))
            {
                try
                {
                    function.Call(p1);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("Exception when trying to invoke {0}.{1}\n{2}", name, functionName, ex);
                }
            }

            return false;
        }

        public bool TryCall(string functionName, object p1, object p2)
        {
            if (TryGetFunction(functionName, 2, out LuaFunction function))
            {
                try
                {
                    function.Call(p1, p2);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("Exception when trying to invoke {0}.{1}\n{2}", name, functionName, ex);
                }
            }

            return false;
        }

        public bool TryCall(string functionName, object p1, object p2, object p3)
        {
            if (TryGetFunction(functionName, 3, out LuaFunction function))
            {
                try
                {
                    function.Call(p1, p2, p3);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("Exception when trying to invoke {0}.{1}\n{2}", name, functionName, ex);
                }
            }

            return false;
        }

        public bool TryCall(string functionName, object p1, object p2, object p3, object p4)
        {
            if (TryGetFunction(functionName, 4, out LuaFunction function))
            {
                try
                {
                    function.Call(p1, p2, p3, p4);
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("Exception when trying to invoke {0}.{1}\n{2}", name, functionName, ex);
                }
            }

            return false;
        }

        public object? GetObject(string fullPath)
        {
            return Interpreter[fullPath];
        }

        public void SetObject(string fullPath, object? obj)
        {
            Interpreter[fullPath] = obj;
        }

        public void RegisterFunction(string name, MethodInfo staticMethod)
        {
            Interpreter.RegisterFunction(name, staticMethod);
        }

        public void RegisterFunction(string name, object target, MethodInfo instanceMethod)
        {
            Interpreter.RegisterFunction(name, target, instanceMethod);
        }

        public void CallWithTag(string tag)
        {
            if (tagToFunctions.TryGetValue(tag, out HashSet<string> functions))
            {
                foreach (string functionId in functions)
                {
                    TryCall(functionId);
                }
            }
        }

        public void CallWithTag(string tag, object p1)
        {
            if (tagToFunctions.TryGetValue(tag, out HashSet<string> functions))
            {
                foreach (string functionId in functions)
                {
                    TryCall(functionId, p1);
                }
            }
        }

        public void CallWithTag(string tag, object p1, object p2)
        {
            if (tagToFunctions.TryGetValue(tag, out HashSet<string> functions))
            {
                foreach (string functionId in functions)
                {
                    TryCall(functionId, p1, p2);
                }
            }
        }

        public void CallWithTag(string tag, object p1, object p2, object p3)
        {
            if (tagToFunctions.TryGetValue(tag, out HashSet<string> functions))
            {
                foreach (string functionId in functions)
                {
                    TryCall(functionId, p1, p2, p3);
                }
            }
        }

        public void CallWithTag(string tag, object p1, object p2, object p3, object p4)
        {
            if (tagToFunctions.TryGetValue(tag, out HashSet<string> functions))
            {
                foreach (string functionId in functions)
                {
                    TryCall(functionId, p1, p2, p3, p4);
                }
            }
        }

        public void Dispose()
        {
            Interpreter.Dispose();
        }
    }
}