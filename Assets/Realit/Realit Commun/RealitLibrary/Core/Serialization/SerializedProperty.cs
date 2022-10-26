using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Library
{

    public interface ISerializedProperty  
    {
        public string SerializedValue();
        public void SetValue(string serializedValue);
    }
    public abstract class SerializedProperty<T> : ISerializedProperty
    {
        public T Value { get; set; }

        public SerializedProperty(T value)
        {
            Value = value;
        }

        public SerializedProperty(string serializedValue)
        {
            SetValue(serializedValue);
        }

        public abstract void SetValue(string serializedValue);
        public abstract string SerializedValue();
    }

    public class IntProperty : SerializedProperty<int>
    {
        public IntProperty(int value) : base(value) { }

        public IntProperty(string serializedValue) : base(serializedValue)
        {
        }

        public override string SerializedValue() => Value.ToString();
        public override void SetValue(string serializedValue) => Value = int.Parse(serializedValue);
    }

    public class FloatProperty : SerializedProperty<float>
    {
        public FloatProperty(float value) : base(value) { }

        public FloatProperty(string serializedValue) : base(serializedValue)
        {
        }

        public override string SerializedValue() => Value.ToString();

        public override void SetValue(string serializedValue) => float.Parse(serializedValue);
    }

    public class Vector3Property : SerializedProperty<Vector3>
    {
        private const string indentificator = "V3_";
        public Vector3Property(Vector3 value) : base(value)
        {
        }

        public Vector3Property(string serializedValue) : base(serializedValue)
        {
        }

        public override string SerializedValue() => $"{indentificator}{Value.x}/{Value.y}/{Value.z}";

        public override void SetValue(string serializedValue)
        {
            string[] values = serializedValue.Remove(0, indentificator.Length).Split('/');
            Value = new Vector3()
            {
                x = float.Parse(values[0]),
                y = float.Parse(values[1]),
                z = float.Parse(values[2]),
            };
        }

        public static bool IsVector3(string serializedValue) => serializedValue.StartsWith(indentificator);
    }

    public class StringProperty : SerializedProperty<string>
    {
        public StringProperty(string value) : base(value) { }

        public override string SerializedValue() => Value;

        public override void SetValue(string serializedValue) => Value = serializedValue;
    }

    public class BoolProperty : SerializedProperty<bool>
    {
        public BoolProperty(bool value) : base(value) { }

        public BoolProperty(string serializedValue) : base(serializedValue)
        {
        }

        public override string SerializedValue() => Value ? "True" : "False";

        public override void SetValue(string serializedValue) => Value = serializedValue == "True";
    }
}
