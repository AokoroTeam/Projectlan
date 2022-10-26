using Michsky.UI.ModernUIPack;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Aokoro.UI
{
    [ExecuteInEditMode, DefaultExecutionOrder(-90)]
    public class WindowManager : UIManager
    {
        public Transform WindowsParent;
        [SerializeField, Dropdown(nameof(windowsNames))]
        private string defaultWindow;
        public string DefaultWindow => defaultWindow;

        [SerializeField, ReadOnly]
        private Michsky.UI.ModernUIPack.WindowManager windowManager;

        private List<string> windowsNames() => windowManager != null ? windowManager.windows.Select(ctx => ctx.windowName).ToList() :
         new List<string>() { "No WindowManager" };

        private void OnValidate()
        {
            windowManager = GetComponent<Michsky.UI.ModernUIPack.WindowManager>();
        }

        protected override void Awake()
        {
            OnValidate();
            base.Awake();
        }

        private void Start()
        {
            if (!string.IsNullOrWhiteSpace(DefaultWindow))
                OpenWindow(defaultWindow);
        }

        public void OpenWindow(string windowName) => windowManager.OpenWindow(windowName);
        public void ShowCurrentWindow() => windowManager.ShowCurrentWindow();
        public void HideCurrentWindow() => windowManager.HideCurrentWindow();
        public Michsky.UI.ModernUIPack.WindowManager.WindowItem CurrentWindow() => windowManager.windows[windowManager.currentWindowIndex];
        public Michsky.UI.ModernUIPack.WindowManager.WindowItem AddWindow(string windowName, GameObject windowObject)
        {
            Michsky.UI.ModernUIPack.WindowManager.WindowItem window = new Michsky.UI.ModernUIPack.WindowManager.WindowItem();

            window.windowName = windowName;
            window.windowObject = windowObject;

            windowManager.windows.Add(window);

            return window;
        }

        public Michsky.UI.ModernUIPack.WindowManager.WindowItem GetWindow(string windowName)
        {
            int index = windowManager.windows.FindIndex(ctx => ctx.windowName == windowName);
            return index == -1 ? null : windowManager.windows[index];
        }
    }
}