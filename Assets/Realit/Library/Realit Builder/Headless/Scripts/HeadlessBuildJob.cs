using Realit.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TriLibCore;
using UnityEngine;
using System.Xml;
using System.Threading.Tasks;
using Aokoro.Sequencing;
using Realit.Settings;

namespace Realit.Builder.Headless
{
    public class HeadlessBuildJob
    {
        public bool CanBuild => IsPlayerSetup && IsModelSetup && IsProjectSetup;
        public bool IsProjectSetup => Scene.IsValid(DataSection.Project);
        public bool IsPlayerSetup => Scene.IsValid(DataSection.Player);
        public bool IsModelSetup => Scene.IsValid(DataSection.Model);
        public bool AreAssetSetup => Scene.IsValid(DataSection.Assets);

        public RealitScene Scene { get; private set; }
        public HeadlessBuilderData Data { get; private set; }
        public string Output { get; private set; }

        public bool IsMiao;

        Sequencer sequencer;

        public HeadlessBuildJob(HeadlessBuilderData data, string output)
        {
            Scene = new RealitScene();
            this.Data = data;
            this.Output = output;
        }
        private void StopSequence()
        {
            sequencer.Stop();
        }
        public void Process()
        {
            sequencer = SequencerBuilder.Begin()
                .Do(() =>
                {
                    try
                    {
                        RealitHeadlessBuilder.Log(".rltb loaded. Creating Scene...");

                        //Setting up data...
                        ModelImporter.OnMaterialLoaded += OnModelLoaded;
                        ModelImporter.Instance.LoadMesh(Data.ModelPath);

                        PlayerDataBuilder.SetPlayerRotation(Data.PlayerRotation);
                        PlayerDataBuilder.SetPlayerPosition(Data.PlayerPosition);
                    }
                    catch
                    {
                        RealitHeadlessBuilder.ExitWithError();
                        StopSequence();
                        throw;
                    }
                })
                .WaitUntil(() => CanBuild)
                .Do(() =>
                {
                    try
                    {
                        var watch = new System.Diagnostics.Stopwatch();
                        //Building
                        RealitHeadlessBuilder.Log($"Scene ready. Building at {Output}...");

                        watch.Start();
                        RealitHeadlessBuilder.Log("Gathering Data...");
                        var buildData = Scene.BuildData(false, RealitSettings.GlobalSettings.formatting);
                        watch.Stop();

                        sequencer.SetParameter("buildData", buildData);
                        RealitHeadlessBuilder.Log($"Data gathered : {watch.ElapsedMilliseconds / 1000f} secondes. Compressing...");
                    }
                    catch
                    {
                        RealitHeadlessBuilder.ExitWithError();
                        StopSequence();
                        throw;
                    }
                })
                .Yield()
                .Do(() =>
                {
                    try
                    {
                        var watch = new System.Diagnostics.Stopwatch();
                        byte[] buildData = sequencer.GetParameterValue<byte[]>("buildData");
                        
                        watch.Start();
                        byte[] compressedData = RealitSettings.GlobalSettings.rszCompression switch
                        {
                            Settings.CompressionType.Gzip => RealitCompressor.CompressGzip(buildData, System.IO.Compression.CompressionLevel.Optimal),
                            Settings.CompressionType.Brotli => RealitCompressor.CompressBrotli(buildData, System.IO.Compression.CompressionLevel.Optimal),
                            Settings.CompressionType.None => buildData,
                        };
                        watch.Stop();


                        RealitHeadlessBuilder.Log($"Data compressed : {watch.ElapsedMilliseconds / 1000f} secondes");
                        
                        Directory.CreateDirectory(Output);
                        string path = Path.Combine(Output, $"{Data.ProjectName}.rsz");
                        WriteFile(path, compressedData);
                    }
                    catch
                    {
                        RealitHeadlessBuilder.ExitWithError();
                        StopSequence();
                        throw;
                    }
                   
                })
                .Build();


            sequencer.Play();
        }

        private static void WriteFile(string path, byte[] buildData)
        {
            try
            {
                RealitHeadlessBuilder.Log($"Writing data to {path}...");
                File.WriteAllBytes(path, buildData);

                RealitHeadlessBuilder.Log($"Successfully writted");
                RealitHeadlessBuilder.ExitWithSuccess();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                RealitHeadlessBuilder.ExitWithError();
                throw;
            }
        }

        private void OnModelLoaded(AssetLoaderContext ctx)
        {
            ModelImporter.OnMaterialLoaded -= OnModelLoaded;

            if (RealitHeadlessBuilder.Instance.TryGetArg("-miao", out string xmlPath))
            {
                try
                {
                    if (File.Exists(xmlPath))
                    {
                        XmlDocument xmlDoc = new XmlDocument();

                        using (FileStream fileStream = new FileStream(xmlPath, FileMode.Open, FileAccess.Read))
                            xmlDoc.Load(fileStream);

                        MiaoMapper miao = ScriptableObject.CreateInstance<MiaoMapper>();
                        miao.PostProcess(xmlDoc, ctx);
                    }
                    else
                    {
                        RealitHeadlessBuilder.Log($"Couldn't find a miao collada at path {xmlPath}.");
                    }

                }
                catch
                {
                    RealitHeadlessBuilder.Log("Error while reading the miao collada...");
                }
            }
            else
            {

                for (int i = 0; i < Data.Appertures.Length; i++)
                {
                    string[] s = Data.Appertures[i].Split('.');
                    string path = s[0];

                    int[] submeshes = s.Length > 1 ? GetSubmeshesFromString(s[1]) : new int[0];

                    ModelDataBuilder.AddAperture(path, submeshes);
                }
            }
            
        }

        private int[] GetSubmeshesFromString(string s)
        {
            string[] stringIndices = s.Split('&');
            int[] indices = new int[stringIndices.Length];

            for (int i = 0; i < stringIndices.Length; i++)
                indices[i] = int.Parse(stringIndices[i]);

            return indices;
        }

    }
}
