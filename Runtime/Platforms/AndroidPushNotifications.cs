#if UNITY_ANDROID || UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Notifications.Android;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    class AndroidPushNotifications : AndroidJavaProxy, IPushPlatform
    {
        TaskCompletionSource<string> s_RegisterTcSource;

        IPushPlatformCallbacks m_PushNotificationsHandler;
        readonly ICoroutineRunner m_Container;

        AndroidJavaObject s_CurrentActivity;
        AndroidJavaObject s_CurrentContext;
        AndroidJavaObject s_PushNotificationManager;

        public AndroidPushNotifications(ICoroutineRunner container)
            : base("com.unity.services.pushnotifications.android.UnityPushNotificationsCallback")
        {
            m_Container = container;
        }

        public void Initialize(IPushPlatformCallbacks callbacks)
        {
            m_PushNotificationsHandler = callbacks;
        }

        AndroidJavaObject GetCurrentActivity()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        public void OnApplicationPause(bool isPaused)
        {
            // Android has no pause/unpause actions.
        }

        /// <summary>
        /// Registers for push notifications on Android. Returns the device token for the registered device.
        /// This method checks if the user grants the notification permission, it will register for push notification and get the device token.
        /// </summary>
        /// <returns>The push notification token for this device</returns>
        public Task<string> RegisterForPushNotifications(PushNotificationSettings settings)
        {
            // Check if a Task exists before re instantiating a new TaskCompletionSource
            if (s_RegisterTcSource != null)
            {
                return s_RegisterTcSource.Task;
            }
            s_RegisterTcSource = new TaskCompletionSource<string>();

            if (String.IsNullOrEmpty(settings.firebaseWebApiKey) ||
                String.IsNullOrEmpty(settings.firebaseAppID) ||
                String.IsNullOrEmpty(settings.firebaseProjectNumber) ||
                String.IsNullOrEmpty(settings.firebaseProjectID))
            {
                throw new Exception("UGS Push Notifications is missing Android settings - make sure these are set in the editor Project Settings");
            }

            m_Container.StartCoroutine(
                RequestAuthorization(
                    settings.firebaseWebApiKey,
                    settings.firebaseProjectNumber,
                    settings.firebaseAppID,
                    settings.firebaseProjectID));

            return s_RegisterTcSource.Task;
        }

        IEnumerator RequestAuthorization(string firebaseApiKey, string firebaseSenderId, string firebaseApplicationId, string firebaseProjectId)
        {
            var request = new PermissionRequest();
            while (request.Status == PermissionStatus.RequestPending)
            {
                yield return null;
            }

            if (s_RegisterTcSource == null)
            {
                yield return null;
            }

            switch (request.Status)
            {
                case PermissionStatus.Allowed:
                    InitializePushNotifications(firebaseApiKey, firebaseSenderId, firebaseApplicationId, firebaseProjectId);
                    break;
                case PermissionStatus.Denied:
                case PermissionStatus.DeniedDontAskAgain:
                    s_RegisterTcSource.TrySetException(new Exception($"Authorization request was denied"));
                    s_RegisterTcSource = null;
                    break;
            }
        }

        void InitializePushNotifications(string firebaseApiKey, string firebaseSenderId, string firebaseApplicationId, string firebaseProjectId)
        {
            try
            {
                // Call Android Plugin code to initialise Push Notification with firebase
                using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    s_CurrentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }
                s_CurrentContext = GetCurrentActivity().Call<AndroidJavaObject>("getApplicationContext");

                var unityPushNotificationManagerClass = new AndroidJavaClass("com.unity.services.pushnotifications.android.UnityPushNotifications");
                s_PushNotificationManager = unityPushNotificationManagerClass.CallStatic<AndroidJavaObject>("getUnityPushNotificationImpl", s_CurrentContext);
                s_PushNotificationManager.Call("initialize", s_CurrentActivity, this);
                s_PushNotificationManager.Call("registerForPushNotifications", firebaseApiKey, firebaseApplicationId, firebaseSenderId, firebaseProjectId);
            }
            catch (Exception e)
            {
                if (s_RegisterTcSource == null)
                {
                    return;
                }
                s_RegisterTcSource.TrySetException(new Exception($"Failed to initialize Push Notification Plugin {e.Message}"));
                s_RegisterTcSource = null;
            }
        }

        public Dictionary<string, object> CheckForAppLaunchByNotification()
        {
            // This is to check if the notification is launched on cold boot and call OnRemoteNotificationReceived
            try
            {
                AndroidJavaObject intent = GetCurrentActivity().Call<AndroidJavaObject>("getIntent");
                string intentNotificationData = intent.Call<string>("getStringExtra", "notificationData");
                if (intentNotificationData != null)
                {
                    // remove opened notificationData so it cannot be re-processed
                    intent.Call("removeExtra", "notificationData");

                    return ParseNotification(intentNotificationData);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Push: GetIntent -> Failed to check intent for notification data: {e.Message}");
            }

            return null;
        }

        // Implementing the UnityRemoteNotificationsCallback from the Plugin
        // See: UnityPushNotificationsCallback.java
        void OnTokenReceived(string token)
        {
            s_RegisterTcSource.TrySetResult(token);
            s_RegisterTcSource = null;
        }

        // Implementing the UnityRemoteNotificationsCallback from the Plugin
        // See: UnityPushNotificationsCallback.java
        void OnRemoteNotificationReceived(string notificationDataAsJson)
        {
            Dictionary<string, object> notificationData = ParseNotification(notificationDataAsJson);
            m_PushNotificationsHandler.RemoteNotificationReceived(notificationData);
        }

        Dictionary<string, object> ParseNotification(string notificationData)
        {
            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(notificationData);
            }
            catch (Exception e)
            {
                Debug.Log($"Push: Failed handle notification: {e.Message}");
            }

            return null;
        }
    }
}
#endif
