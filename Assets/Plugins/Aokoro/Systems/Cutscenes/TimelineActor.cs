using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.Cutscenes
{
    public class TimelineActor : MonoBehaviour
    {
        [SerializeField]
        private ActorComponent[] actorComponents;

        private void OnEnable()
        {
            TimelineBinding.AddComponents(actorComponents);
        }
        
        private void OnDisable()
        {
            TimelineBinding.RemoveComponents(actorComponents);
        }
    }
}