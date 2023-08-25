#nullable enable
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Popcron.LuaScript
{
    public static class LuaScriptAssetChangesChecker
    {
        private static readonly Dictionary<LuaScriptAsset, FileSystemWatcher> watchers = new Dictionary<LuaScriptAsset, FileSystemWatcher>();
        private static readonly ConcurrentStack<string> modifiedAssetsPaths = new ConcurrentStack<string>();
        private static bool initialized;

        [InitializeOnLoadMethod]
        private static void TryInitialize()
        {
            if (!initialized)
            {
                initialized = true;
                EditorApplication.update += OnUpdate;
            }
        }

        private static void OnUpdate()
        {
            foreach (LuaScriptAsset script in LuaScriptAsset.All)
            {
                if (!watchers.ContainsKey(script))
                {
                    string assetPath = AssetDatabase.GetAssetPath(script);
                    if (string.IsNullOrEmpty(assetPath)) continue;

                    string directoryPath = Directory.GetParent(assetPath).FullName;
                    FileSystemWatcher newWatcher = new(directoryPath, script.name + ".lua");
                    newWatcher.EnableRaisingEvents = true;
                    newWatcher.Changed += Changed;
                    watchers.Add(script, newWatcher);
                }
            }

            if (modifiedAssetsPaths.Count > 0)
            {
                AssetDatabase.Refresh();
                string rootPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Assets");
                while (modifiedAssetsPaths.TryPop(out string fullPath))
                {
                    string assetPath = fullPath.Replace(rootPath, "Assets");
                    if (AssetDatabase.LoadAssetAtPath<LuaScriptAsset>(assetPath) is LuaScriptAsset asset)
                    {
                        asset.OnModified?.Invoke();
                    }
                }
            }
        }

        private static void Changed(object sender, FileSystemEventArgs e)
        {
            modifiedAssetsPaths.Push(e.FullPath);
        }
    }
}
