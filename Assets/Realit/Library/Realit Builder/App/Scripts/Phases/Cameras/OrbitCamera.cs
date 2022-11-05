using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using Aokoro;

namespace Realit.Builder.App.Scene
{
    //Handles the camera that looks around de building mesh
    [AddComponentMenu("Realit/Builder/Edition/Cameras/Orbit")]
    public class OrbitCamera : CinemachineInputProvider
    {
        [SerializeField]
        private InputAction click;
        private CinemachineFreeLook freeLook;

        private Transform center;
        [BoxGroup("CameraSettings"), SerializeField, Range(0,1)]
        private float normHorizontalScreenSize;
        [BoxGroup("CameraSettings"), SerializeField, Range(0,1)]
        private float normVerticalScreenSize;

        private bool canRotate;

        private void Awake()
        {
            click.performed += ctx => {
                canRotate = UIExtentions.Raycast(Pointer.current.position.ReadValue()) == 0;
            };

            click.canceled += ctx => canRotate = false;

            freeLook = GetComponent<CinemachineFreeLook>();

            center = new GameObject("CameraCenter").transform;
            center.SetParent(transform.parent);
            center.position = Vector3.zero;

            freeLook.LookAt = center;
            freeLook.Follow = center;

            ModelImporter.OnModelLoaded += Refresh;
        }

        private void OnEnable()
        {
            click.Enable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            click.Disable();
        }
        private void OnDestroy()
        {
            ModelImporter.OnModelLoaded -= Refresh;
        }


        public void Refresh(GameObject root)
        {
            if (root == null)
                return;

            Bounds bound = default;
            MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                MeshRenderer renderer = renderers[i];

                if (bound == default)
                    bound = renderer.bounds;
                else
                    bound.Encapsulate(renderer.bounds);
            }

            if (bound != default)
                SetupCameraWithModelBounds(bound);
        }

        private void SetupCameraWithModelBounds(Bounds bound)
        {
            center.position = bound.center;

            ///Ring radius
            //Only take on concideration the largest width
            float width = Mathf.Max(bound.size.x, bound.size.z);
            float height = bound.size.y;

            //Max size
            bool isHeight = height > width;
            float size = isHeight ? height : width;
            float screenSize = (isHeight ? normVerticalScreenSize : normHorizontalScreenSize);

            float distance = GetDistanceForSize(size, screenSize);

            freeLook.m_Orbits[0].m_Radius = .5f;
            freeLook.m_Orbits[1].m_Radius = distance;
            freeLook.m_Orbits[2].m_Radius = .5f;

            freeLook.m_Orbits[0].m_Height = bound.min.y - distance;
            freeLook.m_Orbits[1].m_Height = 0;
            freeLook.m_Orbits[2].m_Height = bound.max.y + distance;
        }

        private float GetDistanceForSize(float size, float targetScreenSize)
        {
            
            float radius = size * 0.5f;

            //How much normalized screen space should it take
            float screenSize = 1 / targetScreenSize;

            //Tangent to get de distance (adjacent) using the FOV
            float tan = Mathf.Tan(freeLook.m_Lens.FieldOfView * 0.5f * Mathf.Deg2Rad);

            float distance = (radius * screenSize) / tan;
            return distance;
        }

        public override float GetAxisValue(int axis) => canRotate ? base.GetAxisValue(axis) : 0;
    }
}