using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.UI.ControlsDiplaySystem
{
    public struct CD_Command : IEnumerable<CD_InputCombination>
    {
        public string actionName;

        private List<CD_InputCombination> combinations;
        public int CombinationsCount => combinations.Count;


        public CD_InputCombination this[int i] => combinations[i];
        public CD_Command(string actionName)
        {
            this.actionName = actionName;
            combinations = new List<CD_InputCombination>();
        }

        
        public void Addcombination(params CD_InputRepresentation[] inputs) => this.combinations.Add(inputs);
        public void Addcombination(int range, CD_InputRepresentation[] inputs)
        {
            CD_InputRepresentation[] shortenInputs = new CD_InputRepresentation[range];
            for (int i = 0; i < range; i++)
                shortenInputs[i] = inputs[i];

            combinations.Add(shortenInputs);
        }

        IEnumerator<CD_InputCombination> IEnumerable<CD_InputCombination>.GetEnumerator() => combinations.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable).GetEnumerator();


    }
    public struct CD_InputCombination : IEnumerable<CD_InputRepresentation>
    {
        private CD_InputRepresentation[] inputs;
        public CD_InputCombination(CD_InputRepresentation[] matchedInputs)
        {
            inputs = matchedInputs;
        }
        public CD_InputRepresentation this[int i] => inputs[i];
        public int Length => inputs != null ? inputs.Length : 0;

        IEnumerator<CD_InputRepresentation> IEnumerable<CD_InputRepresentation>.GetEnumerator() => inputs.GetEnumerator() as IEnumerator<CD_InputRepresentation>;

        IEnumerator IEnumerable.GetEnumerator() => (this as IEnumerable).GetEnumerator();


        public static implicit operator CD_InputCombination(CD_InputRepresentation[] inputs) => new CD_InputCombination(inputs);
    }
}
