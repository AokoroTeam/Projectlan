using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Aokoro.TargetMatching
{
    [Serializable]
    public struct TargetMatch
    {
        public string stateName;
        public TargetMatchMachine[] targetMatches;
        public  int stateNameHash;
        private bool snapOnCancel;

        private int currentTargetMatch;
        public bool Done { get; private set; }


        public TargetMatch(string stateName, bool snapOnCancel = false,params TargetMatchMachine[] targetMatches)
        {
            this.currentTargetMatch = 0;
            this.Done = false;
            this.stateName = stateName;
            this.stateNameHash = Animator.StringToHash(stateName);
            this.snapOnCancel = snapOnCancel;

            var targetMatchesList = new List<TargetMatchMachine>(targetMatches);
            targetMatchesList.Sort((x, y) => x.StopTime.CompareTo(y.StopTime));
            this.targetMatches = targetMatchesList.ToArray();
        }

        public Vector3 CurrentTargetPosition => targetMatches[currentTargetMatch].targetPoint;

        public bool IsInCorrectState(Animator animator, int layer, out AnimatorStateInfo stateInfo)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(layer);

            if (!stateInfo.IsName(stateName))
            {
                stateInfo = animator.GetNextAnimatorStateInfo(layer);
                if (!stateInfo.IsName(stateName))
                    return false;
            }

            return true;
        }

        public void Start(Animator animator, float transition, int layer)
        {
            animator.CrossFadeInFixedTime(stateNameHash, transition, layer);
        }
        public void Update(Animator animator, AnimatorStateInfo stateInfos)
        {
            if (Done)
                return;

            var targetMatch = targetMatches[currentTargetMatch];
            targetMatch.ComputeTargetMatching(animator, stateInfos);

            if (targetMatch.Done)
                currentTargetMatch++;

            if (currentTargetMatch >= targetMatches.Length)
            {
                currentTargetMatch = targetMatches.Length - 1;
                Done = true;
            }
        }

        public static TargetMatch Empty => new TargetMatch();

        public void ForceStop(Animator anim)
        {/*
            if(!Done && snapOnCancel)
            {
                Vector3 pos = targetMatches[targetMatches.Length - 1].GetTargetPosition(anim, 0);
                Debug.DrawLine(pos, anim.transform.position, Color.green);
                Debug.DrawRay(pos, anim.targetPosition, Color.red);

                anim.transform.position = pos;
                Done = true;
            }*/
        }
    }
}
