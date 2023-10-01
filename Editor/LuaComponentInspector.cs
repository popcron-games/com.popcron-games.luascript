#nullable enable
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Popcron.LuaScript
{
    [CustomEditor(typeof(LuaComponent), true)]
    public class LuaComponentInspector : Editor
    {
        protected SerializedProperty mode;
        protected SerializedProperty text;
        protected SerializedProperty asset;
        private bool shouldReload;

        protected virtual void OnEnable()
        {
            mode = serializedObject.FindProperty(LuaComponent.ModePropertyName);
            text = serializedObject.FindProperty(LuaComponent.TextPropertyName);
            asset = serializedObject.FindProperty(LuaComponent.AssetPropertyName);
        }

        public override void OnInspectorGUI()
        {
            if (shouldReload)
            {
                shouldReload = false;
                ((LuaComponent)target).Reload();
            }

            serializedObject.Update();
            EditorGUILayout.PropertyField(mode);

            LuaComponent.SourceMode sourceMode = (LuaComponent.SourceMode)mode.enumValueIndex;
            if (sourceMode == LuaComponent.SourceMode.Literal)
            {
                EditorGUILayout.PropertyField(text);
            }
            else if (sourceMode == LuaComponent.SourceMode.Asset)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(asset);
                if (EditorGUI.EndChangeCheck())
                {
                    shouldReload = true;
                }
            }
            else
            {
                EditorGUILayout.LabelField("Unknown mode type");
            }

            LuaComponent component = (LuaComponent)target;
            if (component.LuaScript is LuaScript luaScript)
            {
                foreach (KeyValuePair<string, HashSet<string>> pair in luaScript.TagsOfFunctions)
                {
                    string name = pair.Key;
                    HashSet<string> tags = pair.Value;
                    if (tags.Contains("Button"))
                    {
                        if (GUILayout.Button(name))
                        {
                            luaScript.Call(name);
                        }
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
