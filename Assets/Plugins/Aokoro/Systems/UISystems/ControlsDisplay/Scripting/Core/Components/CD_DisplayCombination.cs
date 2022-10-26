using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Aokoro.UI.ControlsDiplaySystem.UI;

namespace Aokoro.UI.ControlsDiplaySystem
{
    public class CD_DisplayCombination : MonoBehaviour
    {
        [SerializeField]
        private Transform root;
        private GameObject[] instantiatedInputs;
        public virtual void Fill(CD_InputCombination combination, CD_DisplayCommand commandDisplayer)
        {
            int length = combination.Length;
            instantiatedInputs = new GameObject[length];
            for (int i = 0; i < length; i++)
            {
                if (i != 0)
                    commandDisplayer.And(root);

                var matchedInput = combination[i];
                instantiatedInputs[i] = GameObject.Instantiate(matchedInput.display.representation, root);
                if (instantiatedInputs[i].TryGetComponent(out ControlIcon icon))
                    icon.SetupIcon(matchedInput.control);

            }
        }
        public void Clear()
        {
            if (instantiatedInputs == null)
                return;

            for (int i = 0; i < instantiatedInputs.Length; i++)
                Destroy(instantiatedInputs[i].gameObject);

            instantiatedInputs = null;
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}
