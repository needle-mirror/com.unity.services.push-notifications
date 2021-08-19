using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

[assembly: InternalsVisibleTo("Unity.Services.PushNotifications.Tests")]

namespace Unity.Services.PushNotifications
{
    public class PushNotifications
    {
        static PushNotificationsAnalyticsPlatformWrapper s_AnalyticsPlatformWrapper = new PushNotificationsAnalyticsPlatformWrapper();
        static PushNotificationAnalytics s_PushNotificationAnalyticsImpl = new PushNotificationAnalytics(new EventsWrapper(), s_AnalyticsPlatformWrapper);
        internal static PushNotificationReceivedHandler notificationReceivedHandler = new PushNotificationReceivedHandler(s_PushNotificationAnalyticsImpl, s_AnalyticsPlatformWrapper);

        internal static bool isInitialised = false;
        
#if UNITY_IOS
        static IOSPushNotifications s_IOSPushNotifications = new IOSPushNotifications();
#elif UNITY_ANDROID
        static AndroidPushNotifications s_AndroidPushNotifications = new AndroidPushNotifications(notificationReceivedHandler, s_PushNotificationAnalyticsImpl);
#endif
        
        public static event Action<Dictionary<string, object>> OnNotificationReceived
        {
#if UNITY_IOS
            add => IOSPushNotifications.InternalNotificationWasReceived += value;
            remove => IOSPushNotifications.InternalNotificationWasReceived -= value;
#elif UNITY_ANDROID
            add => s_AndroidPushNotifications.InternalNotificationWasReceived += value;
            remove => s_AndroidPushNotifications.InternalNotificationWasReceived -= value;
#else
            add
            { /* No action on unsupported platforms */ }
            remove
            { /* No action on unsupported platforms */ }
#endif
        }

        public static PushNotificationAnalytics Analytics
        {
            get
            {
                if (!isInitialised)
                {
                    throw new Exception("Unity services core hasn't been initialised - please make sure you've called `await UnityServices.InitializeAsync()` before using the Push Notifications SDK.");
                }
                return s_PushNotificationAnalyticsImpl;
            }
        } 

        /// <summary>
        /// Registers for push notifications with the appropriate mechanism for the current platform.
        ///
        /// This method will automatically handle platform specific intricacies of getting a push notification token, and will
        /// send the appropriate analytics events to Unity Analytics 2.
        /// </summary>
        /// <param name="settings">A PushNotificationSettings object with the settings for the SDK. See the documentation on that class for more information.</param>
        /// <returns>(Asynchronously via a Task) The device token as a string.</returns>
        public static Task<string> RegisterForPushNotificationsAsync(PushNotificationSettings settings)
        {
            if (!isInitialised)
            {
                throw new Exception("Unity services core hasn't been initialised - please make sure you've called `await UnityServices.InitializeAsync()` before using the Push Notifications SDK.");
            }
#if UNITY_IOS
            return s_IOSPushNotifications.RegisterForPushNotificationsAsync();
#elif UNITY_ANDROID
            return s_AndroidPushNotifications.RegisterForPushNotificationsAsync(settings.AndroidApiKey, settings.AndroidSenderId, settings.AndroidApplicationId, settings.AndroidProjectId);
#else
            Debug.Log("Push notifications are not supported on this platform at this time, returning an empty push token");
            return Task.FromResult("");
#endif
        }
    }
}
