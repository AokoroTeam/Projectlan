using System;
using System.Collections;
using System.Collections.Generic;
using TriLibCore;
using UnityEngine;

namespace Realit.Builder.Headless
{
    public class HeadlessModelDataBuilder : ModelDataBuilder
    {
        protected override void OnModelLoaded(GameObject model)
        {
            if (model == null)
            {
                Console.WriteLine($"[Realit] Couldn't load model. Maybe the path was wrong?");
                return;
            }
            else
            {
                base.OnModelLoaded(model);
            }
        }
        protected override void OnErrorLoading(IContextualizedError error)
        {
            base.OnErrorLoading(error);
            RealitHeadlessBuilder.ExitWithError();
        }
    }
}
