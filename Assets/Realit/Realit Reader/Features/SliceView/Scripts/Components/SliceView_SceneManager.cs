using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Realit.Reader.Features.SliceView
{
    public class SliceView_SceneManager : FeatureComponent<SliceView>
    {
        public static SliceView_SceneManager Instance
        {
            get
            {
                if(m_instance == null)
                    m_instance = GameObject.FindObjectOfType<SliceView_SceneManager>();

                return m_instance;
            }
        }

        private static SliceView_SceneManager m_instance;

        public Bounds LevelBounds;

        protected override void Awake()
        {
            if (Instance != this)
                Destroy(gameObject);
            else
                base.Awake();
        }

        protected override void OnFeatureInitiate()
        {
        }

        protected override void OnFeatureStarts()
        {
            if (LevelBounds == null)
                LevelBounds = GetCurrentBounds();
        }

        private Bounds GetCurrentBounds()
        {
            return new Bounds();
        }

        protected override void OnFeatureEnds()
        {
        }

    }
}