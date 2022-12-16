using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    class PushNotificationReceivedHandler
    {
        readonly IPushNotificationAnalyticsPlatformWrapper m_PlatformWrapper;
        readonly IPushNotificationsAnalytics m_NotificationAnalytics;

        bool m_IsCleanStart = true;

        internal PushNotificationReceivedHandler(IPushNotificationsAnalytics analytics, IPushNotificationAnalyticsPlatformWrapper platformWrapper)
        {
            m_NotificationAnalytics = analytics;
            m_PlatformWrapper = platformWrapper;
        }

        internal Dictionary<string, object> HandleReceivedNotification(string jsonNotificationData)
        {
            try
            {
                Dictionary<string, object> notificationData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonNotificationData);
                if (notificationData != null)
                {
                    // If the application is in the background, or we're opening for the first time, then we want to send the relevant analytics.
                    // (Note: This doesn't 100% guarantee that we're launching from an event, but it matches the previous deltaDNA behavior).
                    if (!m_PlatformWrapper.IsApplicationFocused() || m_IsCleanStart)
                    {
                        m_NotificationAnalytics.RecordNotificationOpened(notificationData, true);
                        m_IsCleanStart = false;
                    }
                    else
                    {
                        m_NotificationAnalytics.RecordNotificationOpened(notificationData, false);
                    }
                }

                return notificationData;
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to parse notification user info dictionary data: {e.Message}");
                return null;
            }
        }
    }
}
