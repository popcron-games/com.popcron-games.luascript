#nullable enable
using Popcron.LuaScript;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomLuaComponent), true)]
public class CustomLuaComponentEditor : LuaComponentInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        CustomLuaComponent component = (CustomLuaComponent)target;
        EditorGUI.BeginChangeCheck();
        foreach ((string type, string name) in component.ExposedVariables)
        {
            object? currentValue;
            LuaScript? script = component.LuaScript;
            if (script is not null)
            {
                currentValue = script.GetObject(name);
            }
            else
            {
                currentValue = component.GetInitialData(name);
            }

            if (type == "decimal")
            {
                if (currentValue is not double doubleValue)
                {
                    doubleValue = 0;
                }

                double newValue = EditorGUILayout.DoubleField(name, doubleValue);
                if (newValue != doubleValue)
                {
                    component.SetInitialData(name, newValue);
                    if (script is not null)
                    {
                        script.SetObject(name, newValue);
                    }
                }
            }
            else if (type == "integer")
            {
                if (currentValue is not int integerValue)
                {
                    integerValue = 0;
                }

                int newValue = EditorGUILayout.IntField(name, integerValue);
                if (newValue != integerValue)
                {
                    component.SetInitialData(name, newValue);
                    if (script is not null)
                    {
                        script.SetObject(name, newValue);
                    }
                }
            }
            else if (type == "boolean")
            {
                if (currentValue is not bool booleanValue)
                {
                    booleanValue = false;
                }

                bool newValue = EditorGUILayout.Toggle(name, booleanValue);
                if (newValue != booleanValue)
                {
                    component.SetInitialData(name, newValue);
                    if (script is not null)
                    {
                        script.SetObject(name, newValue);
                    }
                }
            }
            else if (type == "text")
            {
                if (currentValue is not string textValue)
                {
                    textValue = "";
                }

                string newValue = EditorGUILayout.TextField(name, textValue);
                if (newValue != textValue)
                {
                    component.SetInitialData(name, newValue);
                    if (script is not null)
                    {
                        script.SetObject(name, newValue);
                    }
                }
            }
            else if (type == "vector2")
            {
                if (currentValue is not Vector2 vector2Value)
                {
                    vector2Value = Vector2.zero;
                }

                Vector2 newValue = EditorGUILayout.Vector2Field(name, vector2Value);
                if (newValue != vector2Value)
                {
                    component.SetInitialData(name, newValue);
                    if (script is not null)
                    {
                        script.SetObject(name, newValue);
                    }
                }
            }
            else if (type == "vector3")
            {
                if (currentValue is not Vector3 vector3Value)
                {
                    vector3Value = Vector3.zero;
                }

                Vector3 newValue = EditorGUILayout.Vector3Field(name, vector3Value);
                if (newValue != vector3Value)
                {
                    component.SetInitialData(name, newValue);
                    if (script is not null)
                    {
                        script.SetObject(name, newValue);
                    }
                }
            }
            else if (type == "color")
            {
                Color colorValue = Color.white;
                if (currentValue is string colorStringValue)
                {
                    colorValue = ColorUtility.TryParseHtmlString(colorStringValue, out Color parsedColor) ? parsedColor : Color.white;
                }

                Color newValue = EditorGUILayout.ColorField(name, colorValue);
                if (newValue != colorValue)
                {
                    component.SetInitialData(name, '#' + ColorUtility.ToHtmlStringRGBA(newValue));
                    if (script is not null)
                    {
                        script.SetObject(name, '#' + ColorUtility.ToHtmlStringRGBA(newValue));
                    }
                }
            }
            else if (type == "asset")
            {
                if (currentValue is not Object assetValue)
                {
                    assetValue = null;
                }

                Object? newValue = EditorGUILayout.ObjectField(name, assetValue, typeof(Object), true);
                if (newValue != assetValue)
                {
                    component.SetInitialData(name, newValue);
                    if (script is not null)
                    {
                        script.SetObject(name, newValue);
                    }
                }
            }
            else
            {
                Debug.LogFormat(component, "Unfamiliar type {0}", type);
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(component);
        }

        serializedObject.ApplyModifiedProperties();
    }
}