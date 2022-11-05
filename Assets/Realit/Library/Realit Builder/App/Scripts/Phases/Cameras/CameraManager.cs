using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Realit.Builder.App.Cameras
{
    [AddComponentMenu("Realit/Builder/Edition/Cameras/Manager")]
    public class CameraManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] cameras;

        private void Start()
        {
            SetCameraActiveByIndex(0);
        }

        public void SetCameraActiveByIndex(int index)
        {
            for (int i = 0; i < cameras.Length; i++)
                cameras[i].SetActive(index == i);
        }
    }

}