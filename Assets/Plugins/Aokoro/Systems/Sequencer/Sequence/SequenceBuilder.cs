using Aokoro.Sequencing.Steps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Aokoro.Sequencing
{
    public partial class SequencerBuilder
    {
        /// <summary>
        /// Initialize a builder
        /// </summary>
        /// <returns></returns>
        public static SequencerBuilder Begin() => new SequencerBuilder();
        
        public static SequencerBuilder Begin(params IStep[] steps) => new SequencerBuilder(steps);
        /// <summary>
        /// Changes the builder into a Sequence
        /// </summary>
        /// <returns></returns>
        public Sequencer Build() => new Sequencer(Steps);

        private List<IStep> Steps;

        private SequencerBuilder()
        {
            Steps = new List<IStep>();
        }
        private SequencerBuilder(IStep[] steps)
        {
            Steps = new List<IStep>(steps);
        }

        /// <summary>
        /// Invokes a methode
        /// </summary>
        /// <param name="method">methode to invoke</param>
        /// <returns></returns>
        public SequencerBuilder Do(Action method)
        {
            Steps.Add(new MethodStep(method));
            return this;
        }
        /// <summary>
        /// Invoke a coroutine and wait for it to complete
        /// </summary>
        /// <param name="getRoutine">How to access the coroutine</param>
        /// <param name="from">MonoBehavior with the coroutine</param>
        /// <returns></returns>
        public SequencerBuilder DoCoroutine(Func<IEnumerator> getRoutine, MonoBehaviour from)
        {
            Steps.Add(new CoroutineStep(getRoutine, from));
            return this;
        }

        /// <summary>
        /// Invoke a coroutine and wait for it to complete
        /// </summary>
        /// <param name="coroutine">name of the coroutine</param>
        /// <param name="from">MonoBehavior with the coroutine</param>
        /// <returns></returns>
        public SequencerBuilder DoCoroutine(string coroutine, MonoBehaviour from)
        {
            Steps.Add(new CoroutineStep(coroutine, from));
            return this;
        }

        /// <summary>
        /// Depending of the condition, the sequencer will continue the specified sub-sequencer
        /// </summary>
        /// <param name="condition">What sequencer should it pick?</param>
        /// <returns></returns>
        public SequencerBuilder Branch(Func<ISequencer> condition)
        {
            Steps.Add(new Steps.BranchStep(condition));
            return this;
        }

        public SequencerBuilder If(Func<bool> ifThis, Func<ISequencer> then)
        {
            Steps.Add(new BranchStep(() => ifThis.Invoke() ? then.Invoke() : Sequencer.Empty));
            return this;
        }
        /// <summary>
        /// Excecute and wait for multiple steps to complete
        /// </summary>
        /// <param name="steps">steps to excecute</param>
        /// <returns></returns>
        public SequencerBuilder DoSteps(params IStep[] steps)
        {
            for (int i = 0; i < steps.Length; i++)
                Steps.Add(steps[i]);

            return this;
        }
        /// <summary>
        /// Execute and wait for a step to complete
        /// </summary>
        /// <param name="step"> step to excecute </param>
        /// <returns></returns>
        public SequencerBuilder DoStep(IStep step)
        {
            Steps = Steps.Append(step).ToList();
            return this;
        }
        /// <summary>
        /// Execute and wait for a step to complete
        /// </summary>
        /// <param name="step">Func to get the step to excecute</param>
        /// <returns></returns>
        public SequencerBuilder DoStep(Func<IStep> step)
        {
            Steps = Steps.Append(new DynamicStep(step)).ToList();
            return this;
        }

        /// <summary>
        /// Excecute and wait for a sub-sequencer to complete
        /// </summary>
        /// <param name="sequencer">sub-sequencer</param>
        /// <returns></returns>
        public SequencerBuilder DoStep(ISequencer sequencer) => DoStep((IStep)sequencer);
        /// <summary>
        /// Excecute and wait for multiple steps to complete
        /// </summary>
        /// <param name="steps">steps to excecute</param>
        /// <returns></returns>
        public SequencerBuilder DoSteps(params ISequencer[] steps) => DoSteps((IStep[])steps);
        
        /// <summary>
        /// Excecute and wait for a sub-sequencer to complete
        /// </summary>
        /// <param name="getSequence">how to get sub-sequencer</param>
        /// <returns></returns>
        public SequencerBuilder DoStep(Func<ISequencer> getSequence) => DoStep((Func<IStep>)getSequence);

        /// <summary>
        /// Wait until the condition is true
        /// </summary>
        /// <param name="condition">condition</param>
        /// <returns></returns>
        public SequencerBuilder WaitUntil(Func<bool> condition)
        {
            Steps.Add(new Steps.WaitUntilStep(condition));
            return this;
        }
        
        /// <summary>
        /// Wait for specifics seconds
        /// </summary>
        /// <param name="duration">Duration of the wait</param>
        /// <returns></returns>
        public SequencerBuilder WaitForSeconds(float duration)
        {
            Steps.Add(new WaitSecondsStep(duration));
            return this;
        }

        /// <summary>
        /// Wait for multiple frames
        /// </summary>
        /// <param name="frames">Duration of the wait</param>
        /// <returns></returns>
        public SequencerBuilder WaitForFrames(int frames)
        {
            Steps.Add(new WaitForFramesStep(frames));
            return this;
        }

        /// <summary>
        /// Wait until the next frame
        /// </summary>
        /// <returns></returns>
        public SequencerBuilder Yield()
        {
            Steps.Add(new YieldStep());
            return this;
        }

        public SequencerBuilder Log(string message)
        {
            Steps.Add(new MethodStep(() => SequencerManager.Log(message)));
            return this;
        }
        /// <summary>
        /// Clear de builder
        /// </summary>
        /// <returns></returns>
        public SequencerBuilder Clear()
        {
            Steps.Clear();
            return this;
        }
    } 
}
