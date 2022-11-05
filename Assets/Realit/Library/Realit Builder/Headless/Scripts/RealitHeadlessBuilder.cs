using Aokoro;
using DG.Tweening.Plugins.Core.PathCore;
using NaughtyAttributes;
using Newtonsoft.Json;
using Realit.Builder.App;
using Realit.Builder.Headless.Pipes;
using Realit.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TriLibCore;
using UnityEngine;
using Path = System.IO.Path;

namespace Realit.Builder.Headless
{
    [AddComponentMenu("Realit/Builder/Realit Builder (Headless)")]
    public class RealitHeadlessBuilder : RealitBuilderBase
    {
        public static new RealitHeadlessBuilder Instance => GetInstance<RealitHeadlessBuilder>();

        public static event Action OnExit;
        [SerializeField] RealitPipeClient pipeClient;

#if UNITY_EDITOR
        [SerializeField] private string[] arguments;
#endif
        Dictionary<string, string> args;

        private void Start()
        {
            args = GetCommandLineArgs();

            if (args.TryGetValue("-rltb", out string rltbPath) && args.TryGetValue("-output", out string output))
            {
                try
                {
                    Process(rltbPath, output);
                }
                catch (Exception e)
                {
                    ExitWithError();
                    throw e;
                }
            }
            else
            {
                ExitWithError();
            }
        }

        public bool TryGetArg(string arg, out string argValue) => args.TryGetValue(arg, out argValue);

        // Original code from https://docs-multiplayer.unity3d.com/docs/tutorials/goldenpath
        private Dictionary<string, string> GetCommandLineArgs()
        {
            Dictionary<string, string> argumentDictionary = new Dictionary<string, string>();
#if UNITY_EDITOR
            var commandLineArgs = arguments;
#else
            var commandLineArgs = System.Environment.GetCommandLineArgs();
#endif

            for (int argumentIndex = 0; argumentIndex < commandLineArgs.Length; ++argumentIndex)
            {
                var arg = commandLineArgs[argumentIndex].ToLower();
                if (arg.StartsWith("-"))
                {
                    var value = argumentIndex < commandLineArgs.Length - 1 ?
                                commandLineArgs[argumentIndex + 1].ToLower() : null;
                    value = (value?.StartsWith("-") ?? false) ? null : value;

                    argumentDictionary.Add(arg, value);
                }
            }
            return argumentDictionary;
        }
        
        private void Process(string rltbPath, string outputPath)
        {
            Log($"loading .rltb at {rltbPath}...");
            if (!File.Exists(rltbPath))
            {
                Log($"Could not find {rltbPath}");
                ExitWithError();
                return;
            }
            HeadlessBuilderData data = JsonConvert.DeserializeObject<HeadlessBuilderData>(File.ReadAllText(rltbPath), RealitJson.Settings);
            HeadlessBuildJob job = new HeadlessBuildJob(data, outputPath);
            
            job.IsMiao = args.ContainsKey("-miao") || Application.isEditor;
            
            job.Process();
        }


#if !UNITY_EDITOR
        public static void Log(string message) => Instance.pipeClient.AddMessageToQueue($"[Realit] {message}");
#else
        public static void Log(string message) => Debug.Log($"[Realit] {message}");
#endif


        private void OnApplicationQuit()
        {
            OnExit?.Invoke();
            Log("Exiting...");
        }
        
        public static void ExitWithError()
        {
#if !UNITY_EDITOR
            Application.Quit(0);
#else
            Log("Error, should be exiting");
#endif
        }
        public static void ExitWithSuccess()
        {
#if !UNITY_EDITOR
            Application.Quit(1);
#endif
        }
    }
}
