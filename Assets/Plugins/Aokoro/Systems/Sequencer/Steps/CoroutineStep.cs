using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Aokoro.Sequencing.Steps
{
    [Serializable]
    public class CoroutineStep : IStep
    {
        private class CoroutineRunner : Singleton<CoroutineRunner>
        {
            private Dictionary<object, MonoBehaviour> coroutines = new Dictionary<object, MonoBehaviour>();
            protected override void Awake() 
            {
                base.Awake();

                transform.SetParent(SequencerManager.Instance.transform);
            }
            private void Update()
            {
                if (coroutines.Count > 0)
                {
                    List<object> emptys = new List<object>();
                    foreach (var item in coroutines)
                    {
                        if (item.Value == null && !emptys.Contains(item.Key))
                            emptys.Add(item.Key);
                    }
                    foreach (var key in emptys)
                        coroutines.Remove(key);
                }
            }
            public void StartCoroutine(string routineName, MonoBehaviour from) => StartCoroutine(IRunCoroutine(routineName, from));
            public void StartCoroutine(IEnumerator routine, MonoBehaviour from) => StartCoroutine(IRunCoroutine(routine, from));
            internal bool IsCoroutineDone(object routine) => !coroutines.ContainsKey(routine);

            internal void StopCoroutine(string routineName, MonoBehaviour from)
            {
                if (routineName == null)
                    return;

                if (coroutines.Remove(routineName))
                    from?.StopCoroutine(routineName);
            }
            internal void StopCoroutine(IEnumerator routine, MonoBehaviour from)
            {
                if (routine == null)
                    return;

                if(coroutines.Remove(routine))
                    from?.StopCoroutine(routine);
            }

            private IEnumerator IRunCoroutine(IEnumerator routine, MonoBehaviour from)
            {
                coroutines.Add(routine, from);

                yield return from.StartCoroutine(routine);

                from.StopCoroutine(routine);
                coroutines.Remove(routine);
            }

            private IEnumerator IRunCoroutine(string routineName, MonoBehaviour from)
            {
                coroutines.Add(routineName, from);
                yield return from.StartCoroutine(routineName);

                from.StopCoroutine(routineName);
                coroutines.Remove(routineName);
            }

            protected override void OnExistingInstanceFound(CoroutineRunner existintInstance)
            {
                Destroy(gameObject);
            }
        }
        
        private readonly MonoBehaviour from;
        private readonly string routineName;
        private readonly Func<IEnumerator> getRoutine;


        private IEnumerator currentCoroutine;

        public CoroutineStep(string routineName, MonoBehaviour from)
        {
            this.from = from;
            this.routineName = routineName;
        }

        public CoroutineStep(Func<IEnumerator> getRoutine, MonoBehaviour from) : base()
        {
            this.from = from;
            this.getRoutine = getRoutine;
        }

        
        void IStep.OnBegin()
        {
            currentCoroutine = null;
            if (getRoutine != null)
            {
                currentCoroutine = getRoutine.Invoke();
                CoroutineRunner.Instance.StartCoroutine(currentCoroutine, from);
            }
            else
                CoroutineRunner.Instance.StartCoroutine(routineName, from);

        }
        void IStep.OnEnd()
        {
            currentCoroutine = null;
            CoroutineRunner.Instance.StopCoroutine(currentCoroutine, from);
        }



        bool IStep.Tick(ISequencer parent) => 
            CoroutineRunner.Instance.IsCoroutineDone(
                routineName == null ? 
                (object)currentCoroutine :
                (object)routineName );
    }
}
