using Aokoro.Entities.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using NaughtyAttributes;
using Aokoro.Entities;
using Realit.Reader.Player;
using DG.Tweening;
using System;
using Realit.Reader.Player.Movement;

namespace Realit.Reader.Features.SliceView
{
    [AddComponentMenu("Realit/Reader/Features/SliceView/Player")]
    public class SliceView_Player : PlayerFeatureComponent<SliceView>, IUpdateEntityComponent<Realit_Player>
    {
        private Bounds LevelBounds => feature.SceneManager.LevelBounds;

        InputAction cursorPosition;
        InputAction moveAction;
        InputAction rotateAction;
        InputAction zoomAction;

        
        public Realit_Player Manager { get; set; }

        public string ComponentName => "SliceView player";



        #region Inputs Management

        private void OnInteract(InputAction.CallbackContext ctx)
        {
            //Ray
            Ray centerRay = Camera.main.ScreenPointToRay(cursorPosition.ReadValue<Vector2>());
            if (Raycast(centerRay, out RaycastHit centerHit))
                feature.VirtualCamera.Move(centerHit.point);
            else
                Debug.Log("Nothing touched");
        }


        private void OnMove(InputAction.CallbackContext ctx)
        {
            //Pixel positions
            Vector2 delta = ctx.ReadValue<Vector2>();
            Vector2 pixelPositionOfCenter = Camera.main.ViewportToScreenPoint(Vector3.one * .5f);

            //Rays
            Ray centerRay = Camera.main.ScreenPointToRay(pixelPositionOfCenter);
            Ray deltaRay = Camera.main.ScreenPointToRay(pixelPositionOfCenter + delta);

            //Creating plane
            Plane plane;
            if (Raycast(centerRay, out RaycastHit centerHit))
                plane = new Plane(Vector3.up, centerHit.point);
            else
                plane = new Plane(Vector3.up, LevelBounds.center);

            //Dragging
            if (plane.Raycast(centerRay, out float centerDistance)
                && plane.Raycast(deltaRay, out float deltaDistance))
            {
                Vector3 A = centerRay.GetPoint(centerDistance);
                Vector3 B = deltaRay.GetPoint(deltaDistance);

                Debug.DrawRay(A, Vector3.up * 10, Color.blue);
                Debug.DrawRay(B, Vector3.up * 10, Color.green);

                Vector3 worldDelta = B - A;
                //Debug.Log($"A : {A} | B : {B}");

                feature.VirtualCamera.Move(-worldDelta);
            }
        }

        private void OnExit_performed(InputAction.CallbackContext ctx) => Player.EndFeature(feature);
        private void OnRotate_performed(InputAction.CallbackContext ctx) => feature.VirtualCamera.Rotate(ctx.ReadValue<float>());
        private void OnZoom_Performed(InputAction.CallbackContext ctx) => feature.VirtualCamera.Zoom(ctx.ReadValue<float>());

        #endregion

        #region Entity intergration
        public void Initiate(Realit_Player manager)
        {

        }

        public void OnUpdate()
        {
            if (feature.IsActive)
                feature.VirtualCamera.RefreshVCam();
        }

        public override void BindToNewActions(InputActionAsset asset)
        {
            var map = asset.FindActionMap(MapName);
            try
            {
                rotateAction = map.FindAction("Rotate");
                zoomAction = map.FindAction("Zoom");
                moveAction = map.FindAction("Move");
                cursorPosition = map.FindAction("CursorPosition");

                ///Rotation
                rotateAction.performed += OnRotate_performed;
                ///Zoom
                zoomAction.performed += OnZoom_Performed;

                ///Move
                moveAction.performed += OnMove;

                map.FindAction("Interact").performed += OnInteract;

                map.FindAction("Exit").performed += OnExit_performed;

            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }
        #endregion

        #region Feature component integration
        protected override void OnFeatureInitiate()
        {

        }

        protected override void OnFeatureStarts()
        {
            Player.ChangeActionMap(MapName);
            if (Player.GetLivingComponent(out PlayerCharacter character))
            {
                Debug.Log("[Slice view] Freezing player movements");
                character.Freezed.Subscribe(this, 20, true);
            }

            
        }

        protected override void OnFeatureEnds()
        {
            Player.ChangeActionMap(Player.DefaultActionMap);

            if (Player.GetLivingComponent(out PlayerCharacter character))
            {
                Debug.Log("[Slice view] Defreezing player movements");
                character.Freezed.Unsubscribe(this);
            }
        }

        #endregion

       
        #region Utility
        private static bool Raycast(Ray centerRay, out RaycastHit centerHit)
        {
            return Physics.Raycast(centerRay, out centerHit, 100, LayerMask.GetMask("SliceViewPlane"), QueryTriggerInteraction.Collide);
        }

        #endregion
    }
}