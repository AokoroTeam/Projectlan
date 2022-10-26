using Aokoro.Entities.Player;
using UnityEngine;

namespace Realit.Reader.Player.Animations
{
    public abstract class PlayerStateMachineBehaviour<T> : StateMachineBehaviour where T : MonoBehaviour
    {
        protected static T playerComponent;

        protected static PlayerManager playerManager;

        private bool initiated = false;
        protected float CurrentTime { get; private set; }
        protected float CurrentFrame { get; private set; }
        protected float LastTime { get; private set; }
        protected float LastFrame { get; private set; }
        protected bool Exiting { get; private set; }
        protected bool Entring { get; private set; }

        protected Animator playerAnim;

        protected AnimatorStateInfo CurrentStateInfo { get; private set; }

        protected virtual void Initiate(Animator anim)
        {
            playerAnim = anim;
            initiated = true;
            if (!playerComponent)
                playerComponent = anim.GetComponent<T>();
            if (!playerManager)
                playerManager = anim.GetComponent<PlayerManager>();
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);

            if (!initiated)
                Initiate(animator);

            CurrentTime = 0;
            CurrentFrame = 0;
            LastTime = 0;
            LastFrame = 0;
        }

        protected virtual void Exit(bool OnStateExit)
        {

        }
        private static AnimatorClipInfo[] clips;
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            UpdateFrameInfos(animator, stateInfo, layerIndex);
        }

        protected void UpdateFrameInfos(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            bool isCurrent = animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash;

            LastFrame = CurrentFrame;
            LastTime = CurrentTime;

            if (isCurrent)
            {
                CurrentStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
                clips = animator.GetCurrentAnimatorClipInfo(layerIndex);

                Exiting = animator.IsInTransition(layerIndex);
                Entring = false;
            }
            else
            {
                CurrentStateInfo = animator.GetNextAnimatorStateInfo(layerIndex);
                clips = animator.GetNextAnimatorClipInfo(layerIndex);

                Exiting = false;
                Entring = animator.IsInTransition(layerIndex);
            }

            if (clips.Length > 0)
            {
                AnimatorClipInfo currentClip = clips[0];
                float normalizedTime = stateInfo.normalizedTime;
                CurrentTime = normalizedTime * stateInfo.length;

                CurrentFrame = Mathf.FloorToInt(normalizedTime * (currentClip.clip.length * currentClip.clip.frameRate));
            }
        }

        public bool IsFrame(int frame) => frame <= CurrentFrame && frame > LastFrame;
        public bool IsTime(int time) => time <= CurrentFrame && time > LastTime;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            Exit(true);
        }
    }
}