using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realit.Reader.Managers;

namespace Realit.Reader.Features
{
    public abstract class FeatureData<T> : FeatureDataAsset where T : Feature
    {
        internal override Feature GenerateFeature(SceneManager controller) => GenerateFeatureFromData(controller);
        public abstract T GenerateFeatureFromData(SceneManager controller);
    }

    public abstract class FeatureDataAsset : ScriptableObject
    {
        internal abstract Feature GenerateFeature(SceneManager controller);

        internal abstract bool CanGenerateFeature();
    }
}
