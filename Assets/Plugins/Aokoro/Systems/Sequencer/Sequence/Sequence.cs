using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Aokoro.Sequencing.Steps;
using System.Linq;
using NaughtyAttributes;

namespace Aokoro.Sequencing
{
    
    [Serializable]
    public partial class Sequencer : ISequencer
    {
        public Action OnStarted { get; set; }
        public Action OnFinished { get; set; }

        public List<IStep> Steps;        
        IStep ISequencer.Current => current;

        private IStep current;

        private Queue<IStep> StepQueue;

        public static Sequencer Empty => new Sequencer();
        internal Sequencer(params IStep[] steps) : this(steps.AsEnumerable())
        {

        }
        
        internal Sequencer(IEnumerable<IStep> steps)
        {
            this.Steps = new List<IStep>(steps);
        }

        void IStep.OnBegin()
        {
            StepQueue = new Queue<IStep>(Steps);
            if(StepQueue.Count > 0)
                current = StepQueue.Dequeue();
        }
        bool IStep.Tick(ISequencer parent) => SequencerManager.Update(this, parent);
        
        void IStep.OnEnd()
        {
            StepQueue.Clear();
        }


        bool ISequencer.Next()
        {
            bool continues = StepQueue.Count > 0;
            if(continues)
                current = StepQueue.Dequeue();

            return continues;
        }

    }
}