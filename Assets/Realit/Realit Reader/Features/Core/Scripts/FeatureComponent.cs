using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Realit.Reader.Features
{

    public abstract class FeatureComponent<T> : MonoBehaviour where T : Feature
    {
        public T feature { get; private set; }
        
        protected virtual void Awake()
        {
            Feature.OnNewFeatureIsInitiated += BindToFeature;
        }

        protected virtual void Start()
        {
            if (feature == null)
                Destroy(gameObject);
        }

        private void OnEnable()
        {
            if (feature != null)
            {
                feature.onFeatureStarts += OnFeatureStarts;
                feature.onFeatureEnds += OnFeatureEnds;
            }
        }
        
        private void OnDisable()
        {
            if (feature != null)
            {
                feature.onFeatureStarts -= OnFeatureStarts;
                feature.onFeatureEnds -= OnFeatureEnds;
            }
        }

        private void BindToFeature(Feature feature)
        {
            if (this.feature != null)
                return;

            if (feature is T t_feature)
            {
                this.feature = t_feature;

                Feature.OnNewFeatureIsInitiated -= BindToFeature;

                t_feature.onFeatureStarts += OnFeatureStarts;
                t_feature.onFeatureEnds += OnFeatureEnds;
                OnFeatureInitiate();
            }
        }
        
        protected abstract void OnFeatureInitiate();
        protected abstract void OnFeatureStarts();
        protected abstract void OnFeatureEnds();
    }
}