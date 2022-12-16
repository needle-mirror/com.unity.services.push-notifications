using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    class IOSPushNotifications
    {
        static object s_RegistrationLock = new object();
        static TaskCompletionSource<string> s_DeviceRegistrationTcs;
        static string s_DeviceToken;

        static PushNotificationReceivedHandler s_NotificationReceivedHandler;
        static PushNotificationAnalytics s_NotificationAnalytics;

        internal static event Action<Dictionary<string, object>> InternalNotificationWasReceived;

        delegate void NotificationRegistrationCallback(string deviceToken);

        delegate void NotificationReceivedCallback(string serialisedNotificationData);

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern void NativeRegisterForPushNotifications(NotificationRegistrationCallback callback);

        [DllImport("__Internal")]
        static extern void RegisterUnityCallbackForNotificationReceived(NotificationReceivedCallback callback);

        [DllImport("__Internal")]
        static internal extern string GetLaunchedNotificationString();

        [DllImport("__Internal")]
        static internal extern void ResetLaunchedNotificationString();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        internal static void PerformLaunchActions()
        {
            RegisterUnityCallbackForNotificationReceived(NotificationReceived);
        }

#endif
        public IOSPushNotifications(PushNotificationReceivedHandler notificationReceivedHandler, PushNotificationAnalytics analytics)
        {
            s_NotificationReceivedHandler = notificationReceivedHandler;
            s_NotificationAnalytics = analytics;

            #if UNITY_IOS && !UNITY_EDITOR
                string launchedNotificationString = GetLaunchedNotificationString();

                if (launchedNotificationString != null)
                {
                    Debug.Log("App launched from notification, sending relevant events");
                    NotificationReceived(launchedNotificationString);

                    // remove launched notificationData so it cannot be re-processed
                    ResetLaunchedNotificationString();
                }
            #endif
        }

        /// <summary>
        /// Registers for push notifications on iOS. Returns the device token for the registered device.
        /// </summary>
        /// <returns>The push notification token for this device</returns>
        public Task<string> RegisterForPushNotificationsAsync()
        {
            #if UNITY_IOS && !UNITY_EDITOR
            lock (s_RegistrationLock)
            {
                if (!String.IsNullOrEmpty(s_DeviceToken))
                {
                    return Task.FromResult(s_DeviceToken);
                }

                if (s_DeviceRegistrationTcs != null)
                {
                    return s_DeviceRegistrationTcs.Task;
                }

                s_DeviceRegistrationTcs = new TaskCompletionSource<string>();

                NativeRegisterForPushNotifications(NotificationRegistrationTokenReceived);

                return s_DeviceRegistrationTcs.Task;
            }
            #else
            Debug.Log("iOS notification support is only available in iOS builds");
            return Task.FromResult<string>(null);
            #endif
        }

        [AOT.MonoPInvokeCallback(typeof(NotificationRegistrationCallback))]
        internal static void NotificationRegistrationTokenReceived(string token)
        {
            lock (s_RegistrationLock)
            {
                if (string.IsNullOrEmpty(token))
                {
                    s_DeviceRegistrationTcs.TrySetException(new Exception("Failed to register the device for remote notifications."));
                }
                else
                {
                    s_DeviceToken = token;
                    s_DeviceRegistrationTcs.TrySetResult(token);
                    s_NotificationAnalytics.RecordPushTokenUpdated(token);
                    Debug.Log($"Successfully registered for remote push notifications with token: {token}");
                }

                // Reset registration flow ready for next time.
                s_DeviceRegistrationTcs = null;
            }
        }

        [AOT.MonoPInvokeCallback(typeof(NotificationReceivedCallback))]
        internal static void NotificationReceived(string serialisedNotificationData)
        {
            if (string.IsNullOrEmpty(serialisedNotificationData))
            {
                return;
            }

            Dictionary<string, object> userInfo = s_NotificationReceivedHandler.HandleReceivedNotification(serialisedNotificationData);
            InternalNotificationWasReceived?.Invoke(userInfo);
        }
    }
}
