using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Realit.Reader.Player;

namespace Realit.Reader.Features
{

    public interface IPlayerFeature
    {
        public Feature @Feature { get; }
        public string MapName { get; }
        public Realit_Player Player { get; set; }
    }
}