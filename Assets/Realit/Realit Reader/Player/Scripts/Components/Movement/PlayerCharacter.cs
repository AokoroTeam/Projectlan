using EasyCharacterMovement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Aokoro.Entities;
using Aokoro.Entities.Player;
using Aokoro.UI.ControlsDiplaySystem;

using Realit.Reader.Managers;

using UnityEngine.InputSystem;
using NaughtyAttributes;
using Aokoro;

namespace Realit.Reader.Player.Movement
{
    [AddComponentMenu("Realit/Reader/Player/Movement/PlayerCharacter")]
    public class PlayerCharacter : Character, IEntityComponent<PlayerManager>, IPlayerInputAssetProvider, ICD_InputActionsProvider
    {
        [ReadOnly]
        public PlayerMovementUI UI;

        public InfluencedProperty<bool> Freezed;

        public PlayerManager Manager { get; set; }
        public InputActionAsset ActionAsset { get => inputActions; set => inputActions = value; }

        string IEntityComponent.ComponentName => "PlayerCharacter";

        public event Action OnActionsNeedRefresh;

        private PlayerControls playerControls;


        #region Unity Events
        protected override void OnAwake()
        {
            base.OnAwake();
            playerControls = GetComponent<PlayerControls>();
        }

        protected override void OnStart()
        {
            base.OnStart();
            ShowUI();
            GameNotifications.Instance.TriggerNotification("Conseil", "Nous vous conseillons de vous munir d'une souris pour pouvoir vous déplacer en même temps que vous regarder.", 10, 2);
            GameNotifications.Instance.TriggerNotification("Conseil", "Vous pouvez courir en maintenant : \n - <b>Shift</b> \n - <b>clic gauche</b>.", 10, 12);
        }
        protected override void OnOnEnable()
        {
            base.OnOnEnable();
            playerControls.OnControlChanges += TriggerRefresh;
        }

        protected override void OnOnDisable()
        {
            base.OnOnDisable();
            playerControls.OnControlChanges -= TriggerRefresh;
        }

        protected override void OnUpdate()
        {
            if(!Freezed)
                base.OnUpdate();
        }

        protected override void OnFixedUpdate()
        {
            if (!Freezed)
                base.OnFixedUpdate();
        }

        protected override void OnLateFixedUpdate()
        {
            if (!Freezed)
                base.OnLateFixedUpdate();
        }

        protected override void OnLateUpdate()
        {
            if (!Freezed)
                base.OnLateUpdate();
        }
        #endregion

        public void Initiate(PlayerManager manager)
        {
            inputActions = manager.playerInput.actions;
            camera = Camera.main;
        }

        public void BindToNewActions(InputActionAsset asset)
        {

        }

        protected override void Animate()
        {
            if (animator)
            {
                Vector3 currentVelocity = GetVelocity();
                float speed = GetSpeed();

                if (GetMovementInput().sqrMagnitude > .1f)
                {
                    var normVelocity = transform.InverseTransformDirection(currentVelocity).normalized;
                    animator.SetFloat("Norm_Forward_Speed", normVelocity.z, .1f, Time.deltaTime);
                    animator.SetFloat("Norm_Right_Speed", normVelocity.x, .1f, Time.deltaTime);
                    //animator.SetFloat("Angular_Speed", AngularVelocity.y);
                }

                animator.SetBool("IsMoving", speed > .05f);
                animator.SetBool("IsRunning", IsSprinting());
                animator.SetFloat("Speed", speed);
            }
        }

        public void ShowUI() => UI?.Show();
        public void HideUI() => UI?.Hide();


        #region CD_InputActionsProvider
        private void TriggerRefresh() => OnActionsNeedRefresh?.Invoke();
        public string GetControlScheme() => Manager.playerInput.currentControlScheme;

        public InputAction[] GetInputActions() => ActionAsset.actionMaps[0].actions.ToArray();

        public InputDevice[] GetDevices() => Manager.playerInput.devices.ToArray();
        #endregion
    }
}