using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Realit.Scene
{
    
    public interface IDataBuilder
    {
        DataSection Section { get; }
        bool IsValid { get; }
        JObject Serialize();
    }
}