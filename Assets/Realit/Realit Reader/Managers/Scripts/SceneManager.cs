using Aokoro;
using Aokoro.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Realit.Reader.Features;
using Realit.Reader.Player;


namespace Realit.Reader.Managers
{


    [DefaultExecutionOrder(-80)]
    [AddComponentMenu("Realit/Reader/Managers/Scene manager")]
    public class SceneManager : Singleton<SceneManager>
    {
        public enum InitialisationPhase
        {
            None,
            Level,
            Player,
            Features,
            Entities,
            Others,
            Done,
        }

        public WindowManager mainUI;

        /// Initialisation order : 
        /// Level
        /// Player
        /// Features
        /// Entities
        /// Others
        /// 
        public InitialisationPhase LevelInitiationPhase { get; private set; } = InitialisationPhase.None;


        public static event Action<Realit_Player> OnPlayerIsSetup;

        [SerializeField] FeatureDataAsset[] GameFeatures;

        public Realit_Player Player { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            WindowManager.MainUI = mainUI;
        }


        public void InitializeScene(FeatureDataAsset[] loadedFeatures)
        {
            Debug.Log($"[Scene Manager] Initializing Scene");

            InitializeLevel();
            InitializePlayer();
            InitializeFeatures(loadedFeatures);
            InitializeOthers();

            LevelInitiationPhase = InitialisationPhase.Done;

            Debug.Log($"[Scene Manager] Scene initialized");
        }


        protected void InitializeLevel()
        {
            LevelInitiationPhase = InitialisationPhase.Level;
        }


        protected void InitializePlayer()
        {
            LevelInitiationPhase = InitialisationPhase.Player;
            Player = Realit_Player.LocalPlayer as Realit_Player;
            
            Player.OnAwake();

            OnPlayerIsSetup?.Invoke(Player);
        }

        protected void InitializeFeatures(FeatureDataAsset[] loadedFeatures)
        {
            LevelInitiationPhase = InitialisationPhase.Features;

            int length = loadedFeatures.Length;
            var features = new Feature[length];
            for (int i = 0; i < length; i++)
                features[i] = loadedFeatures[i].GenerateFeature(this);

            Player.SetupPlayerFeatures(features);
        }

        protected void InitializeOthers()
        {
            LevelInitiationPhase = InitialisationPhase.Others;

        }

        protected override void OnExistingInstanceFound(SceneManager existingInstance)
        {
            Destroy(gameObject);
        }

        private void OnDisable()
        {
            OnPlayerIsSetup = null;
        }
    }
}
