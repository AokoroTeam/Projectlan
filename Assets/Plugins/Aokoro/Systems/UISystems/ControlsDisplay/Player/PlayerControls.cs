using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Aokoro.Entities.Player;
using Aokoro.Entities;
using System;

namespace Aokoro.UI.ControlsDiplaySystem
{
    public class PlayerControls : MonoBehaviour, ILateUpdateEntityComponent<PlayerManager>
    {
        string IEntityComponent.ComponentName => "PlayerControls";

        public event Action OnControlChanges;
        private PlayerInput playerInput;
        public PlayerManager Manager { get; set; }

        private InputActionMap lastMap;

        public void Initiate(PlayerManager manager)
        {
            playerInput = manager.playerInput;
        }
        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            if (playerInput.actions != null)
            {
                SetupInputDevices();
            }
        }


        public void SetupInputDevices()
        {
            InputDevice[] devices = InputSystem.devices.ToArray();
            playerInput.SwitchCurrentControlScheme(devices);
            lastMap = playerInput.currentActionMap;
        }

        private void OnEnable()
        {
            playerInput.onControlsChanged += OnControlsChanges;
        }

        private void OnDisable()
        {

            playerInput.onControlsChanged -= OnControlsChanges;
        }
        private void OnControlsChanges(PlayerInput playerInput)
        {
            Debug.Log("[Player] Controls have changed");
            OnControlChanges?.Invoke();
        }

        public void OnLateUpdate()
        {
            if (lastMap != playerInput.currentActionMap)
            {
                lastMap = playerInput.currentActionMap;
                OnControlsChanges(playerInput);
            }
        }
    }
}