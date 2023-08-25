#nullable enable
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Popcron.LuaScript
{
    [ScriptedImporter(2, "lua")]
    public class LuaScriptImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            LuaScriptAsset textAsset = ScriptableObject.CreateInstance<LuaScriptAsset>();
            SerializedObject serializedObject = new(textAsset);
            serializedObject.FindProperty("text").stringValue = File.ReadAllText(ctx.assetPath);
            serializedObject.ApplyModifiedProperties();
            ctx.AddObjectToAsset("main", textAsset);
            ctx.SetMainObject(textAsset);
        }
    }
}