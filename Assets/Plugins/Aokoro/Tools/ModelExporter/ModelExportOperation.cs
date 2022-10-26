using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Aokoro.ModelExports
{
    internal enum OperationStatus
    {
        Waiting,
        Pending,
        Exported,
        Failed,
        Applied,
    }

    public class ModelExportOperation
    {
#if UNITY_EDITOR
        public event Action On_Finished;
        
       
        ModelExport[] exports;

        OperationStatus status;
        public ModelExportOperation(params ModelExport[] exports)
        {
            status = OperationStatus.Waiting;

            this.exports = exports;
            for (int i = 0; i < exports.Length; i++)
                exports[i].Operation = this;
        }

        internal ModelExportOperation Fire()
        {
            status = OperationStatus.Pending;
            try
            {
                EditorUtility.DisplayProgressBar("[Realit Library] Starting...", string.Empty, 0);
                for (int i = 0; i < exports.Length; i++)
                    exports[i].InternalExport();

            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                
                EditorUtility.ClearProgressBar();
                status = OperationStatus.Failed;
            }
            return this;
        }


        private void OnFinished()
        {
            status = OperationStatus.Exported;
            try
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                for (int i = 0; i < exports.Length; i++)
                {
                    EditorUtility.DisplayProgressBar($"[Realit Library] Applying data...", string.Empty, ((float)i) / exports.Length);
                    exports[i].Apply();
                }
                status = OperationStatus.Applied;
            }
            catch
            {
                status = OperationStatus.Failed;
            }

            EditorUtility.ClearProgressBar();
            On_Finished?.Invoke();
        }

        internal void TriggerStatusChange(ModelExport modelExport)
        {
            if (status != OperationStatus.Pending)
                return;

            int pending = 0;
            int length = exports.Length;

            for (int i = 0; i < length; i++)
            {
                var export = exports[i];
                if (export.OperationStatus != OperationStatus.Exported && export.OperationStatus != OperationStatus.Failed)
                    pending++;
            }

            if (pending > 0)
                return;

            float done = length - pending;
            EditorUtility.DisplayProgressBar($"[Realit Library] Exporting Prefabs ...", $"Progress : {done} / {length}", done / length);

            OnFinished();
        }
#endif
    }
}