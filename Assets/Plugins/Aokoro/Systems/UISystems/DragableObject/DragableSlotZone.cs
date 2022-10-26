using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Aokoro.UI
{
    public class DragableSlotZone : MonoBehaviour
    {
        RectTransform t;
        private void Awake()
        {
            t = transform as RectTransform;
        }

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(t, sp, eventCamera);
        }
    }
}