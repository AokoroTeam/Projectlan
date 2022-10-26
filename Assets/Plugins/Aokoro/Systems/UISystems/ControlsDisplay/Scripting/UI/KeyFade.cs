using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Aokoro.UI.ControlsDiplaySystem
{
    public class KeyFade : UIItem
    {
        [SerializeField]
        private AnimationCurve fadeCurve;
        [SerializeField]
        private float fadeTime;
        [SerializeField]
        private float wait;

        private CanvasGroup[] groups;

        private float currentTime;
        private int currentIndex;

        protected override void OnUpdate()
        {
            currentTime += Time.deltaTime;
            int lastIndex = currentIndex == 0 ? groups.Length - 1 : currentIndex - 1;

            Debug.Log(currentTime);

            if (currentTime < fadeTime)
            {
                float advancement = Mathf.InverseLerp(0, fadeTime, currentTime);
                groups[currentIndex].alpha = fadeCurve.Evaluate(advancement);
                groups[lastIndex].alpha = fadeCurve.Evaluate(1 - advancement);
            }
            else if (currentTime < fadeTime + wait)
            {
                groups[currentIndex].alpha = 1;
                groups[lastIndex].alpha = 0;
            }
            else
            {
                currentTime = 0;
                currentIndex++;

                if (currentIndex > groups.Length - 1)
                    currentIndex = 0;
            }
        }

        private void Reboot()
        {
            for (int i = 0; i < groups.Length; i++)
                groups[i].alpha = i == 0 ? 1 : 0;

            currentIndex = 0;
            currentTime = 0;
        }

        protected void Awake()
        {
            groups = GetComponentsInChildren<CanvasGroup>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Reboot();
        }
    }
}
