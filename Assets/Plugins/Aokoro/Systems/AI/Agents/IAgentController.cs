using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro.AI
{
    public interface IAgentController
    {
        IEnumerable<AIAgent> Agents { get; }
    }
}
