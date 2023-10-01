#nullable enable
using MoonSharp.Interpreter;
using Popcron.MoonSharpProxies;
using UnityEngine;

namespace Popcron.LuaScript
{
    public static class LoadProxies
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#endif
        private static void Initialize()
        {
            UserData.RegisterProxyType<GameObjectProxy, GameObject>(x => new GameObjectProxy(x));
            UserData.RegisterProxyType<TransformProxy, Transform>(x => new TransformProxy(x));
            UserData.RegisterProxyType<ComponentProxy, Component>(x => new ComponentProxy(x));
            UserData.RegisterProxyType<BehaviourProxy, Behaviour>(x => new BehaviourProxy(x));
            UserData.RegisterProxyType<MonoBehaviourProxy, MonoBehaviour>(x => new MonoBehaviourProxy(x));
            UserData.RegisterProxyType<ScriptableObjectProxy, ScriptableObject>(x => new ScriptableObjectProxy(x));
            UserData.RegisterProxyType<ObjectProxy, Object>(x => new ObjectProxy(x));
            UserData.RegisterProxyType<MaterialPropertyBlockProxy, MaterialPropertyBlock>(x => new MaterialPropertyBlockProxy(x));

            UserData.RegisterType<Color>();
            UserData.RegisterType<Quaternion>();
            UserData.RegisterType<Vector2>();
            UserData.RegisterType<Vector3>();
            UserData.RegisterType<Vector4>();
            UserData.RegisterType<Rect>();
            UserData.RegisterType<Bounds>();
        }
    }
}