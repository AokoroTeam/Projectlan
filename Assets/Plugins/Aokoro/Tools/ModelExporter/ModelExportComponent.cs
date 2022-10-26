
using System.IO;
using UnityEngine;

namespace Aokoro.ModelExports.Runtime
{
    [AddComponentMenu("Aokoro/ModelExporter")]
    public class ModelExportComponent : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] string exportPath;
        [SerializeField] string fileName;

        private void OnValidate()
        {
            if (exportPath != null && exportPath.StartsWith("Assets/"))
                exportPath = exportPath.Remove(0, 7);
        }

        public void TryExport()
        {
            string filePath = Path.Combine(Application.dataPath, exportPath, $"{fileName}.fbx");
            ModelExport.Export(gameObject, filePath);
        }
#endif

    }
}