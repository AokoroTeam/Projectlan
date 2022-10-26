using System;

namespace Aokoro.Sequencing.Steps
{
    [Serializable]
    public class WaitForFramesStep : IStep
    {
        private int frameCount;
        private readonly int targetFrameCount;
        
        public WaitForFramesStep(int targetFrameCount)
        {
            this.targetFrameCount = targetFrameCount;
        }

        void IStep.OnBegin() => frameCount = 0;

        bool IStep.Tick(ISequencer parent)
        {
            frameCount++;
            return frameCount > targetFrameCount;
        }

        void IStep.OnEnd() { }
    }
}
