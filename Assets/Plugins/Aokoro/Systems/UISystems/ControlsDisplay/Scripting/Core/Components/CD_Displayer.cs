using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.InputSystem;
using NaughtyAttributes;

namespace Aokoro.UI.ControlsDiplaySystem
{
    public class CD_Displayer : MonoBehaviour
    {
        [SerializeField, Required]
        GameObject CommandLayout;

        [SerializeField, Required]
        Transform root;
        public CD_Settings actionSettings;

        [SerializeField, ReadOnly]
        private List<CD_DisplayCommand> displays = new List<CD_DisplayCommand>();

        [SerializeField, RequireInterface(typeof(ICD_InputActionsProvider))]
        private UnityEngine.Object _actionProviderReference;

        private ICD_InputActionsProvider _actionProvider;
        private ICD_InputActionsProvider ActionProvider
        {
            get
            {
                if (_actionProvider == null && _actionProviderReference != null)
                    _actionProvider = _actionProviderReference as ICD_InputActionsProvider;

                return _actionProvider;
            }
            set => _actionProvider = value;
        }

        private void Awake()
        {
            //Ensure that on creation, everything is ready
            Clean();
        }

        private void OnEnable()
        {
            if (ActionProvider != null)
                ActionProvider.OnActionsNeedRefresh += Refresh;
        }

        private void OnDisable()
        {
            if (ActionProvider != null)
                ActionProvider.OnActionsNeedRefresh -= Refresh;
        }


        public void AssignActionProvider(ICD_InputActionsProvider value, bool triggerRefresh = true)
        {
            if (ActionProvider != value)
            {
                if (ActionProvider != null)
                {
                    ActionProvider.OnActionsNeedRefresh -= Refresh;
                    if (triggerRefresh)
                        Refresh();
                }

                ActionProvider = value;
                ActionProvider.OnActionsNeedRefresh += Refresh;
            }
        }

        public void Show()
        {
            root.gameObject.SetActive(true);
            Refresh();
        }

        public void Hide()
        {
            root.gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        [Button("Simulate", EButtonEnableMode.Editor)]
        private void Simulate()
        {
            Refresh();
        }
#endif
        public void Refresh()
        {
            Clean();

            InputAction[] inputActions = ActionProvider.GetInputActions();
            InputDevice[] devices = ActionProvider.GetDevices();
            string schemeName = ActionProvider.GetControlScheme();

            if (ControlsDiplaySystem.GetControlsForControlScheme(schemeName, out CD_ControlScheme scheme))
            {

                CD_InputAction[] actions = ControlsDiplaySystem.SelectInputActions(inputActions, actionSettings);
                CD_Command[] commands = ControlsDiplaySystem.ExtractCommands(actions, scheme, devices, actionSettings);

                for (int i = 0; i < commands.Length; i++)
                {
                    CD_DisplayCommand displayer = GameObject.Instantiate(CommandLayout, root).GetComponent<CD_DisplayCommand>();
                    displays.Add(displayer);
                    displayer.Fill(commands[i]);
                }
            }
            else
            {
                Debug.Log($"No compatible scheme for {schemeName}");
                Hide();
            }

            //Debug.Log("[Control Display] Controls have been successfuly updated", gameObject);
            //Canvas.ForceUpdateCanvases();
        }

        private void Clean()
        {
            if (displays == null)
                displays = new List<CD_DisplayCommand>();
            else
            {
                foreach (var display in displays)
                    Destroy(display.gameObject);

                displays.Clear();
            }
        }
    }
}
