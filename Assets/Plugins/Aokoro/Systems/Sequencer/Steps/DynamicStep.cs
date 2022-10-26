using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Aokoro.Sequencing.Steps
{
    [Serializable]
    public class DynamicStep : IStep
    {
        private IStep dynamicStep;
        private Func<IStep> getStep;

        private object[] dependances = new object[0];
        public DynamicStep(Func<IStep> getStep)
        {
            this.getStep = getStep;
        }
        
        void IStep.OnBegin()
        {
            dynamicStep = getStep.Invoke();
            SequencerManager.BeginStep(dynamicStep, this.Parent());
        }

        void IStep.OnEnd()
        {
            SequencerManager.EndStep(dynamicStep);
        }

        bool IStep.Tick(ISequencer parent) => dynamicStep.Tick(parent);
    }
}
