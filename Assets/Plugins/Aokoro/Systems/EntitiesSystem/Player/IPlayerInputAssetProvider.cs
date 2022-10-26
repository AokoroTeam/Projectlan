using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Aokoro.Entities.Player
{
    public interface IPlayerInputAssetProvider 
    {
        public InputActionAsset ActionAsset { get; set; }

        void BindToNewActions(InputActionAsset asset);
    }
}