using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using System.Linq;
using System;

namespace Realit.Builder.App.Edition.UI
{
    public enum EditionFunctionality
    {
        Player,
        Lightning,
        Furnitures,
        Export,
        Delete,
        None,
    }

    [AddComponentMenu("Realit/Builder/Edition/UI/EditionWindow")]
    public class EditionWindow : MonoBehaviour, IPhaseComponent
    {
        /// <summary>
        /// First is from, second is to.
        /// </summary>
        public static event Action<EditionFunctionality, EditionFunctionality> OnFunctionnalityIsDisplayed;

        [System.Serializable]
        public struct EditionFunc
        {
            public EditionFunctionality functionality;
            public GameObject content;
            public Toggle toggle;
        }

        [SerializeField]
        private Inspector inspector;
        [SerializeField]
        private EditionFunc[] functionalities;

        public AssetReference playerAssetReference;

        private EditionFunctionality functionality;

        public Phase phase => throw new NotImplementedException();

        private void Start()
        {
            functionality = EditionFunctionality.None;
            SetupFunctionnality(EditionFunctionality.Player, true);
        }

        //Called from toggles
        public void SetupFunctionnalityFromToggle(Toggle toggle) => SetupFunctionnality(functionalities.FirstOrDefault(ctx => ctx.toggle == toggle).functionality);

        //Changes the displayed functionnality and setups up the environnement accordingly.
        public void SetupFunctionnality(EditionFunctionality to, bool force = false)
        {
            if (force || this.functionality != to)
            {
                EditionFunctionality from = this.functionality;
                functionality = to;
                EditionFunc func = functionalities.FirstOrDefault(ctx => ctx.functionality == to);

                inspector.TryOpen();
                inspector.DisplayFunctionnality(to, func.content);
                OnFunctionnalityIsDisplayed?.Invoke(from, to);
            }
        }
    }
}