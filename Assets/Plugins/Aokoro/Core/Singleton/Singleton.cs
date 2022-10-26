using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aokoro
{

    [DefaultExecutionOrder(-20)]
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static bool m_ShuttingDown = false;
        private static object m_Lock = new object();

        public static bool HasInstance => m_Instance == null && FindObjectOfType<T>() == null;
        private static T m_Instance;

        protected bool IsInstance => Instance == this;

        protected virtual void Awake()
        {
            if (Instance != this)
                OnExistingInstanceFound(Instance);
        }

        /// <summary>
        /// 
        /// </summary>
        public static T Instance
        {
            get
            {
                if (m_ShuttingDown && Application.isPlaying)
                {
                    Debug.LogWarning("[Singleton] Instance" + typeof(T) + " already destroyed. Returning null");
                    return null;
                }

                lock (m_Lock)
                {
                    if (m_Instance == null)
                    {

                        m_Instance = FindObjectOfType<T>();

                        if (m_Instance == null)
                        {
                            string fullName = typeof(T).ToString();
                            var nameArray = fullName.Split('.');
                            string name = nameArray[nameArray.Length - 1] + " Instance";

                            var singletonObject = new GameObject(name);
                            m_Instance = singletonObject.AddComponent<T>();
                        }
                    }
                    return m_Instance;
                }
            }
        }

        protected abstract void OnExistingInstanceFound(T existingInstance);
    }
}