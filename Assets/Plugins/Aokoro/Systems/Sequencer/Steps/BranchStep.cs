using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Aokoro.Sequencing.Steps
{
    public class BranchStep : ISequencer
    {
        public Action OnStarted { get; set; }
        public Action OnFinished { get; set; }

        private Func<ISequencer> condition;

        private ISequencer branch;
        IStep ISequencer.Current => branch.Current;

        public BranchStep(Func<ISequencer> condition)
        {
            this.condition = condition;
        }

        void IStep.OnBegin()
        {
            try
            {
                branch = condition.Invoke();
            }
            catch(Exception e)
            {
                SequencerManager.LogError(e, this);
                branch = null;
            }
        }

        bool IStep.Tick(ISequencer from) => branch == null || branch.Tick(from);

        void IStep.OnEnd()
        {
        }

        bool ISequencer.Next() => branch != null && branch.Next();
    }
}
