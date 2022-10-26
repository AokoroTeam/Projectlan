using NaughtyAttributes;
using UnityEngine;
using Cinemachine;
using System.Collections;
using Realit.Reader.Managers;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Realit.Reader.Features.SliceView
{
    [AddComponentMenu("Realit/Reader/Features/SliceView/Manager")]
    public class SliceView_VirtualCamera : FeatureComponent<SliceView>
    {
        private Bounds LevelBounds => feature.SceneManager.LevelBounds;


        [SerializeField, BoxGroup("Rotation")]
        private float rotationSpeedModifier = 10;
        [SerializeField, BoxGroup("Rotation")]
        private CinemachineOrbitalTransposer orbitalTransposer;
        private CinemachineVirtualCamera vCam;


        [SerializeField, BoxGroup("zoom"), MinMaxSlider(0, 200)]
        Vector2 zoom;
        [SerializeField, BoxGroup("zoom")]
        private float zoomDamping;
        [SerializeField, BoxGroup("zoom")]
        private int zoomSpeed;

        [SerializeField, BoxGroup("move")]
        private int recenterSpeed;
        [SerializeField, ReadOnly]
        private Vector3 targetLookAt;

        private CinemachineFollowZoom zoomComponent;
        private Transform cameraCenter;


        protected override void Start()
        {
            base.Start();

            vCam = GetComponent<CinemachineVirtualCamera>();
            orbitalTransposer = vCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            zoomComponent = orbitalTransposer.VirtualCamera.GetComponent<CinemachineFollowZoom>();
            cameraCenter = orbitalTransposer.FollowTarget;

            zoomComponent.m_MinFOV = zoom.x;
            zoomComponent.m_MaxFOV = zoom.y;
            zoomComponent.m_Damping = zoomDamping;
        }


        public void RefreshVCam()
        {
            cameraCenter.position = Vector3.Lerp(cameraCenter.position, targetLookAt, Time.deltaTime * recenterSpeed);
        }

        #region Inputs
        public void MoveTo(Vector3 position)
        {
            targetLookAt = position;
        }
        public void Move(Vector3 delta)
        {
            targetLookAt += delta;
        }

        public void Zoom(float amount)
        {
            float input = amount * Time.deltaTime;
            float newWidth = input * zoomSpeed + zoomComponent.m_Width;
            zoomComponent.m_Width = Mathf.Clamp(newWidth, zoom.x, zoom.y);
        }

        public void Rotate(float amount)
        {
            orbitalTransposer.m_XAxis.m_InputAxisValue = amount * rotationSpeedModifier * Time.deltaTime;
        }

        #endregion

        #region Feature Component
        protected override void OnFeatureInitiate()
        {
            cameraCenter.position = LevelBounds.center;
            targetLookAt = LevelBounds.center;
        }
        protected override void OnFeatureStarts()
        {
            gameObject.SetActive(true);
        }

        protected override void OnFeatureEnds()
        {
            gameObject.SetActive(false);
        }
        #endregion
    }
}
