using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace Aokoro.Sequencing.Steps
{

    public class TweenStep : IStep
    {
        private Func<Tween> getTween;
        private Tween instance;

        public TweenStep(Func<Tween> getTween)
        {
            this.getTween = getTween;
        }


        void IStep.OnBegin()
        {
            instance = getTween.Invoke();
            instance.SetAutoKill(false);
        }

        bool IStep.Tick(ISequencer parent) => instance.IsComplete();

        void IStep.OnEnd()
        {
            instance.Kill(true);
            instance = null;
        }
    }
}