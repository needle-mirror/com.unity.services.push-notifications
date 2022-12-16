using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    static class SdkVersion
    {
        // This value should always match what is in the package.json for this package
        public static readonly string SDK_VERSION = "3.0.0-pre.1";
    }

    class PushNotificationAnalytics : IPushNotificationsAnalytics
    {
        IPushNotificationEventsWrapper m_EventsWrapper;
        IPushNotificationAnalyticsPlatformWrapper m_AnalyticsPlatformWrapper;

        internal PushNotificationAnalytics(IPushNotificationEventsWrapper eventsWrapper, IPushNotificationAnalyticsPlatformWrapper analyticsPlatformWrapper)
        {
            m_EventsWrapper = eventsWrapper;
            m_AnalyticsPlatformWrapper = analyticsPlatformWrapper;
        }

        /// <summary>
        /// This method should be called when the user's push notification token is updated. Most commonly this is
        /// when first registering for notifications, but can also occur at other points during the app lifecycle.
        /// If using one of this SDK's platform specific implementations this method will be called for you.
        /// </summary>
        /// <param name="pushToken">The push token relating to this user's device.</param>
        /// <returns></returns>
        public void RecordPushTokenUpdated(string pushToken)
        {
            Dictionary<string, object> eventData = new Dictionary<string, object>
            {
                { "clientVersion", m_AnalyticsPlatformWrapper.ApplicationVersion() },
                { "sdkVersion", SdkVersion.SDK_VERSION },
                { "sdkMethod", "com.unity.services.pushNotifications.PushNotificationsAnalytics.RecordPushTokenUpdated" },
                { "platform", m_AnalyticsPlatformWrapper.AnalyticsPlatform() }
            };

            RuntimePlatform runtimePlatform = m_AnalyticsPlatformWrapper.RuntimePlatform();
            if (runtimePlatform == RuntimePlatform.Android)
            {
                eventData.Add("androidRegistrationID", pushToken);
            }
            else if (runtimePlatform == RuntimePlatform.IPhonePlayer || runtimePlatform == RuntimePlatform.tvOS)
            {
                eventData.Add("pushNotificationToken", pushToken);
            }

            m_EventsWrapper.RecordCustomEvent("notificationServices", eventData, 1);
        }

        /// <summary>
        /// This method should be called when the user opens the app from a push notification.
        /// If using one of this SDK's platform specific implementations this method will be called for you.
        /// </summary>
        /// <param name="payload">The dictionary containing required the event data.</param>
        /// <param name="didLaunch">Was the app launched from opening the notification?</param>
        /// <returns></returns>
        public void RecordNotificationOpened(Dictionary<string, object> payload, bool didLaunch)
        {
            Dictionary<string, object> eventParams = new Dictionary<string, object>
            {
                { "clientVersion", m_AnalyticsPlatformWrapper.ApplicationVersion() },
                { "sdkVersion", SdkVersion.SDK_VERSION },
                { "sdkMethod", "com.unity.services.pushNotifications.PushNotificationsAnalytics.RecordNotificationOpened" },
                { "platform", m_AnalyticsPlatformWrapper.AnalyticsPlatform() }
            };

            bool insertCommunicationAttrs = false;

            if (payload.ContainsKey("_ddCampaign"))
            {
                eventParams["campaignId"] = Convert.ToInt64(payload["_ddCampaign"]);
                insertCommunicationAttrs = true;
            }
            if (payload.ContainsKey("_ddCohort"))
            {
                eventParams["cohortId"] = Convert.ToInt64(payload["_ddCohort"]);
                insertCommunicationAttrs = true;
            }
            if (insertCommunicationAttrs && payload.ContainsKey("_ddCommunicationSender"))
            {
                eventParams["communicationSender"] = payload["_ddCommunicationSender"];
                eventParams["communicationState"] = "OPEN";
            }

            if (didLaunch)
            {
                eventParams["notificationLaunch"] = true;
            }
            if (payload.ContainsKey("_ddId"))
            {
                eventParams["notificationId"] = Convert.ToInt64(payload["_ddId"]);
            }
            if (payload.ContainsKey("_ddName"))
            {
                eventParams["notificationName"] = payload["_ddName"];
            }
            eventParams["communicationState"] = "OPEN";

            m_EventsWrapper.RecordCustomEvent("notificationOpened", eventParams, 1);
        }
    }
}
