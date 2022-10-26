using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Aokoro.Sequencing.Steps
{
    [Serializable]
    public class WaitUntilStep : IStep
    {
        private Func<bool> condition;
        public WaitUntilStep(Func<bool> condition)
        {
            this.condition = condition;
        }

        void IStep.OnBegin(){}

        bool IStep.Tick(ISequencer parent)
        {
            if (condition == null)
            {
                SequencerManager.LogError(" There is no conditions. returning true", this);
                return true;
            }

            bool result = condition.Invoke();
            return result;
        }

        void IStep.OnEnd(){}
    }
}
