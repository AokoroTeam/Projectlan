using Michsky.UI.ModernUIPack;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Aokoro.UI
{
    [ExecuteInEditMode, DefaultExecutionOrder(-90), RequireComponent(typeof(Canvas))]
    public class UIManager : MonoBehaviour
    {
        public Canvas canvas;
        public static WindowManager MainUI;
        public Action OnUpdate;

        protected virtual void Awake()
        {
            canvas = GetComponent<Canvas>();
        }

        protected virtual void Update()
        {
            OnUpdate?.Invoke();
        }

        
    }
}