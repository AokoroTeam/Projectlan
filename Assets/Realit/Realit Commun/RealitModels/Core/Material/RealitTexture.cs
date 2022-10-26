using Aspose.ThreeD.Shading;
using NaughtyAttributes;
using Newtonsoft.Json.Linq;
using Realit.Settings;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace Realit.Models.Materials
{
    [System.Serializable]
    public class RealitTexture
    {
        private const string WidthKey = "w";
        private const string HeightKey = "h";
        private const string DataKey = "d";
        private const string MipCountKey = "mc";
        private const string CompressionKey = "c";
        private const string DataTypeKey = "dt";
        private const string GraphicFormatKey = "gf";
        private const string TextureFormatKey = "tf";

#if UNITY_EDITOR
        [SerializeField, ReadOnly, AllowNesting]
        Texture2D sourceTexture;
#endif
        [SerializeField, ReadOnly, AllowNesting]
        Settings.CompressionType compression;
        [SerializeField, ReadOnly, AllowNesting]
        TextureDataType dataType;

        [SerializeField, ReadOnly, AllowNesting]
        int height, width;
        [SerializeField, ReadOnly, AllowNesting]
        int mipCount;
        [SerializeField, ReadOnly, AllowNesting]
        TextureFormat textureFormat;
        [SerializeField, ReadOnly, AllowNesting]
        GraphicsFormat graphicFormat;

        byte[] data;


        public RealitTexture(Texture2D texture)
        {
#if UNITY_EDITOR
            this.sourceTexture = texture;
#endif
            width = texture.width;
            height = texture.height;
            graphicFormat = SystemInfo.GetCompatibleFormat(texture.graphicsFormat, FormatUsage.Render);

            Texture2D validTexture = new(width, height, graphicFormat, TextureCreationFlags.None);

            RenderTexture last = RenderTexture.active;
            RenderTexture temp = RenderTexture.GetTemporary(width, height, 32, graphicFormat);
            RenderTexture.active = temp;


            //Converting
            Graphics.Blit(texture, temp);
            validTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
            validTexture.Apply(true, false);

            RenderTexture.ReleaseTemporary(temp);
            RenderTexture.active = last;

            textureFormat = validTexture.format;
            mipCount = validTexture.mipmapCount;

            ExtractDataFromTexture(validTexture);

            GameObject.DestroyImmediate(validTexture);
        }

        public RealitTexture(JToken token)
        {
            height = (int)token[HeightKey];
            width = (int)token[WidthKey];


            graphicFormat = (GraphicsFormat)token[GraphicFormatKey].ToObject<int>();
            textureFormat = (TextureFormat)token[TextureFormatKey].ToObject<int>();

            compression = (Settings.CompressionType)token[CompressionKey].ToObject<int>();
            dataType = (TextureDataType)token[DataTypeKey].ToObject<int>();

            mipCount = token[MipCountKey].ToObject<int>();
            var bytes = token[DataKey].ToObject<byte[]>();

            this.data = compression switch
            {
                Settings.CompressionType.Brotli => RealitCompressor.DecompressBrotli(bytes),
                Settings.CompressionType.Gzip => RealitCompressor.DecompressGzip(bytes),
                Settings.CompressionType.None => bytes,
                _ => bytes
            };

#if UNITY_EDITOR
            this.sourceTexture = null;
#endif
        }

        private void ExtractDataFromTexture(Texture2D texture)
        {
            
            RealitSettings globalSettings = RealitSettings.GlobalSettings;
            if(globalSettings.IsCustomCompression)
            {
                compression = globalSettings.textureCompression;
                dataType = globalSettings.textureDataType;

                byte[] bytes = dataType == TextureDataType.Image ? texture.EncodeToPNG() : texture.GetRawTextureData();

                data = compression switch
                {
                    Settings.CompressionType.None => bytes,
                    Settings.CompressionType.Gzip => RealitCompressor.CompressGzip(bytes),
                    Settings.CompressionType.Brotli => RealitCompressor.CompressBrotli(bytes),
                    _ => null,
                };
            }
            else
            {
                compression = Settings.CompressionType.Gzip;
                if (GraphicsFormatUtility.IsCrunchFormat(textureFormat))
                {
                    dataType = TextureDataType.RawData;
                    this.data = RealitCompressor.CompressGzip(texture.GetRawTextureData());
                }
                else
                {
                    byte[] image = texture.EncodeToPNG();
                    byte[] rawData = texture.GetRawTextureData();

                    bool isImage = image.Length < rawData.Length;
                    Debug.Log(isImage);
                    dataType = isImage ? TextureDataType.Image : TextureDataType.RawData;
                    this.data = RealitCompressor.CompressGzip(isImage ? image : rawData);
                }
            }
        }
        
        public void GenerateTexture(out Texture2D texture)
        {

            TextureCreationFlags creationFlags = TextureCreationFlags.MipChain;

            if(GraphicsFormatUtility.IsCrunchFormat(textureFormat))
                creationFlags |= TextureCreationFlags.Crunch;

            texture = new Texture2D(width, height, graphicFormat, mipCount, creationFlags);
            
            switch (dataType)
            {
                case TextureDataType.Image:
                    texture.LoadImage(data);
                    break;
                case TextureDataType.RawData:
                    texture.LoadRawTextureData(data);
                    break;
            }

            if (Compressable(width) && Compressable(height))
                texture.Compress(false);

            //Free some space
            texture.Apply(true, true);

#if UNITY_EDITOR
            this.sourceTexture = texture;
#endif
        }

        private bool Compressable(int pixels) => pixels >= 4 &&(pixels % 4) == 0;
        public JToken Serialize()
        {
            Debug.Log($"{graphicFormat} and {textureFormat}");

            return new JObject(
                new JProperty(WidthKey, width),
                new JProperty(HeightKey, height),
                new JProperty(GraphicFormatKey, (int)graphicFormat),
                new JProperty(TextureFormatKey, (int)textureFormat),
                new JProperty(CompressionKey, compression),
                new JProperty(DataTypeKey, dataType),
                new JProperty(DataKey, data),
                new JProperty(MipCountKey, mipCount)
            );
        }
    }
}