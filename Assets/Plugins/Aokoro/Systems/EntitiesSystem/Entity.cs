using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using NaughtyAttributes;
using UnityEngine.AI;
using Aokoro;

namespace Aokoro.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        IUpdateEntityComponent[] Ucomponents;
        ILateUpdateEntityComponent[] LUcomponents;
        IFixedUpdateEntityComponent[] FUcomponents;

        Dictionary<string, IEntityComponent> components;

        public InfluencedProperty<bool> Freezed;

        private bool isReady = false;

        protected virtual void Awake()
        {
            Freezed = new InfluencedProperty<bool>(false);
            Initiate<Entity>();
        }

        protected virtual void Initiate<T>() where T : Entity
        {

            var componentsArray = SetupComponents<T>(GetComponentsInChildren<IEntityComponent>());
            components = new Dictionary<string, IEntityComponent>(componentsArray.Length);

            List<IUpdateEntityComponent> updatesList = new();
            List<ILateUpdateEntityComponent> lateUpdatesList = new();
            List<IFixedUpdateEntityComponent> fixedUpdatesList = new();

            for (int i = 0; i < componentsArray.Length; i++)
            {
                IEntityComponent component = componentsArray[i];
                if (component is IUpdateEntityComponent u)
                    updatesList.Add(u);
                if (component is IFixedUpdateEntityComponent fu)
                    fixedUpdatesList.Add(fu);
                if (component is ILateUpdateEntityComponent lu)
                    lateUpdatesList.Add(lu);

                components.Add(component.ComponentName, component);
            }

            Ucomponents = updatesList.ToArray();
            FUcomponents = fixedUpdatesList.ToArray();
            LUcomponents = lateUpdatesList.ToArray();

            isReady = true;
        }

        private IEntityComponent[] SetupComponents<T>(IEntityComponent[] ChildComponents) where T : Entity
        {
            List<IEntityComponent> componentsList = new List<IEntityComponent>();
            int count = ChildComponents.Length;

            for (int i = 0; i < count; i++)
            {
                IEntityComponent component = ChildComponents[i];
                Type componentType = component.GetType();
                Type[] implementedInterfaces = componentType.GetInterfaces();

                for (int j = 0; j < implementedInterfaces.Length; j++)
                {
                    Type interfaceType = implementedInterfaces[j];
                    if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEntityComponent<>))
                    {
                        var ga = interfaceType.GetGenericArguments()[0];
                        if (ga == typeof(T) || ga.IsSubclassOf(typeof(T)))
                        {
                            if (component is IUpdateEntityComponent<T> u)
                            {
                                u.Manager = this as T;
                                u.Initiate(this as T);
                            }
                            else if (component is IFixedUpdateEntityComponent<T> fu)
                            {
                                fu.Manager = this as T;
                                fu.Initiate(this as T);
                            }
                            else if (component is ILateUpdateEntityComponent<T> Lu)
                            {
                                Lu.Manager = this as T;
                                Lu.Initiate(this as T);
                            }
                            else if(component is IEntityComponent<T> c)
                            {
                                c.Manager = this as T;
                                c.Initiate(this as T);
                            }
                            break;
                        }
                        else
                        {
                            Debug.LogError("Wrong manager for this living component");
                        }
                    }

                }

                componentsList.Add(component);
            }

            return componentsList.ToArray();
        }

        protected virtual void Update()
        {
            if (isReady && !Freezed.Value)
            {
                for (int i = 0; i < Ucomponents.Length; i++)
                {
                    try
                    {
                        Ucomponents[i].OnUpdate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e, Ucomponents[i] as MonoBehaviour);
                    }
                }
            }
        }

        protected virtual void FixedUpdate()
        {
            if (isReady && !Freezed.Value)
            {
                for (int i = 0; i < FUcomponents.Length; i++)
                {
                    try
                    {
                        FUcomponents[i].OnFixedUpdate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e, FUcomponents[i] as MonoBehaviour);
                    }
                }
            }
        }

        protected virtual void LateUpdate()
        {
            if (isReady && !Freezed.Value)
            {
                for (int i = 0; i < LUcomponents.Length; i++)
                {
                    try
                    {
                        LUcomponents[i].OnLateUpdate();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }


        public bool GetLivingComponent<T>(out T component) where T : IEntityComponent
        {
            component = default;
            foreach (IEntityComponent c1 in components.Values)
            {
                if (c1 is T c)
                {
                    component = c;
                    return true;
                }
            }
            return false;
        }
        public T GetLivingComponent<T>() where T : IEntityComponent
        {
            foreach (IEntityComponent c1 in components.Values)
            {
                if (c1 is T c)
                    return c;
            }

            return default;
        }
        public T GetLivingComponent<T>(string name) where T : IEntityComponent
        {
            if (components.TryGetValue(name, out IEntityComponent entityComponent) && entityComponent is T result)
                return result;
            
            return default;
        }
        public bool GetLivingComponent<T>(string name, out T component) where T : IEntityComponent
        {
            if (components.TryGetValue(name, out IEntityComponent entityComponent) && entityComponent is T result)
            {
                component = result;
                return true;
            }

            component = default;
            return false;
        }
        public void Log(string message)
        {
            Debug.Log(string.Concat("[", gameObject.name, "] => ", message));
        }


    }
}
