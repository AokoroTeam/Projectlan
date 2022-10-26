using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Aokoro.Sequencing.Steps
{
    [Serializable]
    public class TimelineStep : IStep
    {
        public PlayableDirector playableDirector { private set; get; }

        public TimelineAsset timeline { private set; get; }

        public TimelineStep(PlayableDirector playableDirector, TimelineAsset timeline) : base()
        {
            this.playableDirector = playableDirector;
            this.timeline = timeline;
        }
        public TimelineStep(PlayableDirector playableDirector) : base()
        {
            this.playableDirector = playableDirector;
        }

        void IStep.OnBegin()
        {
            if (playableDirector.state == PlayState.Playing)
                playableDirector.Stop();

            if (timeline != null)
                playableDirector.Play(timeline);
            else
                playableDirector.Play();
        }

        bool IStep.Tick(ISequencer parent) => playableDirector.state == PlayState.Playing;

        void IStep.OnEnd()
        {
            playableDirector.Stop();
        }
    }
}
