using Aokoro.Entities.Player;
using Cinemachine;
using UnityEngine;
using Aokoro.Entities;

namespace Realit.Reader.Player.CameraManagement
{

    public class FirstPersonCameraController : PlayerCamController, ILateUpdateEntityComponent<PlayerManager>
    {
        CinemachineVirtualCamera Vcam
        {
            get
            {
                if (_vcam == null)
                    _vcam = GetComponent<CinemachineVirtualCamera>();
                return _vcam;
            }
        }
        CinemachinePOV Pov
        {
            get
            {
                if (_pov == null)
                    _pov = Vcam.GetCinemachineComponent<CinemachinePOV>();
                return _pov;
            }
        }
        CinemachineVirtualCamera _vcam;
        CinemachinePOV _pov;

        public override void Initiate(PlayerManager manager)
        {
            base.Initiate(manager);
            transform.SetParent(null);
        }

        public override float GetAxisValue(int axis)
        {
            return base.GetAxisValue(axis);
        }

        public void OnCameraLive(ICinemachineCamera from, ICinemachineCamera to)
        {
            Recenter();
        }
        
        private void Start()
        {
            Recenter();
        }

        private void Recenter()
        {
            Quaternion rot = Quaternion.LookRotation(Vcam.LookAt.transform.forward, Vector3.up);
            Pov.ForceCameraPosition(Vcam.transform.position, rot);
            //Pov.transform.rotation = rot;
            //Pov.m_HorizontalAxis.Value = Pov.m_VerticalAxis.Value = 0;
        }

        public void OnLateUpdate()
        {
            Pov.LookAtTarget.forward = Manager.transform.forward;
        }
    }
}