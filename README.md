![Alt text](image.png)
# LuaScript
Provides an interface to interact with Lua as a package, with some extra extensions:
- [LuaScript](https://github.com/popcron-games/com.popcron-games.luascript#luascript)
- [LuaComponent](https://github.com/popcron-games/com.popcron-games.luascript#luacomponent)
- [Tags](https://github.com/popcron-games/com.popcron-games.luascript#tags)

### Dependencies
- Includes [moonsharp](https://github.com/moonsharp-devs/moonsharp) for Unity
- Depends on `com.unity.nuget.newtonsoft-json` package at least version 1.0.0

## LuaScript
The main script that encapsulates moonsharp's state object provides these:
- `TryGetFunction(name)` to get a function with the given name if it exists
- `TryCall(name)` to try and call a function that may or may not exist
- `GetObject(name)` to get a variable from Lua's global table in this script
- `SetObject(name, value)` to add a value to the global table
- `RegisterFunction(name, method)` to add a function to this script
- `CallWithTag(tag)` to call all functions with this tag
- `Dispose()` to dispose the state object, should mirror the creation event (OnEnable/OnDisable, Awake/OnDestroy, etc)

### Example
Performing addition through a static C# method:
```cs
int a = 1;
int b = 4;
string sourceCode = "function abacus(a, b) return mutate(a, b) end";
LuaScript newScript = new LuaScript("scriptName", sourceCode);
RegisterFunctions(newScript);
Debug.Log(newScript.Call("abacus", a, b)); //advanced 1+1

public static void RegisterFunctions(LuaScript script)
{
    script.RegisterFunction("mutate", typeof(API).GetMethod(nameof(Mutate)));
}

public static class API
{
    public static int Mutate(int a, int b) 
    {
        return a + b;
    }
}
```

## Tags
Tags are an added feature available when scripting and to help setup custom APIs
1. Using tags on functions:
    ```lua
    #UpdateEvent
    function thisWillBeCalledEveryFrame()
        -- do something
    end
    ```
    Good for abstracting away from the function name
    ```cs
    TextAsset asset = Resources.Load<TextAsset>("MyScript");
    LuaScript script = new(asset.name, asset.text);
    script.CallWithTag("UpdateEvent");
    ```
2. Using tags at the top to attach tags to the script itself:
    ```lua
    #IsStaticSystem
    #Tags=CouldBe:AnyFormat()

    #GameStarted
    function start()
        print("game has begun")
    end
    ```
    Can be used to categorize loaded scripts at runtime
    ```cs
    LuaScript script = ...
    if (script.Tags.Contains("IsStaticSystem"))
    {
        Game.AddStaticSystem(script);
    }
    else if (script.Tags.Contains("AttachToPlayer"))
    {
        scriptsToGivePlayers.Add(script);
    }
    ```

## LuaComponent
![custom component](https://media.discordapp.net/attachments/860402958692122645/1144694349171531826/image.png)

- A basic component that will call functions with these tags:
  - Awake
  - Start
  - OnEnable
  - OnDisable
  - no Update or FixedUpdate
- will have `transform` and `gameObject` accessible from scripts
- will automatically reload itself when its asset changes in editor or literal text changes in either editor or build

### Example
To extend functionality, inherit from `LuaComponent` and override `OnCreated()` to register your functions:
```cs
public class CustomLuaComponent : LuaComponent
{
    protected override void OnCreated()
    {
        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
        MethodInfo method = typeof(MyClass).GetMethod(nameof(Print), flags);
        script.AddFunction("print", Print);
    }

    private void Print(object? message)
    {
        Debug.Log("lua> " + message, this);
    }
}
```
```lua
function start()
    print("hello " .. "world")
end
```

## Included sample
The included sample has a `CustomLuaComponent` to show how the base `LuaComponent` can be extended, in this case to implement a new feature for exposing values to the inspector. Its included as a sample rather than extra content because its implementation is not that performant on low end devices for realtime use, but would work well for event based uses.
```lua
#Exposed color=playerColor
#Exposed decimal=playerSpeed
#Exposed integer=playerHealth

#Awake
function loadColor()
    --do something here
end
```

Types that are supported for this component
* color
* decimal
* integer
* boolean
* text
* vector2
* vector3
* asset
