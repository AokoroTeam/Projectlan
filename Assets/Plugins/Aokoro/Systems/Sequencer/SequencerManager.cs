using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aokoro.Sequencing.Steps;
using NaughtyAttributes;
using System.Linq;

namespace Aokoro.Sequencing
{
    public enum SequenceUpdateType
    {
        Update,
        FixedUpdate,
        LateUpdate,
    }

    [DefaultExecutionOrder(150), ExecuteInEditMode]
    public class SequencerManager : Singleton<SequencerManager>
    {
        [SerializeField]
        private static Dictionary<SequenceUpdateType, List<ISequencer>> Threads
        {
            get 
            {
                if (Instance.t_threads == null)
                {
                    Instance.t_threads = new Dictionary<SequenceUpdateType, List<ISequencer>>()
                    {
                        { SequenceUpdateType.Update, new List<ISequencer>() },
                        { SequenceUpdateType.LateUpdate, new List<ISequencer>() },
                        { SequenceUpdateType.FixedUpdate, new List<ISequencer>() },
                    };
                }

                return Instance.t_threads;
            }
        }

        private Dictionary<SequenceUpdateType, List<ISequencer>> t_threads;

        private static Dictionary<ISequencer, List<object>> Dependances = new Dictionary<ISequencer, List<object>>();

        private static Dictionary<IStep, ISequencer> RunningSteps = new Dictionary<IStep, ISequencer>();

        [ShowNativeProperty]
        public int UpdateSequenceCount => Threads[SequenceUpdateType.Update].Count;
        [ShowNativeProperty]
        public int FixedUpdateSequenceCount => Threads[SequenceUpdateType.FixedUpdate].Count;
        [ShowNativeProperty]
        public int LateUpdateSequenceCount => Threads[SequenceUpdateType.LateUpdate].Count;

        protected override void Awake()
        {
            base.Awake();

            gameObject.name = "[Sequencer Manager]";
            if(Application.isPlaying)
                DontDestroyOnLoad(this);

        }

        // Update is called once per frame
        private void Update()
        {
            Clean();
            UpdateSequencers(SequenceUpdateType.Update);
        }
        private void FixedUpdate() => UpdateSequencers(SequenceUpdateType.FixedUpdate);

        private void LateUpdate() => UpdateSequencers(SequenceUpdateType.LateUpdate);

        internal static void UpdateSequencers(SequenceUpdateType updateThread)
        {
            ISequencer[] sequencers = Threads[updateThread].ToArray();
            for (int i = 0; i < sequencers.Length; i++)
            {
                ISequencer s = sequencers[i];
                if (Update(s))
                    EndStep(s);
            }
        }

        private static void Clean()
        {/*
            var roots = Threads.SelectMany(t => t.Value).ToList();

            List<IStep> stepToClean = new List<IStep>(RunningSteps.Count);
            foreach(var stepRelation in RunningSteps)
            {
                if (stepRelation.Key is ISequencer s && roots.Contains(s))
                    continue;

                if (stepRelation.Value == null)
                    stepToClean.Add(stepRelation.Key);
            }

            if(stepToClean.Count > 0)
            {
                LogError($"Cleaning {stepToClean.Count} because their parent is missing. You should not kill sequencers only with sequencer.Kill()");
                foreach (IStep step in stepToClean)
                {
                    RunningSteps.Remove(step);
                    if (step is ISequencer s)
                        Dependances.Remove(s);
                }
            }*/
        }

        internal static bool Update(ISequencer sequencer, ISequencer parent = null)
        {
            //avoid the error
            if (sequencer == null)
            {
                LogError("Null sequencer found. Deleting it...");
                return true;
            }

            try
            {
                //Starts the sequencer
                if (!sequencer.IsRunning())
                {
                    BeginStep(sequencer, parent);
                    if (parent != null)
                        AddDependances(sequencer, GetDependances(parent));
                }

                //If dependances are missing then the sequence is killed
                if (Dependances.TryGetValue(sequencer, out var dependances) && dependances.Any(d => d == null))
                    return true;

                //Step to update
                var step = sequencer.Current;
                if (step == null)
                    return sequencer.MoveToNext(parent);

                //Start Step
                if (!step.IsRunning())
                    BeginStep(step, sequencer);

                //If done
                if (step.Tick(sequencer))
                {
                    EndStep(step);
                    return sequencer.MoveToNext(parent);
                }

            }
            catch (Exception e)
            {
                LogError(e);
                //End step if an error is raised 
                return sequencer.MoveToNext(parent);
            }

            //Not finished yet
            return false;
        }

        internal static void Play(ISequencer sequencer, SequenceUpdateType thread, params object[] dependances)
        {
            try
            {
                if (sequencer == null)
                {
                    LogError("Trying to start a null sequencer");
                    return;
                }
                if (sequencer.IsRunning())
                {
                    LogError("Trying to start an already running Sequence", sequencer);
                    return;
                }

                Threads[thread].Add(sequencer);

                if(dependances.Length > 0)
                    AddDependances(sequencer, dependances);
                
                sequencer.OnStarted?.Invoke();

                Update(sequencer, null);
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        internal static void Stop(ISequencer sequencer)
        {
            EndSequencer(sequencer);
            EndStep(sequencer);
        }

        internal static void EndSequencer(ISequencer sequencer)
        {
            try
            {
                var instantiatedSteps = RunningSteps
                    .Where(kv => kv.Value == sequencer) //Children
                    .Select(kv => kv.Key)
                    .ToList();

                foreach (IStep step in instantiatedSteps)
                {
                    if (step is ISequencer s)
                        EndSequencer(s);
                    else
                        EndStep(step);
                }

                if (IsRoot(sequencer))
                {
                    foreach (var thread in Threads)
                    {
                        if (thread.Value.Remove(sequencer))
                            break;
                    }
                }

                Dependances.Remove(sequencer);
                sequencer.OnFinished?.Invoke();
            }
            catch (Exception e)
            {
                LogError(e);
            }
        }

        #region Setups

        /// <summary>
        /// Setup a step before running it
        /// </summary>
        /// <param name="step">Uninitialized step</param>
        /// <param name="parent"></param>
        static internal void BeginStep(IStep step, ISequencer parent)
        {
            if (step.IsRunning())
            {
                LogError($"Trying to begin {step.GetType().Name} but it's already running.", step);
                return;
            }

            RunningSteps.Add(step, parent);
            step.OnBegin();
        }

        /// <summary>
        /// Setup a Step before killing it
        /// </summary>
        /// <param name="step"> Ended step </param>
        static internal void EndStep(IStep step)
        {
            if (step == null)
            {
                LogError("Trying to stop a null step");
                return;
            }
            if (step.IsRunning())
            {
                RunningSteps.Remove(step);
                step.OnEnd();
            }
        }
        #endregion
        #region Roots
        static internal bool HasParent(IStep step) => RunningSteps.TryGetValue(step, out ISequencer parent) && parent != null;
     
        internal static ISequencer GetParent(IStep step) => RunningSteps[step];

        internal static ISequencer Root(IStep step)
        {
            if (!HasParent(step))
                return step as ISequencer;
            else
                return GetParent(step);
        }
        internal static bool IsRoot(ISequencer sequence) => !HasParent(sequence);
        #endregion

        #region States
        internal static bool IsRunning(IStep step) => RunningSteps.ContainsKey(step);

        #endregion

        #region Dependances
        static internal void AddDependances(ISequencer sequencer, object[] dependances)
        {
            if (!Dependances.ContainsKey(sequencer))
                Dependances.Add(sequencer, new List<object>());

            for (int i = 0; i < dependances.Length; i++)
            {
                object d = dependances[i];
                if (!Dependances[sequencer].Contains(d))
                    Dependances[sequencer].Add(d);
            }
        }
        static internal void RemoveDependances(ISequencer sequencer, object[] dependances)
        {
            if (!Dependances.ContainsKey(sequencer))
                return;

            for (int i = 0; i < dependances.Length; i++)
                Dependances[sequencer].Remove(dependances[i]);

            if (Dependances[sequencer].Count == 0)
                Dependances.Remove(sequencer);
        }

        static internal object[] GetDependances(ISequencer sequencer)
        {
            if (!Dependances.ContainsKey(sequencer))
                return new object[0];

            return Dependances[sequencer].ToArray();
        }

        #endregion

        #region Logs

        static internal void Log(string message, IStep step = null)
        {
            if (step != null)
                Debug.Log($"[Sequencer] Message : from {step} : {message}");
            else
                Debug.Log($"[Sequencer] Message : {message}");
        }

        static internal void LogError(string message, IStep step = null) => LogError(new Exception(message), step);

        static internal void LogError(Exception exception, IStep step = null)
        {
            if (step != null)
            {
                var newException = new Exception($"[Sequencer] Error from {step.GetType().Name} : {exception.Message}", exception);
                Debug.LogException(newException);
            }
            else
            {
                var newException = new Exception($"[Sequencer] : {exception.Message}", exception);
                Debug.LogException(newException);
            }
        }

        #endregion

        #region Publics
        public static void KillAllSequencers()
        {
            foreach (var thread in Threads)
            {
                ISequencer[] sequencers = thread.Value.ToArray();
                for (int i = 0; i < sequencers.Length; i++)
                {
                    ISequencer s = sequencers[i];
                    Stop(s);
                }
                thread.Value.Clear();
            }
            List<IStep> steps = new List<IStep>();

            foreach (var rs in RunningSteps)
                steps.Add(rs.Key);

            foreach (IStep step in steps)
                EndStep(step);

            Dependances.Clear();
            RunningSteps.Clear();

        }
        #endregion
        protected override void OnExistingInstanceFound(SequencerManager existintInstance)
        {
            Destroy(this);
        }

        private void OnDestroy()
        {
            //KillAllSequencers();
        }
    }
}