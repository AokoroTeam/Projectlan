using Newtonsoft.Json.Linq;
using Realit.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realit.Models;
using NaughtyAttributes;
using Realit.Library.Doors;

namespace Realit.Reader.Managers
{
    [AddComponentMenu("Realit/Reader/Data/Model data reader")]
    public class ModelDataReader : MonoBehaviour, IDataReader
    {
        public DataSection Section => DataSection.Model;

        public bool Done => isModelGenerated;

        public bool Skipped { get; set; }

        private bool isModelGenerated = false;
        private RealitDoors doors;

        [SerializeField]
        RealitModel structure;
        [SerializeField]
        RealitModel apertures;

        static GameObject Structure;
        static GameObject Apertures;

        public void ApplyData(JToken data)
        {
            isModelGenerated = false;

            structure = new RealitModel(data["Structure"]);
            apertures = new RealitModel(data["Apertures"]);

            Structure = structure.Generate(transform);
            Apertures = apertures.Generate(transform);

            RealitReader.OnModelLoaded(Structure);
            RealitReader.Instance.OnEndsLoading += Instance_OnEndsLoading;
            isModelGenerated = true;

            
        }

        private void Instance_OnEndsLoading()
        {
            RealitReader.Instance.OnEndsLoading -= Instance_OnEndsLoading;
            Refresh();

            //After material have been applied
            doors = Apertures.AddComponent<RealitDoors>();
            doors.SetupDoors();
            doors.ToDoors();
        }

        public static void Refresh()
        {
            if (Structure != null)
            {
                var renderers = Structure.GetComponentsInChildren<MeshRenderer>();
                for (int index = 0; index < renderers.Length; index++)
                {
                    renderers[index].UpdateGIMaterials();
                    Material[] sharedMaterials = renderers[index].sharedMaterials;
                    for (int i = 0; i < sharedMaterials.Length; i++)
                    {
                        sharedMaterials[i].shader = Shader.Find(sharedMaterials[i].shader.name);
                    }
                }
            }
        }
    }
}