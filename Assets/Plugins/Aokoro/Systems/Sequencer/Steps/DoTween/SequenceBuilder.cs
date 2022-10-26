using Aokoro.Sequencing.Steps;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.Sequencing
{
    public partial class SequencerBuilder
    {
        /// <summary>
        /// Wait for a tween
        /// </summary>
        /// <returns></returns>
        public SequencerBuilder Tween(Func<Tween> getTween)
        {
            Steps.Add(new TweenStep(getTween));
            return this;
        }
    }
}