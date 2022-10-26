using Newtonsoft.Json.Serialization;
using System.Collections;

using System;
using System.Linq;

using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Realit
{
    public static class RealitSerializer
    {

        #region Vector 3
        public static JArray Serialize(this Vector3 vector) => new(vector.x, vector.y, vector.z);
        public static Vector3 Deserialize(this Vector3 vector, JToken token)
        {
            vector.x = (float)token[0];
            vector.y = (float)token[1];
            vector.z = (float)token[2];
            
            return vector;
        }
        public static JArray Serialize(this Vector3[] vectors) => new(vectors.Select(Serialize));
        public static Vector3[] Deserialize(this Vector3[] vectors, JToken array)
        {
            vectors = array.Select(token => new Vector3().Deserialize(token)).ToArray();
            return vectors;
        }
        #endregion

        #region Vector 2
        public static JArray Serialize(this Vector2 vector) => new(vector.x, vector.y);
        public static Vector2 Deserialize(this Vector2 vector, JToken token)
        {
            vector.x = (float)token[0];
            vector.y = (float)token[1];
            return vector;
        }
        public static JArray Serialize(this Vector2[] vectors) => new(vectors.Select(Serialize));
        public static Vector2[] Deserialize(this Vector2[] vectors, JToken array)
        {
            vectors = array.Select(token => new Vector2().Deserialize(token)).ToArray();
            return vectors;
        }
        #endregion
    }
}
