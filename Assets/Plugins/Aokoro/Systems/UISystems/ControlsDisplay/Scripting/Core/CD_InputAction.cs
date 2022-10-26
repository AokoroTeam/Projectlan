using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using static Aokoro.UI.ControlsDiplaySystem.CD_Settings;

namespace Aokoro.UI.ControlsDiplaySystem
{
    public readonly struct CD_InputAction
    {
        public readonly CD_ActionSettings settings;
        public readonly InputAction action;

        public CD_InputAction(CD_ActionSettings settings, InputAction action)
        {
            this.settings = settings;
            this.action = action;
        }
        public CD_InputAction(string name, InputAction action)
        {
            this.settings = new CD_ActionSettings(name);
            this.action = action;
        }
        public static implicit operator InputAction(CD_InputAction _InputActions) => _InputActions.action;
    }
}
