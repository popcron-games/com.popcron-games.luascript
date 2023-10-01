#nullable enable
using Popcron;
using Popcron.LuaScript;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[ExecuteAlways]
public class CustomLuaComponent : LuaComponent, IEventHandler
{
    public const string InitialUnityData = nameof(initialUnityData);
    public const string InitialGenericData = nameof(initialGenericData);

    [SerializeField]
    private List<SerializedUnityObject> initialUnityData = new List<SerializedUnityObject>();

    [SerializeField]
    private List<SerializedGenericData> initialGenericData = new List<SerializedGenericData>();

    private readonly HashSet<string> exposedVariables = new HashSet<string>();
    private readonly Dictionary<string, string> exposedVariableNameToType = new Dictionary<string, string>();

    public IEnumerable<(string type, string name)> ExposedVariables
    {
        get
        {
            foreach (string name in exposedVariables)
            {
                if (exposedVariableNameToType.TryGetValue(name, out string type))
                {
                    yield return (type, name);
                }
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Everything.Add(this);
    }

    protected override void OnDisable()
    {
        Everything.Remove(this);
        base.OnDisable();
    }

    public object? GetInitialData(ReadOnlySpan<char> key)
    {
        foreach (var item in initialUnityData)
        {
            if (key.SequenceEqual(item.name.AsSpan()))
            {
                return item.value;
            }
        }

        foreach (var item in initialGenericData)
        {
            if (key.SequenceEqual(item.name.AsSpan()))
            {
                return item.Value;
            }
        }

        return null;
    }

    public void SetInitialData(ReadOnlySpan<char> key, object? value)
    {
        if (value is Object unityObject)
        {
            for (int i = 0; i < initialUnityData.Count; i++)
            {
                SerializedUnityObject item = initialUnityData[i];
                if (key.SequenceEqual(item.name.AsSpan()))
                {
                    if (unityObject != null)
                    {
                        item.value = unityObject;
                        return;
                    }
                }
            }

            for (int i = initialUnityData.Count - 1; i >= 0; i--)
            {
                SerializedUnityObject item = initialUnityData[i];
                if (key.SequenceEqual(item.name.AsSpan()))
                {
                    initialUnityData.RemoveAt(i);
                }
            }

            if (unityObject != null)
            {
                initialUnityData.Add(new SerializedUnityObject(key.ToString(), unityObject));
            }
        }
        else
        {
            for (int i = 0; i < initialGenericData.Count; i++)
            {
                SerializedGenericData item = initialGenericData[i];
                if (key.SequenceEqual(item.name.AsSpan()))
                {
                    if (value is not null && value is not Object)
                    {
                        initialGenericData[i] = new SerializedGenericData(item.name, value);
                        return;
                    }
                }
            }

            for (int i = initialGenericData.Count - 1; i >= 0; i--)
            {
                SerializedGenericData item = initialGenericData[i];
                if (key.SequenceEqual(item.name.AsSpan()))
                {
                    initialUnityData.RemoveAt(i);
                }
            }
        
            if (value is not null)
            {
                initialGenericData.Add(new SerializedGenericData(key.ToString(), value));
            }
        }
    }

    private void FindExposedVariables(LuaScript script)
    {
        exposedVariables.Clear();
        exposedVariableNameToType.Clear();
        foreach (string fullTagLine in script.Tags)
        {
            if (fullTagLine.Contains("Exposed"))
            {
                int equalsSeparator = fullTagLine.LastIndexOf('=');
                if (equalsSeparator != -1)
                {
                    int lastSpaceIndex = fullTagLine.LastIndexOf(' ');
                    string variableName = fullTagLine.Substring(equalsSeparator + 1);
                    string variableType = fullTagLine.Substring(lastSpaceIndex + 1, equalsSeparator - lastSpaceIndex - 1);
                    exposedVariables.Add(variableName);
                    exposedVariableNameToType.Add(variableName, variableType);
                    script.SetObject(variableName, GetInitialData(variableName));
                }
                else
                {
                    Debug.LogErrorFormat(this, "Incomplete information for exposing a variable, format is #Exposed type=name");
                }
            }
        }
    }

    protected override void OnCreated(LuaScript luaScript, ReadOnlySpan<char> sourceCode)
    {
        base.OnCreated(luaScript, sourceCode);
        FindExposedVariables(luaScript);
    }

    public override void Reload()
    {
        base.Reload();
        FindExposedVariables(LuaScript!);
    }

    void IEventHandler.Dispatch<T>(T e)
    {
        if (e is ILuaFunctionCaller caller)
        {
            caller.InvokeFunction(LuaScript ?? throw new Exception("Lua script not available at this state"));
        }
        else
        {
            LuaScript?.CallWithTag(e.GetType().Name);
        }
    }
}