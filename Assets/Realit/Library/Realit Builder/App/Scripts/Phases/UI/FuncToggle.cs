using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Realit.Builder.App.Edition.UI
{
    [AddComponentMenu("Realit/Builder/Edition/UI/Functionnality Toggle")]
    public class FuncToggle : MonoBehaviour
    {
        private Toggle toggle;
        [SerializeField]
        private EditionWindow windowManager;

        [SerializeField]
        Image icon;
        [SerializeField]
        Color selectedColor;
        [SerializeField]
        Color unselectedColor;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
        }

        public void OnValueChanges(bool value)
        {
            icon.color = value ? selectedColor : unselectedColor;
            ToggleGroup group = toggle.group;
            group.EnsureValidState();

            if (value)
                windowManager.SetupFunctionnalityFromToggle(toggle);
        }
    }
}