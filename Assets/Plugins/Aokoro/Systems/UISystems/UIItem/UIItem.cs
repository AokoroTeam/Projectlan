using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.UI
{
    public abstract class UIItem : MonoBehaviour
    {
        protected UIManager uIManager;
        protected Canvas canvas => uIManager.canvas;

        [SerializeField, BoxGroup("UI item")]
        bool hasUpdate = true;

        protected virtual void Awake()
        {
            uIManager = GetComponentInParent<UIManager>();
            if (uIManager == null)
                Destroy(this);
        }

        protected virtual void OnEnable()
        {
            if (hasUpdate)
            {
                if (uIManager != null)
                    uIManager.OnUpdate += OnUpdate;
            }
        }
        protected virtual void OnDisable()
        {
            if (hasUpdate)
            {
                if (uIManager != null)
                    uIManager.OnUpdate -= OnUpdate;
            }
        }

        protected abstract void OnUpdate();
    }
}