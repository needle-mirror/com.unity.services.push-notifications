using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    internal interface IPushPlatformCallbacks
    {
        void RemoteNotificationReceived(Dictionary<string, object> notificationData);
    }

    internal class PushNotificationsServiceInstance : IPushNotificationsService, IPushPlatformCallbacks
    {
        string m_DeviceToken;

        readonly IPushAnalytics m_Analytics;
        readonly IPushPlatform m_PlatformLogic;
        readonly ISystemWrapper m_PlatformWrapper;
        readonly IMainThreadHelper m_MainThreadHelper;

        /// <summary>
        /// This is no longer required. Notification events are recorded for you automatically when you register for push notifications using RegisterForPushNotificationsAsync.
        /// </summary>
        [Obsolete("Do not use this. It will be deleted in a future version. Notification events are recorded for you automatically.")]
        public IPushNotificationsAnalytics Analytics { get { return new PushNotificationsAnalyticsStub(m_Analytics); } }

        internal PushNotificationsServiceInstance(
            IPushAnalytics analytics,
            IPushPlatform platformSpecificLogic,
            ISystemWrapper platformWrapper,
            IMainThreadHelper mainThreadHelper)
        {
            m_Analytics = analytics;
            m_PlatformLogic = platformSpecificLogic;
            m_PlatformWrapper = platformWrapper;
            m_MainThreadHelper = mainThreadHelper;
        }

        event Action<Dictionary<string, object>> m_NotificationReceivedEvent;
        public event Action<Dictionary<string, object>> OnRemoteNotificationReceived
        {
            add
            {
                if (String.IsNullOrEmpty(m_DeviceToken))
                {
                    m_NotificationReceivedEvent += value;
                }
                else
                {
                    throw new InvalidOperationException("You may not subscribe to OnRemoteNotificationReceived after calling RegisterForPushNotificationsAsync.");
                }
            }
            remove => m_NotificationReceivedEvent -= value;
        }

        public async Task<string> RegisterForPushNotificationsAsync()
        {
            if (String.IsNullOrEmpty(m_DeviceToken))
            {
                PushNotificationSettings settings = m_PlatformWrapper.GetSettings();

                m_DeviceToken = await m_PlatformLogic.RegisterForPushNotifications(settings);
                if (String.IsNullOrEmpty(m_DeviceToken))
                {
                    throw new Exception("Failed to register the device for remote notifications.");
                }
                else
                {
                    // The underlying platform logic includes interop callbacks, so there's no guarantee we're back where we started.
                    // Therefore: force this bit onto the main thread so that we can use Unity APIs again.
                    m_MainThreadHelper.RunOnMainThread(CompleteRegistration);
                }
            }

            return m_DeviceToken;
        }

        void CompleteRegistration()
        {
            Debug.Log($"DeviceToken = {m_DeviceToken}");

            m_Analytics.RecordPushTokenUpdated(m_DeviceToken);

            Dictionary<string, object> launchNotification = m_PlatformLogic.CheckForAppLaunchByNotification();
            if (launchNotification != null)
            {
                m_Analytics.RecordNotificationOpened(launchNotification, true);
                m_NotificationReceivedEvent?.Invoke(launchNotification);
            }
        }

        internal void OnApplicationPause(bool isPaused)
        {
            m_PlatformLogic.OnApplicationPause(isPaused);
        }

        public void RemoteNotificationReceived(Dictionary<string, object> notificationData)
        {
            // Once again, this is invoked by underlying platform logic so there's no guarantee it's on the main thread.
            // Therefore: enforce it.
            m_MainThreadHelper.RunOnMainThread(() =>
            {
                ProcessRemoteNotification(notificationData);
            });
        }

        void ProcessRemoteNotification(Dictionary<string, object> notificationData)
        {
            if (notificationData != null)
            {
                // If the application is in the background then we want to send the relevant analytics.
                // (Note: This doesn't 100% guarantee that we're launching from an event, but it matches the previous deltaDNA behavior).
                m_Analytics.RecordNotificationOpened(notificationData, !m_PlatformWrapper.IsApplicationFocused());
            }

            m_NotificationReceivedEvent?.Invoke(notificationData);
        }
    }
}
