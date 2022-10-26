
using UnityEngine;
using UnityEngine.InputSystem;

namespace Aokoro.UI.ControlsDiplaySystem
{
    [System.Serializable]
    public struct CD_ControlScheme
    {
        public string ControlSchemeName => controlSchemeName;

        [SerializeField] private string controlSchemeName;
        [SerializeField] private CD_DeviceControls[] controls;

        public CD_InputRepresentation GetInputRepresentationFromControl(CD_InputControl control)
        {
            for (int i = 0; i < controls.Length; i++)
            {
                if (InputSystem.IsFirstLayoutBasedOnSecond(control.Device, controls[i].DeviceName))
                    return controls[i].GetInputRepresentationFromControl(control);
            }

            return null;
        }

        internal int GetInputRepresentationsFromControls(CD_InputControl[] controls, CD_InputRepresentation[] output)
        {
            int size = 0;
            for (int i = 0; i < controls.Length; i++)
            {
                CD_InputRepresentation representation = GetInputRepresentationFromControl(controls[i]);
                if (representation != null)
                {
                    output[size] = representation;
                    size++;
                }
            }

            return size;
        }
    }
}