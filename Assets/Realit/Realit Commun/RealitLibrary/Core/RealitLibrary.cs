using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Realit.Library
{
    public enum LibraryObjectType
    {
        Prefab,
        Material,
        Model,
    }

    public class RealitLibrary : ScriptableObject
    {
        private static RealitLibrary instance;

        public static RealitLibrary Instance 
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<RealitLibrary>("RealitLibrary");

                return instance;
            } 

        }
        
        public static void LogError(string message) => Debug.LogError($"[Realit Library] {message}");
        public static void LogWarning(string message) => Debug.LogWarning($"[Realit Library] {message}");
        public static void Log(string message) => Debug.Log($"[Realit Library] {message}");
    }
}
