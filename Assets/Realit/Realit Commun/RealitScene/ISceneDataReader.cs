using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Realit.Scene
{
    
    public interface IDataReader
    {
        DataSection Section { get; }
        bool Done { get; }
        bool Skipped { get; set; }

        void ApplyData(JToken data);
    }
}