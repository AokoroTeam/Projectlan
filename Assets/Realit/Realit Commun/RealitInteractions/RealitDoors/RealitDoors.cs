using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Realit.Library.Doors
{
    [ExecuteInEditMode]
    public class RealitDoors : MonoBehaviour
    {
        [SerializeField]
        RealitDoor[] doors;

        public void SetupDoors()
        {
            var doors = GetComponentsInChildren<MeshRenderer>();
            this.doors = new RealitDoor[doors.Length];

            for (int i = 0; i < doors.Length; i++)
            {
                if (!doors[i].TryGetComponent(out RealitDoor door))
                    door = doors[i].gameObject.AddComponent<RealitDoor>();
                door.gameObject.layer = LayerMask.NameToLayer("Door");
                this.doors[i] = door;
            }
        }

        [Button]
        public void ToDoors()
        {
            for (int i = 0; i < doors.Length; i++)
                doors[i].ToDoor();
        }

        [Button]
        public void ToDefault()
        {
            for (int i = 0; i < doors.Length; i++)
                doors[i].ToDefault();
        }
    }
}
