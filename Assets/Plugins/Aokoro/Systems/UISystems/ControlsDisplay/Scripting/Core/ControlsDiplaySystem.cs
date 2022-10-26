using UnityEngine;
using System;
using System.IO;
using System.Linq;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem.Utilities;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Aokoro.UI.ControlsDiplaySystem
{
    public static class ControlsDiplaySystem
    {

        private static CD_Data _data;
        internal static CD_Data Data
        {
            get
            {
                if (_data == null)
                {
                    _data = Resources.Load<CD_Data>("DeviceControlsList");
#if UNITY_EDITOR
                    if (_data == null)
                    {
                        _data = ScriptableObject.CreateInstance<CD_Data>();
                        AssetDatabase.CreateAsset(_data, Path.Combine("Resources", "DeviceControllist"));
                    }
#endif
                }

                return _data;
            }
        }

        public static bool GetControlsForControlScheme(string controlScheme, out CD_ControlScheme scheme) => Data.TryGetControlsForScheme(controlScheme, out scheme);


        public static CD_Command[] ExtractCommands(CD_InputAction[] actions, CD_ControlScheme controlScheme, InputDevice[] devices, CD_Settings settings)
        {
            int length = actions.Length;
            CD_Command[] commands = new CD_Command[actions.Length];

            for (int i = 0; i < length; i++)
            {
                int skipBindingCount = 0;
                //System representation of actions
                CD_InputAction cd_action = actions[i];
                //Action to work with
                InputAction action = cd_action.action;
                //Create the command that will represent the action and the controls associated to it
                CD_Command command = new CD_Command(cd_action.settings.outputName);

                //Going through all bindings inside of the action
                var bindings = action.bindings;
                int bindingCount = bindings.Count;

                for (int j = 0; j < bindingCount; j++)
                {
                    if (!cd_action.settings.IsBindingRequested(command.CombinationsCount + 1 + skipBindingCount))
                    {
                        skipBindingCount++;
                        continue;
                    }

                    InputBinding binding = bindings[j];
                    //Only the final path is intresting
                    string effectivePath = binding.effectivePath;

                    //If so, skip because it is already used later on
                    if (binding.isPartOfComposite)
                        continue;

                    //If the binding is itself a combination of multiple bindings
                    if (binding.isComposite)
                    {
                        CD_InputControl composite = ExtractCompositeControls(devices, bindings, j);

                        //Modifiers
                        if (composite.compositeType is "OneModifier" or "TwoModifier")
                        {
                            CD_InputRepresentation[] representations = new CD_InputRepresentation[composite.Lenght];
                            int representationLenght = controlScheme.GetInputRepresentationsFromControls(composite.Split(), representations);
                            command.Addcombination(representationLenght, representations);
                        }
                        //Axis etc...
                        else
                        {
                            CD_InputRepresentation representation = controlScheme.GetInputRepresentationFromControl(composite);
                            command.Addcombination(representation);
                        }
                    }

                    else if (TryGetControlPathsFromBinding(devices, effectivePath, out string controlPath, out string displayName, out InputDevice device))
                    {
                        CD_InputControl control = new CD_InputControl(controlPath, displayName, device.displayName);
                        CD_InputRepresentation representation = controlScheme.GetInputRepresentationFromControl(control);
                        command.Addcombination(representation);
                    }
                }

                commands[i] = command;
            }

            return commands;
        }

        private static CD_InputControl ExtractCompositeControls(InputDevice[] devices, ReadOnlyArray<InputBinding> bindings, int index)
        {

            CD_InputControl composite = new CD_InputControl(bindings[index].GetNameOfComposite());

            //Composites parts are after the Composite
            while (true)
            {
                index++;
                if (index >= bindings.Count)
                    break;

                var compositeBinding = bindings[index];

                //Out of the composite
                if (!compositeBinding.isPartOfComposite)
                    break;

                if (TryGetControlPathsFromBinding(devices, compositeBinding.effectivePath, out string compositeControlPath, out string compositeDisplayName, out InputDevice device))
                    composite.AddControl(compositeControlPath, compositeDisplayName, device.displayName);
            }
            return composite;
        }

        private static bool TryGetControlPathsFromBinding(InputDevice[] devices, string bindingPath, out string controlPath, out string displayName, out InputDevice associatedDevice)
        {
            displayName = string.Empty;
            controlPath = string.Empty;
            associatedDevice = null;

            foreach (var device in devices)
            {
                var control = InputControlPath.TryFindControl(device, bindingPath);
                if (control != null)
                {
                    displayName = InputControlPath.ToHumanReadableString(control.path,
                        out string deviceLayoutName,
                        out controlPath,
                        InputControlPath.HumanReadableStringOptions.OmitDevice,
                        device);

                    associatedDevice = device;
                    /*Debug.Log($"Display name : {displayName} | ControlPath : {controlPath} | Device Layout : {deviceLayoutName}");
                    Debug.Log($"{control.path} | {control.name} | {control.variants}  | {control.shortDisplayName} ");*/
                    return true;
                }
            }

            return false;
        }

        public static CD_InputAction[] SelectInputActions(InputAction[] actions, CD_Settings settings)
        {
            if (!settings.HasValue)
                return SelectInputactions(actions);
            else
                return settings.ConvertInputSystemActions(actions);

        }
        public static CD_InputAction[] SelectInputactions(InputAction[] actions)
        {
            CD_InputAction[] cD_InputActions = new CD_InputAction[actions.Length];
            for (int i = 0; i < actions.Length; i++)
            {
                InputAction action = actions[i];
                cD_InputActions[i] = new CD_InputAction(action.name, action);
            }

            return cD_InputActions;
        }
    }
}
