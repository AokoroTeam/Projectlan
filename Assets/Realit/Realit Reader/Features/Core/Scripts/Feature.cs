using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realit.Reader.Managers;

namespace Realit.Reader.Features
{
    public abstract class Feature
    {
        public bool IsActive;
        public abstract string FeatureName { get; }
        public static event Action<Feature> OnNewFeatureIsInitiated;
        public event Action onFeatureStarts;
        public event Action onFeatureEnds;

        internal void Setup(SceneManager controller)
        {
            GenerateNeededContentOnSetup(controller);

            OnNewFeatureIsInitiated?.Invoke(this);
        }

        protected abstract void GenerateNeededContentOnSetup(SceneManager controller);
        
        public abstract void CleanContentOnDestroy(SceneManager controller);

        public void StartFeature()
        {
            OnFeatureStarts();
            onFeatureStarts?.Invoke();
        }
        
        public void EndFeature()
        {
            OnFeatureEnds();
            onFeatureEnds?.Invoke();
        }

        protected abstract void OnFeatureStarts();
        protected abstract void OnFeatureEnds();
    }
}