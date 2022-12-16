using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    class PushNotificationsServiceInstance : IPushNotificationsService
    {
        PushNotificationsAnalyticsPlatformWrapper m_AnalyticsPlatformWrapper;
        PushNotificationAnalytics m_PushNotificationAnalyticsImpl;
        internal PushNotificationReceivedHandler notificationReceivedHandler;

#if UNITY_IOS
        IOSPushNotifications m_IOSPushNotifications;
#elif UNITY_ANDROID
        AndroidPushNotifications m_AndroidPushNotifications;
#endif

        internal PushNotificationsServiceInstance()
        {
            m_AnalyticsPlatformWrapper = new PushNotificationsAnalyticsPlatformWrapper();
            m_PushNotificationAnalyticsImpl = new PushNotificationAnalytics(new EventsWrapper(), m_AnalyticsPlatformWrapper);
            notificationReceivedHandler = new PushNotificationReceivedHandler(m_PushNotificationAnalyticsImpl, m_AnalyticsPlatformWrapper);

#if UNITY_IOS
            m_IOSPushNotifications = new IOSPushNotifications(notificationReceivedHandler, m_PushNotificationAnalyticsImpl);
#elif UNITY_ANDROID
            m_AndroidPushNotifications = new AndroidPushNotifications(notificationReceivedHandler, m_PushNotificationAnalyticsImpl);
#endif
        }

        public event Action<Dictionary<string, object>> OnNotificationReceived
        {
#if UNITY_IOS
            add => IOSPushNotifications.InternalNotificationWasReceived += value;
            remove => IOSPushNotifications.InternalNotificationWasReceived -= value;
#elif UNITY_ANDROID
            add => m_AndroidPushNotifications.InternalNotificationWasReceived += value;
            remove => m_AndroidPushNotifications.InternalNotificationWasReceived -= value;
#else
            add
            { /* No action on unsupported platforms */ }
            remove
            { /* No action on unsupported platforms */ }
#endif
        }

        public IPushNotificationsAnalytics Analytics => m_PushNotificationAnalyticsImpl;

        /// <summary>
        /// Registers for push notifications with the appropriate mechanism for the current platform.
        ///
        /// This method will automatically handle platform specific intricacies of getting a push notification token, and will
        /// send the appropriate analytics events to Unity Analytics 2.
        /// </summary>
        /// <returns>(Asynchronously via a Task) The device token as a string.</returns>
        public Task<string> RegisterForPushNotificationsAsync()
        {
            PushNotificationSettings settings = PushNotificationSettings.GetAssetInstance();
            return RegisterForPushNotificationsInternal(settings);
        }

        Task<string> RegisterForPushNotificationsInternal(PushNotificationSettings settings)
        {
#if UNITY_IOS
            return m_IOSPushNotifications.RegisterForPushNotificationsAsync();
#elif UNITY_ANDROID
            if (string.IsNullOrEmpty(settings.firebaseWebApiKey) || string.IsNullOrEmpty(settings.firebaseAppID) || string.IsNullOrEmpty(settings.firebaseProjectNumber) || string.IsNullOrEmpty(settings.firebaseProjectID))
            {
                throw new Exception("UGS Push Notifications is missing Android settings - make sure these are set in the editor Project Settings");
            }
            return m_AndroidPushNotifications.RegisterForPushNotificationsAsync(settings.firebaseWebApiKey, settings.firebaseProjectNumber, settings.firebaseAppID, settings.firebaseProjectID);
#else
            Debug.Log("Push notifications are not supported on this platform at this time, returning an empty push token");
            return Task.FromResult("");
#endif
        }
    }
}
