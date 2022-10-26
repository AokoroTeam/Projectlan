using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.Sequencing
{
    public static class SequencerExtentions
    {
        public static T AddDependances<T>(this T sequencer, params object[] dependences) where T :ISequencer
        {
            SequencerManager.AddDependances(sequencer, dependences);
            return sequencer;
        }
        public static T RemoveDependances<T>(this T sequencer, params object[] dependences) where T : ISequencer
        {
            SequencerManager.RemoveDependances(sequencer, dependences);
            return sequencer;
        }

        public static object[] Dependances<T>(this T sequencer) where T : ISequencer => SequencerManager.GetDependances(sequencer);

        public static T Play<T>(this T sequencer, params object[] dependences) where T : ISequencer =>
            Play(sequencer, SequenceUpdateType.Update, dependences);

        public static T Play<T>(this T sequencer, SequenceUpdateType updateType, params object[] dependences) where T : ISequencer
        {
            SequencerManager.Play(sequencer, updateType, dependences);

            return sequencer;
        }

        public static T Stop<T>(this T sequencer) where T : ISequencer
        {
            SequencerManager.Stop(sequencer);
            return sequencer;
        }

        internal static bool MoveToNext<T>(this T sequencer, T parent) where T : ISequencer
        {
            if (sequencer.Next())
            {
                //Debug.Log("Next step");
                return SequencerManager.Update(sequencer, parent);
            }
            else
            {
                SequencerManager.EndSequencer(sequencer);
                return true;
            }
        }
    }
}