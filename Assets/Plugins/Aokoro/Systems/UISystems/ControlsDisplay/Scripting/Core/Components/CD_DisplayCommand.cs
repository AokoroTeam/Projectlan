using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace Aokoro.UI.ControlsDiplaySystem
{
    public class CD_DisplayCommand : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI description;
        [SerializeField]
        private Transform combinationsParents;
        [SerializeField]
        private GameObject combinationLayout;

        private CD_DisplayCombination[] instantiatedCombinations;
        private List<GameObject> ands = new List<GameObject>();
        private List<GameObject> ors = new List<GameObject>();
        private bool or, and;

        public virtual void Fill(CD_Command command, bool withOr = true, bool withAnd = true)
        {
            or = withOr;
            and = withAnd;

            description.text = command.actionName;

            Clear();
            instantiatedCombinations = new CD_DisplayCombination[command.CombinationsCount];
            CreateCommandDisplays(command, instantiatedCombinations);
        }


        protected virtual void CreateCommandDisplays(CD_Command command, CD_DisplayCombination[] output)
        {
            for (int i = 0; i < command.CombinationsCount; i++)
            {
                if (i > 0)
                    Or(combinationsParents);

                CD_InputCombination combination = command[i];
                output[i] = CreateCombinationDisplays(combination);
            }
        }

        protected virtual CD_DisplayCombination CreateCombinationDisplays(CD_InputCombination combination)
        {
            CD_DisplayCombination display = Instantiate(combinationLayout, combinationsParents)
                .GetComponent<CD_DisplayCombination>();

            display.Fill(combination, this);

            return display;
        }

        public virtual void And(Transform root)
        {
            if (and)
                ands.Add(Instantiate(ControlsDiplaySystem.Data.And, root));
        }

        public virtual void Or(Transform root)
        {
            if (or)
                ors.Add(Instantiate(ControlsDiplaySystem.Data.Or, root));
        }


        public void Clear()
        {
            if (instantiatedCombinations != null)
            {
                for (int i = 0; i < instantiatedCombinations.Length; i++)
                {

                    CD_DisplayCombination cD_DisplayCombination = instantiatedCombinations[i];
                    if (cD_DisplayCombination)
                    {
                        cD_DisplayCombination.Clear();
                        Destroy(cD_DisplayCombination.gameObject);
                    }
                }

                instantiatedCombinations = null;
            }
            if (ors != null)
            {
                foreach (var or in ors)
                    Destroy(or);
                ors.Clear();
            }
            if (ands != null)
            {

                foreach (var and in ands)
                    Destroy(and);
                ands.Clear();
            }

        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}
