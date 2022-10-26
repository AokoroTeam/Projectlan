using Aokoro.Sequencing;
using Aokoro.Sequencing.Steps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Aokoro.Sequencing
{
    public partial class Sequencer
    {
        Dictionary<string, IParameter> Parameters
        {
            get
            {
                if (parameters == null)
                    parameters = new Dictionary<string, IParameter>();

                return parameters;
            }
            set
            {
                parameters = value;
            }
        }
        Dictionary<string, IParameter> parameters;

        public void ClearParameters() => Parameters.Clear();
       
        public void CreateParameters(params IParameter[] parameters) => this.Parameters = parameters.ToDictionary(ctx => ctx.name, ctx => ctx);
        public void SetParameter<T>(string name, T value)
        {
            if (!Parameters.ContainsKey(name))
                Parameters.Add(name, new SequenceParameterBase<T>(name, value));
            else
            {
                object p_object = Parameters[name];

                if (p_object is SequenceParameterBase<T> p)
                    p.Value = value;
                else
                    SequencerManager.LogError(name + " Is of type " + p_object.GetType() + " but your trying to assign a " + typeof(T) + " value", this);
            }
        }

        public void RemoveParameters(string name)
        {
            if (Parameters.ContainsKey(name))
                Parameters.Remove(name);
        }

        public T GetParameterValue<T>(string name)
        {
            if (Parameters.TryGetValue(name, out IParameter iP))
            {
                if (iP is SequenceParameterBase<T> p)
                    return p.Value;
                else
                {
                    SequencerManager.LogError(name + " Is of type " + iP.GetType() + " but your trying to assign a " + typeof(T) + " value", this);
                    return default;
                }
            }

            SequencerManager.LogError($"Cannot find parameter with name {name}", this);
            return default;
        }

        
    }
}

