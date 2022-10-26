using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Realit.Scene
{
    public enum DataSection
    {
        Project = 0,
        Model = 1,
        Player = 2,
        Assets = 3
    }

    public class RealitScene
    {
        private Encoding encoding = Encoding.UTF8;

#if IS_BUILDER
        private Dictionary<DataSection, IDataBuilder> builders;
        public bool IsValid(DataSection sceneSection) => builders.TryGetValue(sceneSection, out IDataBuilder dataBuilder) && dataBuilder.IsValid;
#endif
#if IS_READER
        private Dictionary<DataSection, IDataReader> readers;
#endif

        public RealitScene()
        {
            var gos = SceneManager.GetActiveScene().GetRootGameObjects();
#if IS_READER
            readers = new Dictionary<DataSection, IDataReader>();

            var allGetters = gos.SelectMany(go => go.GetComponentsInChildren<IDataReader>());

            ///Todo : priorités et versions
            foreach (var provider in allGetters)
            {
                if (!readers.ContainsKey(provider.Section))
                    readers.Add(provider.Section, provider);
            }
#endif

#if IS_BUILDER
            builders = new Dictionary<DataSection, IDataBuilder>();

            var allSetters = gos.SelectMany(go => go.GetComponentsInChildren<IDataBuilder>());
            foreach (var provider in allSetters)
            {
                if (!builders.ContainsKey(provider.Section))
                    builders.Add(provider.Section, provider);
            }
#endif

        }

#if IS_BUILDER
        
        public byte[] BuildData(bool compress = true, Formatting formatting = Formatting.None)
        {
            JObject json = new JObject();

            foreach (var kvp in builders)
                json.Add(new JProperty(kvp.Key.ToString(), kvp.Value.Serialize()));

            string stringData = json.ToString(formatting);
            var uncompressedData = encoding.GetBytes(stringData);

            return compress ? RealitCompressor.CompressBrotli(uncompressedData) : uncompressedData;
        }
#endif

#if IS_READER
        public bool IsDone => readers.All(ctx => ctx.Value.Done || ctx.Value.Skipped);

        public void ReadData(byte[] data)
        {
            var decompressedData = RealitCompressor.DecompressBrotli(data);

            JObject json = JObject.Parse(encoding.GetString(decompressedData));

            ReadSection(json, DataSection.Project);
            ReadSection(json, DataSection.Model);
            ReadSection(json, DataSection.Player);
            ReadSection(json, DataSection.Assets);

        }

        private void ReadSection(JObject json, DataSection section)
        {
            if (readers.TryGetValue(section, out IDataReader reader))
            {
                string key = section.ToString();
                if (json.TryGetValue(key, StringComparison.OrdinalIgnoreCase, out JToken token))
                    reader.ApplyData(token);
                else
                    reader.Skipped = true;
            }
        }
#endif
    }

}