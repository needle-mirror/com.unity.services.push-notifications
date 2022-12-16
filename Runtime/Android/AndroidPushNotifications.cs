using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    class AndroidPushNotifications: AndroidJavaProxy
    {
        static object s_RegistrationLock = new object();
        static TaskCompletionSource<string> s_DeviceRegistrationTcs;
        static string s_DeviceToken;

        PushNotificationReceivedHandler m_NotificationReceivedHandler;
        PushNotificationAnalytics m_NotificationAnalytics;

        AndroidJavaObject m_IntentExtras;

        public AndroidPushNotifications(PushNotificationReceivedHandler notificationReceivedHandler, PushNotificationAnalytics analytics)
            : base("com.unity.services.pushnotifications.android.UnityCallbackClass")
        {
            m_NotificationReceivedHandler = notificationReceivedHandler;
            m_NotificationAnalytics = analytics;

            try
            {
                AndroidJavaObject intent = GetCurrentActivity().Call<AndroidJavaObject>("getIntent");
                string intentNotificationData = intent.Call<string>("getStringExtra", "notificationData");

                if (intentNotificationData != null)
                {
                    Debug.Log("App launched from notification, sending relevant events");
                    OnNotificationReceived(intentNotificationData);

                    // remove opened notificationData so it cannot be re-processed
                    intent.Call("removeExtra", "notificationData");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to check intent for notification data: {e.Message}");
            }
        }

        internal event Action<Dictionary<string, object>> InternalNotificationWasReceived;

        public Task<string> RegisterForPushNotificationsAsync(string firebaseApiKey, string firebaseSenderId, string firebaseApplicationId, string firebaseProjectId)
        {
            lock (s_RegistrationLock)
            {
                if (!string.IsNullOrEmpty(s_DeviceToken))
                {
                    return Task.FromResult(s_DeviceToken);
                }

                if (s_DeviceRegistrationTcs != null)
                {
                    return s_DeviceRegistrationTcs.Task;
                }

                s_DeviceRegistrationTcs = new TaskCompletionSource<string>();

                try
                {
                    AndroidJavaObject applicationContext = GetCurrentActivity().Call<AndroidJavaObject>("getApplicationContext");

                    AndroidJavaObject instance = GetPluginInstance();
                    instance.Call("setCallbackClass", this);
                    instance.Call("initialize", applicationContext,
                        firebaseApiKey, firebaseApplicationId, firebaseSenderId, firebaseProjectId);
                }
                catch (Exception e)
                {
                    s_DeviceRegistrationTcs.TrySetException(new Exception($"Failed to initialize Push Notification plugin instance and register the device for remote notifications"));
                }

                return s_DeviceRegistrationTcs.Task;
            }
        }

        AndroidJavaObject GetPluginInstance()
        {
            AndroidJavaClass notificationPluginObject = new AndroidJavaClass("com.unity.services.pushnotifications.android.UnityNotifications");
            AndroidJavaObject instance = notificationPluginObject.GetStatic<AndroidJavaObject>("INSTANCE");
            if (instance == null)
            {
                Debug.LogError("Unity Push Notification Android plugin is missing, android push notifications will not work as expected.");
                return null;
            }

            return instance;
        }

        AndroidJavaObject GetCurrentActivity()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }

        // Called from the Kotlin code
        internal void OnTokenReceived(string token)
        {
            lock (s_RegistrationLock)
            {
                if (s_DeviceRegistrationTcs != null)
                {
                    if (String.IsNullOrEmpty(token))
                    {
                        s_DeviceRegistrationTcs.TrySetException(new Exception("Failed to register the device for remote notifications."));
                    }
                    else
                    {
                        s_DeviceRegistrationTcs.TrySetResult(token);
                    }
                    // Reset registration flow ready for next time.
                    s_DeviceRegistrationTcs = null;
                }

                if (!String.IsNullOrEmpty(token))
                {
                    s_DeviceToken = token;

                    MainThreadHelper.RunOnMainThread(() =>
                    {
                        m_NotificationAnalytics.RecordPushTokenUpdated(token);
                    });

                    Debug.Log($"Successfully registered for remote push notifications with token: {token}");
                }
            }
        }

        // Called from the Kotlin code
        internal void OnNotificationReceived(string notificationDataAsJson)
        {
            if (string.IsNullOrEmpty(notificationDataAsJson))
            {
                Debug.Log("Notification received with no data, ignoring");
                return;
            }

            MainThreadHelper.RunOnMainThread(() =>
            {
                Dictionary<string, object> notificationData = m_NotificationReceivedHandler.HandleReceivedNotification(notificationDataAsJson);
                InternalNotificationWasReceived?.Invoke(notificationData);
            });
        }
    }
}

