using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Library
{
    public struct RealitAssetData
    {
        public string address;
        Dictionary<string,ISerializedProperty> properties;

        public RealitAssetData(string address)
        {
            this.address = address;
            properties = new();
        }

        public T GetProperty<T>(string propertyName) where T : ISerializedProperty
        {
            if (properties.ContainsKey(propertyName))
                return (T)properties[key: propertyName];

            return default;
        }

        public RealitAssetData AddIntProperty(string name, int value)
        {
            properties.Add(name, new IntProperty(value));
            return this;
        }

        public RealitAssetData AddFloatProperty(string name, float value)
        {
            properties.Add(name, new FloatProperty(value));
            return this;
        }

        public RealitAssetData AddVector3Property(string name, Vector3 value)
        {
            properties.Add(name, new Vector3Property(value));
            return this;
        }

        public RealitAssetData AddStringProperty(string name, string value)
        {
            properties.Add(name, new StringProperty(value));
            return this;
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            foreach (var property in properties)
                json.Add(new JProperty(property.Key, property.Value.SerializedValue()));

            return json;
        }

        public RealitAssetData FromJson(JContainer json)
        {
            foreach (JProperty property in json)
            {
                JToken token = property.Value;
                string key = property.Name;

                switch (token.Type)
                {
                    case JTokenType.Integer:
                        properties.Add(key, new IntProperty((int)token));
                        break;
                    case JTokenType.Float:
                        properties.Add(key, new FloatProperty((float)token));
                        break;
                    case JTokenType.String:
                        string value = (string)token;
                        if(Vector3Property.IsVector3(value))
                            properties.Add(key, new Vector3Property(value));
                        else
                            properties.Add(key, new StringProperty(value));

                        break;
                    case JTokenType.Boolean:
                        properties.Add(key, new BoolProperty((bool)token));
                        break;
                    default:
                        break;
                }
            }

            return this;
        }
    }
}
