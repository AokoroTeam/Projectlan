using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDirection
{
    public static Transform MainCamera
    {
        get
        {
            if (mainCamera == null)
            {
                var cam = Camera.main;
                if (cam != null)
                    mainCamera = cam.transform;
                else
                {
                    Debug.LogError("No camera is in the scene, creating one");

                    mainCamera = new GameObject("Main Camera").AddComponent<Camera>().transform;
                    mainCamera.tag = "MainCamera";
                }
            }

            return mainCamera;
        }
    }

    private static Transform mainCamera;

    public static Vector3 Forward => MainCamera == null ? Vector3.forward : FlattenDirection(mainCamera.forward);
    public static Vector3 Back => -Forward;
    public static Vector3 Right => MainCamera == null ? Vector3.right : FlattenDirection(mainCamera.right);
    public static Vector3 Left => -Right;
    public static Vector3 Up => Vector3.up;
    public static Vector3 Down => Vector3.down;
    private static Vector3 FlattenDirection(Vector3 direction) => Vector3.ProjectOnPlane(direction, Vector3.up).normalized * direction.magnitude;
}