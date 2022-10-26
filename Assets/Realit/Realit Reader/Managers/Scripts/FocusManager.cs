using Aokoro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Realit.Reader.Managers
{
    public class FocusManager : Singleton<FocusManager>
    {
        private CanvasGroup canvasGroup;

        public InfluencedProperty<CursorLockMode> cursorLockMode = new InfluencedProperty<CursorLockMode>();
        public InfluencedProperty<bool> cursorVisibility = new InfluencedProperty<bool>();

        protected override void Awake()
        {
            base.Awake();
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void OnFocusLost()
        {
            cursorLockMode.Subscribe(this, PriorityTags.Highest, CursorLockMode.Confined);
            cursorVisibility.Subscribe(this, PriorityTags.Highest, true);

            canvasGroup.interactable = true;
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            
            Time.timeScale = 0;

#if UNITY_WEBG && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = false;
#endif
        }


        public void OnFocusRegained()
        {
            cursorLockMode.Unsubscribe(this);
            cursorVisibility.Unsubscribe(this);

            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;

            Time.timeScale = 1;

#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = true;            //Screen.fullScreen = true;
#endif
        }

        public void OnFocusRegainedAnimation()
        {
            Invoke(nameof(OnFocusRegained), .5f);
        }

        // This function will be called from the webpage
        public void FocusCanvas(string p_focus)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
    if (p_focus == "0") {
        OnFocusLost();
    } else {
        OnFocusRegained();
    }
#endif
        }

        private void CursorVisibility_OnValueChanged(bool value, object key) => Cursor.visible = value;
        private void CursorLockMode_OnValueChanged(CursorLockMode value, object key) => Cursor.lockState = value;

        private void OnEnable()
        {
            cursorLockMode.OnValueChanged += CursorLockMode_OnValueChanged;
            cursorVisibility.OnValueChanged += CursorVisibility_OnValueChanged;
        }

        private void OnDisable()
        {
            cursorLockMode.OnValueChanged -= CursorLockMode_OnValueChanged;
            cursorVisibility.OnValueChanged -= CursorVisibility_OnValueChanged;
        }

        protected override void OnExistingInstanceFound(FocusManager existingInstance)
        {
            Destroy(gameObject);
        }
    }
}