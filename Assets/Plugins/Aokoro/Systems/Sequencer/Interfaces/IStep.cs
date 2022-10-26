using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aokoro.Sequencing
{

    public interface IStep
    {
        /// <summary>
        /// Called at the first tick from the Sequencer
        /// </summary>
        public void OnBegin();


        /// <summary>
        /// Called every frame when runned by a Sequence.
        /// </summary>
        /// <param name="parent">Sequence that runs the step</param>
        /// <returns></returns>
        public bool Tick(ISequencer parent);
        /// <summary>
        /// Called at the last tick from the Sequencer
        /// </summary>
        public void OnEnd();
    }


    
}
