using Newtonsoft.Json.Linq;
using Realit.Scene;
using Realit.Settings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Realit.Reader.Managers
{
    public class ProjectDataReader : MonoBehaviour, IDataReader
    {
        public DataSection Section => DataSection.Project;

        public bool Done => true;

        public bool Skipped { get; set; }


        public string ProjectName;
        public RealitSettings settings;

        public void ApplyData(JToken data)
        {
            ProjectName = ((string?)data["Project"]);
            settings = data["Settings"].ToObject<RealitSettings>();

            RealitSettings.GlobalSettings = settings;
        }
    }
}
