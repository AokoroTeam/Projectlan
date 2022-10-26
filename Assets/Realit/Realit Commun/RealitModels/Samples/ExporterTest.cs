using NaughtyAttributes;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;

namespace Realit.Models.Tests
{
    public class ExporterTest : MonoBehaviour
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);
#endif

        [SerializeField] GameObject objectToExport;
        [SerializeField] Transform parent;

        [SerializeField, ReadOnly]
        private GameObject instance;

        //[SerializeField]
        RealitModel import;
        [SerializeField]
        RealitModel export;

        string file;

        [Button]
        public void Export()
        {
            export = new RealitModel(objectToExport);

            file = export.Serialize().ToString(Newtonsoft.Json.Formatting.None);
        }

        [Button]
        public void Import()
        {
            if (instance != null)
            {
                if (Application.isPlaying)
                    Destroy(instance);
                else
                    DestroyImmediate(instance);
            }

            import = new RealitModel(JObject.Parse(file));

            instance = import.Generate(transform);
            instance.transform.SetPositionAndRotation(parent.position, parent.rotation);

        }

    }
}