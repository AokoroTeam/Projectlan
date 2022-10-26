using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;

namespace Aokoro.UI.ControlsDiplaySystem.UI
{
    public abstract class ControlIcon : MonoBehaviour
    {
        public abstract void SetupIcon(CD_InputControl control);

    }
}
