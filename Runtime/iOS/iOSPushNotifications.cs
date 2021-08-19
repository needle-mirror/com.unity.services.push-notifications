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

        internal static event Action<Dictionary<string, object>> InternalNotificationWasReceived;

        delegate void NotificationRegistrationCallback(string deviceToken);

        delegate void NotificationReceivedCallback(string serialisedNotificationData);

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern void NativeRegisterForPushNotifications(NotificationRegistrationCallback callback);
        
        [DllImport("__Internal")]
        static extern void RegisterUnityCallbackForNotificationReceived(NotificationReceivedCallback callback);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        internal static void PerformLaunchActions()
        {
            RegisterUnityCallbackForNotificationReceived(NotificationReceived);
        }
#endif
        
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
                if (String.IsNullOrEmpty(token))
                {
                    s_DeviceRegistrationTcs.TrySetException(new Exception("Failed to register the device for remote notifications."));
                }
                else
                {
                    s_DeviceToken = token;
                    s_DeviceRegistrationTcs.TrySetResult(token);
                    PushNotifications.Analytics.RecordPushTokenUpdated(token);
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

            Dictionary<string, object> userInfo = PushNotifications.notificationReceivedHandler.HandleReceivedNotification(serialisedNotificationData);
            InternalNotificationWasReceived?.Invoke(userInfo);
        }
    }
}
