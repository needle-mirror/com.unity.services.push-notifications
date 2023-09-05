using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    internal interface IPushPlatform
    {
        void Initialize(IPushPlatformCallbacks callbacks);

        Task<string> RegisterForPushNotifications(PushNotificationSettings settings);

        Dictionary<string, object> CheckForAppLaunchByNotification();

        void OnApplicationPause(bool isPaused);
    }

    internal class UnsupportedPlatformLogic : IPushPlatform
    {
        public void Initialize(IPushPlatformCallbacks callbacks)
        {
        }

        public Dictionary<string, object> CheckForAppLaunchByNotification()
        {
            return null;
        }

        public Task<string> RegisterForPushNotifications(PushNotificationSettings settings)
        {
#if UNITY_EDITOR
            Debug.Log("Push notifications are not available in the Unity Editor.");
#else
            Debug.Log("Push notifications are not available on this platform.");
#endif
            return Task.FromResult("");
        }

        public void OnApplicationPause(bool isPaused)
        {
        }
    }
}
