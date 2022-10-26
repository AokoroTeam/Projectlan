using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aokoro.Sequencing
{
    public static class StepExtensions
    {
        public static Sequencer ToSequence(this IEnumerable<IStep> steps) => new Sequencer(steps.ToArray());
        public static bool IsRunning(this IStep step) => SequencerManager.IsRunning(step);
        
        public static ISequencer Root(this IStep step) => SequencerManager.Root(step);
        public static ISequencer Parent(this IStep step) => SequencerManager.GetParent(step);
    }
}