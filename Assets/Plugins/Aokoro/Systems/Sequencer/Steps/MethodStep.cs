using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Aokoro.Sequencing.Steps
{
    [Serializable]
    public class MethodStep : IStep
    {
        private Action method;

        public MethodStep(Action method)
        {
            this.method = method;
        }

        void IStep.OnBegin(){}

        void IStep.OnEnd(){}

        bool IStep.Tick(ISequencer parent)
        {
            method.Invoke();
            return true;
        }
    }
}
