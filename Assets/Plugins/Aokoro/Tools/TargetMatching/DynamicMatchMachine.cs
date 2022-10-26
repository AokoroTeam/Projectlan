using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Aokoro.TargetMatching
{
    public class DynamicTargetMatchMachine : TargetMatchMachine
    {

        protected Func<AnimatorStateInfo, Vector3> getPos;
        protected Func<AnimatorStateInfo, Quaternion> getRot;



        public DynamicTargetMatchMachine(float startTime, float endTime, float stopTime, TargetBone TargetBone,
            Func<AnimatorStateInfo, Vector3> getPos) : base(startTime, endTime, stopTime, TargetBone, getPos(default))
        {
            this.getPos = getPos;
        }
        public DynamicTargetMatchMachine(float startTime, float endTime, float stopTime, TargetBone TargetBone,
            Func<AnimatorStateInfo, Vector3> getPos, Func<AnimatorStateInfo, Quaternion> getRot) : this(startTime, endTime, stopTime, TargetBone, getPos)
        {
            this.getRot = getRot;
            HasRot = true;
        }

        public DynamicTargetMatchMachine(float startTime, float endTime, TargetBone TargetBone,
            Func<AnimatorStateInfo, Vector3> getPos) : this(startTime, endTime, endTime, TargetBone, getPos)
        {
            HasRot = false;
        }
        public DynamicTargetMatchMachine(float startTime, float endTime, TargetBone TargetBone,
            Func<AnimatorStateInfo, Vector3> getPos, Func<AnimatorStateInfo, Quaternion> getRot) : this(startTime, endTime, endTime, TargetBone, getPos)
        {
            this.getRot = getRot;
            HasRot = true;
        }
        public DynamicTargetMatchMachine(TargetMatchProfile profile,
            Func<AnimatorStateInfo, Vector3> getPos) : this(profile.StartTime, profile.EndTime, profile.StopTime, profile.TargetBone, getPos)
        {
            HasRot = false;
        }
        public DynamicTargetMatchMachine(TargetMatchProfile profile,
            Func<AnimatorStateInfo, Vector3> getPos, Func<AnimatorStateInfo, Quaternion> getRot) : this(profile.StartTime, profile.EndTime, profile.StopTime, profile.TargetBone, getPos)
        {
            this.getRot = getRot;
            HasRot = true;
        }

        protected override Vector3 GetPosition(Animator anim, AnimatorStateInfo state)
        {
            try
            {
                targetPoint = getPos(state);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "/n" + e.StackTrace);
                return anim.transform.position;
            }

            return base.GetPosition(anim, state);
        }
        protected override Quaternion GetRotation(Quaternion currentRotation, AnimatorStateInfo infos)
        {
            try
            {
                targetRot = getRot(infos);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "/n" + e.StackTrace);
                return currentRotation;
            }

            return targetRot;
        }
    }
}
