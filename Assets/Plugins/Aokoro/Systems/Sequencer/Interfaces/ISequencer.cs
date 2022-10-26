using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.Sequencing
{
    public interface ISequencer : IStep
    {
        Action OnStarted { get; set; }
        Action OnFinished { get; set; }

        /// <summary>
        /// Current step in sequencer
        /// </summary>
        public IStep Current { get; }

        /// <summary>
        /// Move to next step
        /// </summary>
        /// <param name="next"></param>
        /// <returns>True if sequencer isn't finished yet</returns>
        public bool Next();
    }
}