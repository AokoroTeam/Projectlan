using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NaughtyAttributes;
using System;

namespace Aokoro
{

    [System.Serializable]
    public partial class ComplexeCondition
    {
        #region Parameter classes
        public class Parameter
        {
            public readonly bool Inverted;

            public bool Value
            {
                get => Inverted ? !RealValue : RealValue;
            }

            public virtual bool RealValue { get; private set; }

            public void SetValue(bool value) => RealValue = value;

            public Parameter(bool inverted, bool defaultValue = true)
            {
                Inverted = inverted;
                RealValue = defaultValue;
            }
        }

        public class LambdaParameter : Parameter
        {
            private readonly Func<bool> getValue;
            public override bool RealValue => getValue == null || getValue.Invoke();

            public LambdaParameter(Func<bool> lambda, bool inverted, bool defaultValue = true) : base(inverted, defaultValue)
            {
                getValue = lambda;
            }
        }

        #endregion

        [SerializeField]
        private Dictionary<string, Parameter> parameters;

        [SerializeField, ReadOnly]
        private bool result = true;

        public ComplexeCondition()
        {
            parameters = new Dictionary<string, Parameter>();
        }

        public ComplexeCondition(Dictionary<string, Parameter> parameters)
        {
            this.parameters = parameters;
        }

        public void CreateParam(string name, bool inverted = false, bool defaultValue = true)
        {
            if (parameters.ContainsKey(name))
                parameters.Remove(name);

            parameters.Add(name, new Parameter(inverted, defaultValue));
            
        }
        


        public void CreateParam(string name, Func<bool> lambda, bool inverted = false, bool defaultValue = true)
        {
            if (parameters.ContainsKey(name))
                parameters.Remove(name);

            parameters.Add(name, new LambdaParameter(lambda, inverted, defaultValue));
        }


        public bool GetParam(string key)
        {
            if (parameters.TryGetValue(key, out Parameter parameter))
                return parameter.RealValue;
            else
            {
                parameters.Add(key, new Parameter(false));
                return true;
            }
        }

        public void SetParam(string key, bool value)
        {
            if (!parameters.ContainsKey(key))
            {
                Debug.LogError("Parameter " + key + " doesn't exist. Creating default one");
                parameters.Add(key, new Parameter(value));
            }
            else
                parameters[key].SetValue(value);

            if (parameters[key].Value != result)
                result = parameters.All(ctx => ctx.Value.Value);
        }

        public static implicit operator bool(ComplexeCondition complexeCondition) => complexeCondition.result;
    }
}
