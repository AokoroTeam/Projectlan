using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Realit
{
    public class RealitJson
    {

        public static JsonSerializerSettings Settings = new JsonSerializerSettings()
        {
            ContractResolver = new RealitJsonContractSolver(),
            Converters = new JsonConverter[] {
                new Vector3Converter(),
            }
        };
    }

    public class RealitJsonContractSolver : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            JsonContract contract = base.CreateContract(objectType);

            // this will only be called once and then cached
            if (objectType == typeof(Vector3))
                contract.Converter = new Vector3Converter();

            return contract;
        }
    }

    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);

            return new Vector3(
                    (float)obj["x"],
                    (float)obj["y"],
                    (float)obj["z"]
                );
        }

        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            JObject obj = new JObject(
                new JProperty("x", value.x),
                new JProperty("y", value.y),
                new JProperty("z", value.z)
                );

            obj.WriteTo(writer);
        }
    }
}