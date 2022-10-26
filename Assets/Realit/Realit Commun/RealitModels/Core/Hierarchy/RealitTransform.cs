using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Realit.Models.Hierarchy
{
    [System.Serializable]
    public class RealitTransform
    {
        private const string nameKey = "name";
        private const string positionKey = "position";
        private const string rotationKey = "rotation";
        private const string scaleKey = "scale";

        #region Data

        public readonly int ID;
        public readonly string Name;
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly Vector3 Scale;

        public Transform bindedTransform;
        #endregion

        public RealitTransform(Transform t, int index)
        {
            ID = index;
            Name = t.name;
            Position = t.localPosition;
            Rotation = t.localRotation;
            Scale = t.localScale;
            bindedTransform = t;
        }

        public RealitTransform(JObject data, int index)
        {
            ID = index;
            Name = (string)data[nameKey];
            Position = new Vector3().Deserialize(data[positionKey]);
            Rotation = Quaternion.Euler(new Vector3().Deserialize(data[rotationKey]));
            Scale = new Vector3().Deserialize(data[scaleKey]);
            bindedTransform = null;
        }

        public Transform CreateTransform()
        {
            Transform transform = new GameObject(Name).transform;

            transform.position = Position;
            transform.rotation = Rotation;
            transform.localScale = Scale;

            bindedTransform = transform;
            return transform;
        }

       
        public JToken Serialize() => new JObject(
                                        new JProperty(nameKey, Name),
                                        new JProperty(positionKey, Position.Serialize()),
                                        new JProperty(rotationKey, Rotation.eulerAngles.Serialize()),
                                        new JProperty(scaleKey, Scale.Serialize())
                                        );
    }
}