using UnityEditor;

namespace Popcron.LuaScript
{
    [CustomEditor(typeof(LuaComponent), true)]
    public class LuaComponentInspector : Editor
    {
        protected SerializedProperty mode;
        protected SerializedProperty text;
        protected SerializedProperty asset;

        protected virtual void OnEnable()
        {
            mode = serializedObject.FindProperty(LuaComponent.ModePropertyName);
            text = serializedObject.FindProperty(LuaComponent.TextPropertyName);
            asset = serializedObject.FindProperty(LuaComponent.AssetPropertyName);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(mode);

            LuaComponent.SourceMode sourceMode = (LuaComponent.SourceMode)mode.enumValueIndex;
            if (sourceMode == LuaComponent.SourceMode.Literal)
            {
                EditorGUILayout.PropertyField(text);
            }
            else if (sourceMode == LuaComponent.SourceMode.Asset)
            {
                EditorGUILayout.PropertyField(asset);
            }
            else
            {
                EditorGUILayout.LabelField("Unknown mode type");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
