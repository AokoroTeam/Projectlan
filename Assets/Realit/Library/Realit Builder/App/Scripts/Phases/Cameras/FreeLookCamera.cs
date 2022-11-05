using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Aokoro;

namespace Realit.Builder.App.Edition
{
    //Handles the camera that freely moves around
    [AddComponentMenu("Realit/Builder/Edition/Cameras/Free look")]
    public class FreeLookCamera : MonoBehaviour, IPhaseComponent
    {
        [SerializeField]
        private InputActionAsset inputs;
        private InputActionMap freeCameraMap;

        //Actions
        InputAction hold;
        InputAction look;
        InputAction move;
        InputAction cursor;

        private CinemachinePOV pov;
        [SerializeField, BoxGroup("Looking")]
        float lookHorizontalModifier = 1;
        [SerializeField, BoxGroup("Looking")]
        float lookVerticalModifier = 1;
        [SerializeField, BoxGroup("Looking")]
        private GraphicRaycaster raycaster;


        [SerializeField, BoxGroup("Moving")]
        float acceleration = 1;
        [SerializeField, BoxGroup("Moving")]
        float maxSpeed = 1;
        [SerializeField, BoxGroup("Moving")]
        float decelaration = 1;
        [ShowNonSerializedField, BoxGroup("Moving")]
        private Vector3 currentVelocity;
        [ShowNonSerializedField, BoxGroup("Moving")]
        private Vector3 inputDirection;

        Transform mainCamera;

        private Vector2 startingHoldingPos;
        public Phase phase => Phase.Edition;


        private void Awake()
        {
            SetupInputs();
            pov = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>();

            mainCamera = Camera.main.transform;
        }

        private void OnEnable()
        {
            if(this.IsActive())
            { 
                freeCameraMap.Enable();
                look.Disable();
            }
        }


        private void OnDisable()
        {
            freeCameraMap.Disable();
        }

        private void SetupInputs()
        {
            //Clone so that it can be used by others
            freeCameraMap = inputs.FindActionMap("FreeCamera").Clone();

            //For the mouse drag
            hold = freeCameraMap.FindAction("Hold");
            //Hold performed
            hold.performed += OnHold;
            //Hold performed
            hold.started += OnHold;
            //Hold released
            hold.canceled += OnHold;

            //Camera rotation
            look = freeCameraMap.FindAction("Look");

            move = freeCameraMap.FindAction("Move");
            
            cursor = freeCameraMap.FindAction("CursorPosition");
            
        }

        private void Update()
        {
            if (look.enabled)
                SetInputAxis(look.ReadValue<Vector2>());

            HandleMovement();
        }

        private void HandleMovement()
        {
            //Value from keyboard
            inputDirection = move.ReadValue<Vector3>();

            //Horizontal speed is camera dependent
            Vector3 horizontalDirection = mainCamera.TransformDirection(new Vector3(inputDirection.x, 0, inputDirection.z));
            //Vertical speed should always be vertical, regardless of camera orientation
            Vector3 verticalDirection = new Vector3(0, inputDirection.y, 0);

            Vector3 targetVelocity = (horizontalDirection + verticalDirection).normalized * maxSpeed;

            //Is accelerating or decelerating ?
            float lerp = move.triggered ? acceleration : decelaration;
            //Smooth transition
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, lerp * Time.deltaTime);

            transform.position += currentVelocity * Time.deltaTime;
        }



        private void OnHold(InputAction.CallbackContext ctx)
        {
            //Debug.Log(ctx.phase);

            if(ctx.phase == InputActionPhase.Started)
            {
                startingHoldingPos = cursor.ReadValue<Vector2>();
                return;
            }
            else if (ctx.phase == InputActionPhase.Performed)
            {
                //If nothing was in mouse path, enable look inputs
                if(UIExtentions.Raycast(startingHoldingPos) == 0)
                {
                    look.Enable();
                    //Debug.Log("Looking");
                }
            }
            else if (ctx.phase == InputActionPhase.Canceled)
            {
                look.Disable();
                SetInputAxis(Vector2.zero);
            }
        }


        private void SetInputAxis(Vector2 input)
        {
            pov.m_HorizontalAxis.Value += -input.x * lookHorizontalModifier;
            pov.m_VerticalAxis.Value += input.y * lookVerticalModifier;
        }
    }
}