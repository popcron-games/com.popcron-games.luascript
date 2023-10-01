#nullable enable
using MoonSharp.Interpreter;
using UnityEngine;

namespace Popcron.MoonSharpProxies
{
    public class ObjectProxy
    {
        [MoonSharpHidden]
        public Object value;

        public string name
        {
            get => value.name;
            set => this.value.name = value;
        }

        [MoonSharpHidden]
        public ObjectProxy(Object value)
        {
            this.value = value;
        }

        [MoonSharpHidden]
        public static implicit operator Object(ObjectProxy value)
        {
            return value.value;
        }
    }

    public class GameObjectProxy : ObjectProxy
    {
        [MoonSharpHidden]
        public new GameObject value;

        public Transform transform
        {
            get => value.transform;
        }

        [MoonSharpHidden]
        public GameObjectProxy(GameObject value) : base(value)
        {
            this.value = value;
        }
    }

    public class ComponentProxy : ObjectProxy
    {
        [MoonSharpHidden]
        public new Component value;

        [MoonSharpHidden]
        public ComponentProxy(Component value) : base(value)
        {
            this.value = value;
        }

        public object? getComponent(string componentTypeName)
        {
            Component[] components = value.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (component.GetType().Name == componentTypeName)
                {
                    return new ComponentProxy(component);
                }
            }

            return null;
        }
    }

    public class BehaviourProxy : ComponentProxy
    {
        [MoonSharpHidden]
        public new Behaviour value;

        public bool enabled
        {
            get => value.enabled;
            set => this.value.enabled = value;
        }

        [MoonSharpHidden]
        public BehaviourProxy(Behaviour value) : base(value)
        {
            this.value = value;
        }
    }

    public class MonoBehaviourProxy : BehaviourProxy
    {
        [MoonSharpHidden]
        public new MonoBehaviour value;

        [MoonSharpHidden]
        public MonoBehaviourProxy(MonoBehaviour value) : base(value)
        {
            this.value = value;
        }
    }

    public class ScriptableObjectProxy : ObjectProxy
    {
        [MoonSharpHidden]
        public new ScriptableObject value;

        [MoonSharpHidden]
        public ScriptableObjectProxy(ScriptableObject value) : base(value)
        {
            this.value = value;
        }
    }

    public class TransformProxy : ComponentProxy
    {
        [MoonSharpHidden]
        public new Transform value;

        public GameObject gameObject
        {
            get => value.gameObject;
        }

        public Vector3 position
        {
            get => value.position;
            set => this.value.position = value;
        }

        public Vector3 localPosition
        {
            get => value.localPosition;
            set => this.value.localPosition = value;
        }

        public Vector3 localScale
        {
            get => value.localScale;
            set => this.value.localScale = value;
        }

        public Vector3 localEulerAngles
        {
            get => value.localEulerAngles;
            set => this.value.localEulerAngles = value;
        }

        [MoonSharpHidden]
        public TransformProxy(Transform value) : base(value)
        {
            this.value = value;
        }
    }

    public class MaterialPropertyBlockProxy
    {
        [MoonSharpHidden]
        public MaterialPropertyBlock value;

        [MoonSharpHidden]
        public MaterialPropertyBlockProxy(MaterialPropertyBlock value)
        {
            this.value = value;
        }

        public void setFloat(string name, float value)
        {
            this.value.SetFloat(name, value);
        }

        public void setVector(string name, Vector4 value)
        {
            this.value.SetVector(name, value);
        }

        public void setVector3(string name, float x, float y, float z)
        {
            this.value.SetVector(name, new Vector3(x, y, z));
        }

        public void setVector4(string name, float x, float y, float z, float w)
        {
            this.value.SetVector(name, new Vector4(x, y, z, w));
        }

        public void setColor(string name, string colorHex)
        {
            this.value.SetColor(name, ColorUtility.TryParseHtmlString(colorHex, out Color color) ? color : Color.white);
        }

        public void setColor(string name, Color value)
        {
            this.value.SetColor(name, value);
        }

        public void setTexture(string name, Texture value)
        {
            this.value.SetTexture(name, value);
        }

        public void setMatrix(string name, Matrix4x4 value)
        {
            this.value.SetMatrix(name, value);
        }

        public void setBuffer(string name, ComputeBuffer value)
        {
            this.value.SetBuffer(name, value);
        }

        public void setFloatArray(string name, float[] value)
        {
            this.value.SetFloatArray(name, value);
        }

        public void setVectorArray(string name, Vector4[] value)
        {
            this.value.SetVectorArray(name, value);
        }

        public void setMatrixArray(string name, Matrix4x4[] value)
        {
            this.value.SetMatrixArray(name, value);
        }
    }
}