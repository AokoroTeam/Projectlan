using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realit.Reader.Features;
using Realit.Reader.Managers;

namespace Realit.Reader.Features.Settings
{
    public class Realit_Settings : Feature
    {
        public override string FeatureName => "Settings";

        public override void CleanContentOnDestroy(SceneManager controller)
        {
            
        }

        protected override void GenerateNeededContentOnSetup(SceneManager controller)
        {

        }

        protected override void OnFeatureEnds()
        {
        }

        protected override void OnFeatureStarts()
        {
        }
    }
}