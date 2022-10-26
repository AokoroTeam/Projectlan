using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aokoro.TargetMatching;


namespace Aokoro.TargetMatching
{

    public class TargetMatchingManager : MonoBehaviour
    {
        public event Action<string> OnMatchTargetStarts;
        public event Action<string> OnMatchTargetEnds;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private TargetMatch currentTargetMatch = TargetMatch.Empty;

        public TargetMatch CurrentTargetMatch => currentTargetMatch;

        public void StartMatchTarget(string stateName, float transition, int layer, bool snapOnCancel = false, params TargetMatchMachine[] infos)
        {
            if (IsTargetMatching())
                CancelMatchTarget();

            OnMatchTargetStarts?.Invoke(stateName);
            currentTargetMatch = new TargetMatch(stateName, snapOnCancel, infos);
            animator.CrossFade(currentTargetMatch.stateNameHash, transition, layer);
        }

        public void UpdateMatchTarget()
        {
            if (IsTargetMatching())
            {
                if (currentTargetMatch.IsInCorrectState(animator, 0, out AnimatorStateInfo stateInfo))
                {
                    currentTargetMatch.Update(animator, stateInfo);
                    if (!currentTargetMatch.Done)
                        return;
                }

                CancelMatchTarget();
            }
        }

        public void CancelMatchTarget()
        {
            OnMatchTargetEnds?.Invoke(currentTargetMatch.stateName);

            currentTargetMatch.ForceStop(animator);
            //null

            currentTargetMatch = TargetMatch.Empty;
        }

        public bool IsTargetMatching() => !currentTargetMatch.Equals(TargetMatch.Empty);
    }
}
