using UnityEditor;

namespace Popcron.LuaScript
{
    [CustomEditor(typeof(LuaComponent), true)]
    public class LuaComponentInspector : Editor
    {
        private SerializedProperty mode;
        private SerializedProperty text;
        private SerializedProperty asset;
        private SerializedProperty initialData;

        private void OnEnable()
        {
            mode = serializedObject.FindProperty(LuaComponent.ModePropertyName);
            text = serializedObject.FindProperty(LuaComponent.TextPropertyName);
            asset = serializedObject.FindProperty(LuaComponent.AssetPropertyName);
            initialData = serializedObject.FindProperty(LuaComponent.InitialDataPropertyName);
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

            EditorGUILayout.PropertyField(initialData, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
