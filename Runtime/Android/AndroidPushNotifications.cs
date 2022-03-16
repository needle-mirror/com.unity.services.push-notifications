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

        public AndroidPushNotifications(PushNotificationReceivedHandler notificationReceivedHandler, PushNotificationAnalytics analytics)
            : base("com.unity.services.pushnotifications.android.UnityCallbackClass")
        {
            m_NotificationReceivedHandler = notificationReceivedHandler;
            m_NotificationAnalytics = analytics;
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

                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject applicationContext = activity.Call<AndroidJavaObject>("getApplicationContext");

                AndroidJavaObject instance = GetPluginInstance();
                instance.Call("setCallbackClass", this);
                instance.Call("initialize", applicationContext,
                    firebaseApiKey, firebaseApplicationId, firebaseSenderId, firebaseProjectId);

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

