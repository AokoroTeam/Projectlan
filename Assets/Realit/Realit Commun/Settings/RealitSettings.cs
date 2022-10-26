using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Newtonsoft.Json;

namespace Realit.Settings
{

    public enum CompressionType
    {
        Gzip = 1,
        Brotli = 2,
        None = -1,
    }
    public enum TextureDataType
    {
        Image,
        RawData,
    }

    [CreateAssetMenu(menuName = "Aokoro/Realit/Settings"), JsonObject]
    public class RealitSettings : ScriptableObject
    {
        [Space]
        [BoxGroup("Global"), JsonProperty, AllowNesting]
        public CompressionType rszCompression;
        [BoxGroup("Global"), JsonProperty, AllowNesting]
        public Formatting formatting;

        [HorizontalLine, AllowNesting]
        [BoxGroup("Textures"), JsonProperty]
        public bool IsCustomCompression;
        [JsonProperty]
        [BoxGroup("Textures"), ShowIf(nameof(IsCustomCompression)), AllowNesting]
        public CompressionType textureCompression;
        [JsonProperty]
        [BoxGroup("Textures"), ShowIf(nameof(IsCustomCompression)), AllowNesting]
        public TextureDataType textureDataType;

        public static RealitSettings GlobalSettings;
    }
}
