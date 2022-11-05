using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Realit.Builder.App
{

    public interface IPhaseComponent
    {
        Phase phase { get; }

    }

    public static class IPhaseComponentExtension
    {
        public static bool IsActive(this IPhaseComponent component) => component.phase == RealitBuilder.Instance.CurrentPhase;
    }
}