using System;
using System.Collections;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator method);
    }

    class PushNotificationsContainer : MonoBehaviour, ICoroutineRunner
    {
        static bool s_Created;
        static GameObject s_Container;

        PushNotificationsServiceInstance m_Service;

        /// <summary>
        /// For the test harness only.
        /// </summary>
        internal static PushNotificationsContainer Instance { get; private set; }


        internal static PushNotificationsContainer CreateContainer()
        {
            if (!s_Created)
            {
#if UNITY_PUSH_NOTIFICATION_DEVELOPMENT
                Debug.Log("Created Analytics Container");
#endif

                s_Container = new GameObject("PushNotificationsContainer");
                Instance = s_Container.AddComponent<PushNotificationsContainer>();

                s_Container.hideFlags = HideFlags.DontSaveInBuild | HideFlags.NotEditable;
#if !UNITY_PUSH_NOTIFICATION_DEVELOPMENT
                s_Container.hideFlags |= HideFlags.HideInInspector;
#endif

                DontDestroyOnLoad(s_Container);
                s_Created = true;
            }

            return Instance;
        }

        public void Initialize(PushNotificationsServiceInstance service)
        {
            m_Service = service;
        }

        void OnApplicationPause(bool paused)
        {
            m_Service.OnApplicationPause(paused);
        }

        void OnDestroy()
        {
            // NOTE: we use OnDestroy rather than OnApplicationQuit in case the game developer should
            // deliberately/accidentally destroy the container object. This should ensure graceful shutdown
            // of the SDK regardless of 'how' it actually got turned off.
            s_Container = null;
            s_Created = false;
        }
    }
}
