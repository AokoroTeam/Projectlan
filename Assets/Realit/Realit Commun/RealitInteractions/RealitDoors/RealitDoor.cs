using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Realit.Library.Doors
{
    [RequireComponent(typeof(MeshRenderer))]
    public class RealitDoor : MonoBehaviour
    {
        [SerializeField, ReadOnly]
        Material[] DefaultMaterials;
        [SerializeField, ReadOnly]
        Material[] DoorMaterials;

        [SerializeField, ReadOnly]
        Material ditheredDoorMaterial;
        [SerializeField]
        Vector2 minMaxDistance;


        private new MeshRenderer renderer;

        public MeshRenderer Renderer
        {
            get
            {
                if(renderer == null)
                    renderer = GetComponent<MeshRenderer>();

                return renderer;
            }
        }

        private void Awake()
        {
            ditheredDoorMaterial = Resources.Load<Material>("DoorMaterial");

            DefaultMaterials = Application.isPlaying ? Renderer.materials : Renderer.sharedMaterials;
            DoorMaterials = new Material[DefaultMaterials.Length];

            for (int i = 0; i < DefaultMaterials.Length; i++)
            {
                Material mat = new(ditheredDoorMaterial)
                {
                    mainTexture = DefaultMaterials[i].mainTexture,
                    color = DefaultMaterials[i].color
                };

                mat.SetFloat("_Surface", DefaultMaterials[i].GetFloat("_Surface"));
                DoorMaterials[i] = mat;
            }
        }

        [Button]
        internal void ToDoor()
        {

            if (Application.isPlaying)
                Renderer.materials = DoorMaterials;
            else
                Renderer.sharedMaterials = DoorMaterials;

        }

        [Button]
        internal void ToDefault()
        {
            if (Application.isPlaying)
                Renderer.materials = DefaultMaterials;
            else
                Renderer.sharedMaterials = DefaultMaterials;
        }
    }
}
