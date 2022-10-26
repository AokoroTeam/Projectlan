using UnityEngine;

namespace Aokoro.UI.ControlsDiplaySystem
{
    [CreateAssetMenu(menuName = "Aokoro/UI/Inputs/DeviceControlsList")]
    internal class CD_Data : ScriptableObject
    {
        internal GameObject Or => or;
        internal GameObject And => and;

        [SerializeField]
        private GameObject or;
        [SerializeField]
        private GameObject and;
        [SerializeField]
        private CD_ControlScheme[] deviceControls;

        internal bool TryGetControlsForScheme(string schemeName, out CD_ControlScheme scheme)
        {
            for (int i = 0; i < deviceControls.Length; i++)
            {
                CD_ControlScheme cD_ControlScheme = deviceControls[i];

                if (cD_ControlScheme.ControlSchemeName == schemeName)
                {
                    scheme = cD_ControlScheme;
                    return true;
                }
            }

            scheme = default;
            return false;

        }

    }
}
