using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using System;

namespace Aokoro.UI.ControlsDiplaySystem
{
    [System.Serializable]
    public struct CD_InputDisplay
    {
        public GameObject representation;
        [SerializeField] bool isComposite;
        [SerializeField] string[] matchPaths;

        public bool MatchesControl(CD_InputControl control)
        {
            if (control.IsComposite == isComposite)
            {
                foreach (var controlData in control)
                {
                    string path = (isComposite ? control.compositeType : controlData.Path).Trim().ToLower();
                    for (int j = 0; j < matchPaths.Length; j++)
                    {
                        if (path == matchPaths[j])
                            return true;
                    }
                }
            }
            return false;
        }
        public static bool operator ==(CD_InputDisplay c1, CD_InputDisplay c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(CD_InputDisplay c1, CD_InputDisplay c2)
        {
            return !c1.Equals(c2);
        }

#if UNITY_EDITOR
        public void Validate()
        {
            for (int j = 0; j < matchPaths.Length; j++)
                matchPaths[j] = matchPaths[j].Trim().ToLower();
        }

        public override bool Equals(object obj)
        {
            return obj is CD_InputDisplay input &&
                   EqualityComparer<GameObject>.Default.Equals(representation, input.representation) &&
                   EqualityComparer<string[]>.Default.Equals(matchPaths, input.matchPaths) ;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(representation, matchPaths);
        }
#endif
    }

    public class CD_InputRepresentation
    {

        public readonly CD_InputDisplay display;
        public readonly CD_InputControl control;

        public CD_InputRepresentation(CD_InputDisplay display, CD_InputControl control)
        {
            this.display = display;
            this.control = control;
        }
    }
}
