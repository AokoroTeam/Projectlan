using Aokoro.Sequencing.Steps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Aokoro.Sequencing
{
    public interface IParameter
    {
        public string name{ get; }
        public object Value { get; }
    }

    public interface IParameter<T> : IParameter
    {
        new public T Value { get; }
    }
}