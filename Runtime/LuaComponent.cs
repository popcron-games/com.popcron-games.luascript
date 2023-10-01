#nullable enable
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Popcron.LuaScript
{
    public class LuaComponent : MonoBehaviour
    {
        public const string ModePropertyName = nameof(mode);
        public const string TextPropertyName = nameof(text);
        public const string AssetPropertyName = nameof(asset);

        [SerializeField]
        private SourceMode mode;

        [SerializeField, TextArea(3, 15)]
        private string text;

        [SerializeField]
        private LuaScriptAsset? asset;

        private List<SerializedUnityObject> serializedUnityObjects = new List<SerializedUnityObject>();
        private List<SerializedGenericData> serializedData = new List<SerializedGenericData>();
        private string? lastText;
        private bool shouldReloadBecauseLiteralTextChange;
        private float literalTextChangeTimer;

        public LuaScript? LuaScript { get; private set; }

        protected virtual void Awake()
        {
            LuaScript = CreateLuaScript(name);

            bool isEditor = !Application.isPlaying;
            if (LuaScript.TagToFuntions.TryGetValue(nameof(Awake), out HashSet<string>? functions))
            {
                foreach (string function in functions)
                {
                    if (LuaScript.HasTag(function, "EditorOnly"))
                    {
                        if (!isEditor)
                        {
                            continue;
                        }
                    }

                    LuaScript.Call(function);
                    break;
                }
            }
        }

        protected virtual void OnEnable()
        {
            if (LuaScript == null) LuaScript = CreateLuaScript(name);
            LoadSerializedVariables(LuaScript);

            bool isEditor = !Application.isPlaying;
            if (LuaScript.TagToFuntions.TryGetValue(nameof(OnEnable), out HashSet<string>? functions))
            {
                foreach (string function in functions)
                {
                    if (LuaScript.HasTag(function, "EditorOnly"))
                    {
                        if (!isEditor)
                        {
                            continue;
                        }
                    }

                    LuaScript.Call(function);
                    break;
                }
            }
        }

        protected virtual void Start()
        {
            bool isEditor = !Application.isPlaying;
            if (LuaScript.TagToFuntions.TryGetValue(nameof(Start), out HashSet<string>? functions))
            {
                foreach (string function in functions)
                {
                    if (LuaScript.HasTag(function, "EditorOnly"))
                    {
                        if (!isEditor)
                        {
                            continue;
                        }
                    }

                    LuaScript.Call(function);
                    break;
                }
            }
        }

        protected virtual void OnDisable()
        {
            if (mode == SourceMode.Asset)
            {
                if (asset != null)
                {
                    asset.OnModified -= OnModified;
                }
            }

            if (LuaScript != null)
            {
                bool isEditor = !Application.isPlaying;
                if (LuaScript.TagToFuntions.TryGetValue(nameof(OnDisable), out HashSet<string>? functions))
                {
                    foreach (string function in functions)
                    {
                        if (LuaScript.HasTag(function, "EditorOnly"))
                        {
                            if (!isEditor)
                            {
                                continue;
                            }
                        }

                        LuaScript.Call(function);
                        break;
                    }
                }

                SaveSerializedVariables(LuaScript);
                LuaScript.Dispose();
                LuaScript = null;
            }
        }

        protected virtual void Update()
        {
            if (mode == SourceMode.Literal)
            {
                if (text != lastText)
                {
                    lastText = text;
                    if (Application.isPlaying)
                    {
                        literalTextChangeTimer = 1.4f;
                    }
                    else
                    {
                        literalTextChangeTimer = 0f;
                    }

                    shouldReloadBecauseLiteralTextChange = true;
                }

                if (literalTextChangeTimer <= 0 && shouldReloadBecauseLiteralTextChange)
                {
                    shouldReloadBecauseLiteralTextChange = false;
                    Reload();
                }
                else
                {
                    literalTextChangeTimer -= Time.deltaTime;
                }
            }

            if (LuaScript != null)
            {
                bool isEditor = !Application.isPlaying;
                if (LuaScript.TagToFuntions.TryGetValue(nameof(Update), out HashSet<string>? functions))
                {
                    foreach (string function in functions)
                    {
                        if (LuaScript.HasTag(function, "EditorOnly"))
                        {
                            if (!isEditor)
                            {
                                continue;
                            }
                        }

                        LuaScript.Call(function);
                        break;
                    }
                }
            }
        }

        protected virtual void FixedUpdate()
        {
            if (LuaScript != null)
            {
                bool isEditor = !Application.isPlaying;
                if (LuaScript.TagToFuntions.TryGetValue(nameof(FixedUpdate), out HashSet<string>? functions))
                {
                    foreach (string function in functions)
                    {
                        if (LuaScript.HasTag(function, "EditorOnly"))
                        {
                            if (!isEditor)
                            {
                                continue;
                            }
                        }

                        LuaScript.Call(function);
                        break;
                    }
                }
            }
        }

        public ReadOnlySpan<char> GetSourceCode()
        {
            if (mode == SourceMode.Asset)
            {
                if (asset == null)
                {
                    return ReadOnlySpan<char>.Empty;
                }

                return asset.SourceCode;
            }
            else if (mode == SourceMode.Literal)
            {
                return text.AsSpan();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        private void OnModified()
        {
            if (this != null)
            {
                Reload();
            }
        }

        private LuaScript CreateLuaScript(string name)
        {
            if (mode == SourceMode.Asset)
            {
                if (asset != null)
                {
                    asset.OnModified += OnModified;
                }
            }
            else
            {

            }

            ReadOnlySpan<char> sourceCode = GetSourceCode();
            LuaScript luaScript = new LuaScript(name, sourceCode);
            luaScript.SetObject("transform", transform);
            luaScript.SetObject("gameObject", gameObject);
            OnCreated(luaScript, sourceCode);
            return luaScript;
        }

        protected virtual void OnCreated(LuaScript luaScript, ReadOnlySpan<char> sourceCode)
        {

        }

        /// <summary>
        /// Reloads this component's <see cref="Popcron.LuaScript.LuaScript"/> instance.
        /// </summary>
        public virtual void Reload()
        {
            OnDisable();
            OnEnable();
        }

        private void LoadSerializedVariables(LuaScript luaScript)
        {
            foreach (SerializedUnityObject unityObject in serializedUnityObjects)
            {
                luaScript.SetObject(unityObject.name, unityObject.value);
            }

            serializedUnityObjects.Clear();

            foreach (SerializedGenericData data in serializedData)
            {
                luaScript.SetObject(data.name, data.Value);
            }

            serializedData.Clear();
        }

        private void SaveSerializedVariables(LuaScript luaScript)
        {
            serializedUnityObjects.Clear();
            serializedData.Clear();
            foreach (string variableName in GetVariablesWithTag(luaScript, "Serialize"))
            {
                string name = variableName.ToString();
                object? value = luaScript.GetObject(name);
                if (value is Object unityObject)
                {
                    if (unityObject != null)
                    {
                        serializedUnityObjects.Add(new SerializedUnityObject(name, unityObject));
                    }
                }
                else if (value != null)
                {
                    serializedData.Add(new SerializedGenericData(name, value));
                }
            }
        }

        /// <summary>
        /// Returns all variables that are marked for serialization with this format:
        /// <code>#Serialize {tag}</code>
        /// </summary>
        public static IEnumerable<string> GetVariablesWithTag(LuaScript script, string tag)
        {
            foreach (string existingTag in script.Tags)
            {
                if (existingTag.StartsWith(tag))
                {
                    int firstSpaceIndex = existingTag.IndexOf(' ');
                    if (firstSpaceIndex != -1)
                    {
                        ReadOnlySpan<char> remaining = existingTag.AsSpan().Slice(firstSpaceIndex + 1);
                        if (remaining.Length > 0)
                        {
                            yield return remaining.ToString();
                        }
                    }
                }
            }
        }

        public enum SourceMode
        {
            Asset,
            Literal
        }

        [Serializable]
        public class SerializedUnityObject
        {
            public string name;
            public Object value;

            public SerializedUnityObject(string name, Object value)
            {
                this.name = name;
                this.value = value;
            }
        }

        [Serializable]
        public class SerializedGenericData
        {
            private static readonly JsonSerializerSettings settings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

            public string name;
            public string aqn;
            public string json;

            public object? Value => JsonConvert.DeserializeObject(json, Type.GetType(aqn));

            public SerializedGenericData(string name, string aqn, string json)
            {
                this.name = name;
                this.aqn = aqn;
                this.json = json;
            }

            public SerializedGenericData(string name, object value)
            {
                this.name = name;
                aqn = value.GetType().AssemblyQualifiedName;
                json = JsonConvert.SerializeObject(value, Formatting.None, settings);
            }
        }
    }
}