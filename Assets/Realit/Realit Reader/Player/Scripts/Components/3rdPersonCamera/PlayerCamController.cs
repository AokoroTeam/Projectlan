
using Aokoro.Entities;
using Aokoro.Entities.Player;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Realit.Reader.Player.CameraManagement
{
    public class PlayerCamController : CinemachineInputProvider, IEntityComponent<PlayerManager>
    {
        string IEntityComponent.ComponentName => "PlayerCamController";
        public PlayerManager Manager { get; set; }

        private InputAction lookAction;

        [Header("Cam parameters")]
        [SerializeField, Range(.01f, 4)]
        private float verticalSpeed;
        [SerializeField, Range(.01f, 4)]
        private float horizontalSpeed;

        private const float scaling = 200;

        public virtual void Initiate(PlayerManager manager)
        {
            lookAction = manager.playerInput.actions.FindActionMap("DefaultGameplay").FindAction("Look");
            lookAction.Enable();
            XYAxis.Set(lookAction);
        }


        public void DisableInputs()
        {
            lookAction.Disable();
        }

        public void EnableInputs()
        {
            lookAction.Enable();
        }

        public override float GetAxisValue(int axis)
        {
            var mouses = InputSystem.devices;

            if (enabled)
            {
                var action = ResolveForPlayer(axis, axis == 2 ? ZAxis : XYAxis);
                if (action != null)
                {
                    switch (axis)
                    {
                        case 0: return action.ReadValue<Vector2>().x * Time.deltaTime * horizontalSpeed * scaling;
                        case 1: return action.ReadValue<Vector2>().y * Time.deltaTime * verticalSpeed * scaling;
                        case 2: return action.ReadValue<float>() * Time.deltaTime * scaling;
                    }
                }
            }


            return 0;
        }
    }
}
