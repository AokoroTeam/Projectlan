using System;
using UnityEngine;


namespace Aokoro.TargetMatching
{
    [System.Serializable]

    public enum TargetBone
    {
        RightHand,
        LeftHand,
        BothHand,
        RightFoot,
        LeftFoot,
        BothFoot,
        Root,
    }
    [System.Serializable]
    public struct TargetMatchProfile
    {
        [Range(0,1)]
        public float StartTime;
        [Range(0,1)]
        public float EndTime;
        [Range(0,1)]
        public float StopTime;
        public TargetBone TargetBone;
    }
    public class TargetMatchMachine
    {
        public readonly float StartTime;
        public readonly float EndTime;
        public readonly float StopTime;

        protected readonly TargetBone TargetBone;
        protected Vector3 startPos;
        public Vector3 targetPoint;

        protected Vector3 boneToRoot;

        protected bool HasRot;
        protected Quaternion startRot;
        public Quaternion targetRot;

        private bool firstPass;


        public TargetMatchMachine(float startTime, float endTime, float stopTime, TargetBone TargetBone, Vector3 targetPos)
        {
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.StopTime = stopTime;
            this.TargetBone = TargetBone;
            this.targetPoint = targetPos;

            Done = false;
            firstPass = true;
        }
        public TargetMatchMachine(float startTime, float endTime, float stopTime, TargetBone TargetBone, Vector3 targetPos, Quaternion targetRot) : this(startTime, endTime, stopTime, TargetBone, targetPos)
        {
            this.targetRot = targetRot;
            HasRot = true;
        }

        public TargetMatchMachine(float startTime, float endTime, TargetBone TargetBone, Vector3 targetPos) : this(startTime, endTime, endTime, TargetBone, targetPos)
        {
            HasRot = false;
        }
        public TargetMatchMachine(float startTime, float endTime, TargetBone TargetBone, Vector3 targetPos, Quaternion targetRot) : this(startTime, endTime, endTime, TargetBone, targetPos)
        {
            this.targetRot = targetRot;
            HasRot = true;
        }

        public TargetMatchMachine(TargetMatchProfile profile, Vector3 target) : this(profile.StartTime, profile.EndTime, profile.StopTime, profile.TargetBone, target)
        {
            HasRot = false;
        }
        public TargetMatchMachine(TargetMatchProfile profile, Vector3 target, Quaternion targetRot) : this(profile.StartTime, profile.EndTime, profile.StopTime, profile.TargetBone, target)
        {
            this.targetRot = targetRot;
            HasRot = true;
        }


        //Updates animator position with targetMatching
        public void ComputeTargetMatching(Animator anim, AnimatorStateInfo currentStateInfo)
        {
            float normalizedTime = currentStateInfo.normalizedTime;
            if (HasMatch(normalizedTime))
            {
                //Debug.DrawRay(anim.transform.position, Vector3.up, Color.green);
                //Debug.DrawLine(anim.transform.position, position, Color.Lerp(Color.white, Color.red, currentStateInfo.normalizedTime), 1);
                anim.transform.position = GetPosition(anim, currentStateInfo);

                if (HasRot && firstPass)
                    anim.transform.rotation = GetRotation(anim.transform.rotation, currentStateInfo);

                   // anim.transform.rotation = GetRotation(anim.transform.rotation, normalizedTime);
                firstPass = false;
            }
        }
        public bool Done { get; private set; }


        protected virtual Quaternion GetRotation(Quaternion currentRotation, AnimatorStateInfo state)
        {
            float time = state.normalizedTime;

            if (time <= EndTime)
            {
                if (firstPass)
                    startRot = currentRotation;

                float localNormTime = Mathf.InverseLerp(StartTime, EndTime, time);
                return Quaternion.Slerp(startRot, targetRot, localNormTime);
            }
            else if (time <= StopTime)
            {
                return targetRot;
            }
            else
                return currentRotation;
        }
                //Compute offset
        protected virtual Vector3 GetPosition(Animator anim, AnimatorStateInfo state)
        {
            float time = state.normalizedTime;
            //Smooth
            if (time <= EndTime)
            {
                if (firstPass)
                {
                    startPos = anim.rootPosition;

                    Vector3 targetBonePosition;

                    if (TargetBone != TargetBone.BothHand && TargetBone != TargetBone.BothFoot)
                    {
                        anim.SetTarget(GetAvatarTarget(TargetBone), EndTime);
                        anim.Update(0);
                        targetBonePosition = anim.targetPosition;
                    }
                    else
                    {
                        TargetBone right = TargetBone == TargetBone.BothFoot ? TargetBone.RightFoot : TargetBone.RightHand;
                        TargetBone left = TargetBone == TargetBone.BothFoot ? TargetBone.LeftFoot : TargetBone.LeftHand;

                        anim.SetTarget(GetAvatarTarget(right), EndTime);
                        anim.Update(0);
                        Vector3 rightPosition = anim.targetPosition;

                        anim.SetTarget(GetAvatarTarget(left), EndTime);
                        anim.Update(0);
                        Vector3 leftPosition = anim.targetPosition;

                        targetBonePosition = (rightPosition + leftPosition) / 2;
                    }
                    anim.SetTarget(AvatarTarget.Root, EndTime);
                    anim.Update(0);
                    Vector3 endRootPosition = anim.targetPosition;

                    boneToRoot = endRootPosition - targetBonePosition;
                }

                Vector3 rootEndPosition = targetPoint + boneToRoot;
                float localNormTime = Mathf.InverseLerp(StartTime, EndTime, time);

                Vector3 smoothedPosition = Vector3.Lerp(startPos, rootEndPosition, localNormTime);
                
               // Debug.DrawLine(startPos, rootTargetPosition, Color.cyan);
                return smoothedPosition;
            }
            else if (time <= StopTime)
            {
                Vector3 memberPosition;

                if (TargetBone != TargetBone.BothHand && TargetBone != TargetBone.BothFoot)
                {
                    memberPosition = anim.GetBoneTransform(GetBodyBone(TargetBone)).position;
                }
                else
                {
                    TargetBone right = TargetBone == TargetBone.BothFoot ? TargetBone.RightFoot : TargetBone.RightHand;
                    TargetBone left = TargetBone == TargetBone.BothFoot ? TargetBone.LeftFoot : TargetBone.LeftHand;

                    Vector3 rightPosition = anim.GetBoneTransform(GetBodyBone(right)).position;
                    Vector3 leftPosition = anim.GetBoneTransform(GetBodyBone(left)).position;

                    memberPosition = (rightPosition + leftPosition) / 2;
                }
                //Bone offset
                Vector3 offset = anim.transform.position - memberPosition;

                return targetPoint + offset;
            }
            else
                return anim.rootPosition;
        }

        //Is the targetMatching still effective?
        protected bool HasMatch(float currentNormTime) => !Done && currentNormTime <= StopTime && currentNormTime >= StartTime;

        #region Utility
        public static AvatarTarget GetAvatarTarget(TargetBone bone)
        {
            switch (bone)
            {
                case TargetBone.RightFoot:
                    return AvatarTarget.RightFoot;
                case TargetBone.LeftFoot:
                    return AvatarTarget.LeftFoot;
                case TargetBone.RightHand:
                    return AvatarTarget.RightHand;
                case TargetBone.LeftHand:
                    return AvatarTarget.LeftHand;
                case TargetBone.Root:
                    return AvatarTarget.Root;
                default:
                    return AvatarTarget.Body;

            }
        }

        public static HumanBodyBones GetBodyBone(TargetBone bone)
        {
            switch (bone)
            {
                case TargetBone.RightFoot:
                    return HumanBodyBones.RightFoot;
                case TargetBone.LeftFoot:
                    return HumanBodyBones.LeftFoot;
                case TargetBone.RightHand:
                    return HumanBodyBones.RightHand;
                case TargetBone.LeftHand:
                    return HumanBodyBones.LeftHand;
                default:
                    return HumanBodyBones.Hips;
            }
        }
        #endregion
    }

}
