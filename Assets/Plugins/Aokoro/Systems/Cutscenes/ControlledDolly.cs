using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Aokoro.Cutscenes
{
    public class ControlledDolly : MonoBehaviour
    {
        private CinemachineTrackedDolly trackedDolly;
        [SerializeField]
        private float speed = 5;

        private void Awake()
        {
            trackedDolly = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineTrackedDolly>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            float direction = Input.GetAxis("Horizontal");
            trackedDolly.m_PathPosition += direction * speed * Time.deltaTime;
        }
    }
}
