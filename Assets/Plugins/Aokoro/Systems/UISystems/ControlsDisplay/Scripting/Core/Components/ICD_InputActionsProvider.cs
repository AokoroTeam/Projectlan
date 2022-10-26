using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.InputSystem;

namespace Aokoro.UI.ControlsDiplaySystem
{
    public interface ICD_InputActionsProvider
    {
        public InputAction[] GetInputActions();
        InputDevice[] GetDevices();
        public string GetControlScheme();
        public event Action OnActionsNeedRefresh;

    }
}
