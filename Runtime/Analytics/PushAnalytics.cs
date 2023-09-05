using System;
using System.Collections.Generic;
using Unity.Services.Core.Analytics.Internal;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    interface IPushAnalytics
    {
        void RecordPushTokenUpdated(string pushToken);
        void RecordNotificationOpened(Dictionary<string, object> payload, bool didLaunch);
    }

    internal class PushAnalytics : IPushAnalytics
    {
        const string k_PackageName = "com.unity.services.push-notifications";

        readonly IAnalyticsStandardEventComponent m_Analytics;
        readonly ISystemWrapper m_Platform;

        internal PushAnalytics(IAnalyticsStandardEventComponent analytics, ISystemWrapper platform)
        {
            m_Analytics = analytics;
            m_Platform = platform;
        }

        public void RecordPushTokenUpdated(string pushToken)
        {
            var eventParams = new Dictionary<string, object>();

            switch (m_Platform.RuntimePlatform())
            {
                case RuntimePlatform.Android:
                    eventParams.Add("androidRegistrationID", pushToken);
                    break;
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.tvOS:
                    eventParams.Add("pushNotificationToken", pushToken);
                    break;
            }

            m_Analytics.Record("notificationServices", eventParams, 1, k_PackageName);
        }

        public void RecordNotificationOpened(Dictionary<string, object> payload, bool didLaunch)
        {
            var eventParams = new Dictionary<string, object>();

            bool insertCommunicationAttrs = false;

            if (payload.ContainsKey("_ddCampaign"))
            {
                eventParams.Add("campaignId", Convert.ToInt64(payload["_ddCampaign"]));
                insertCommunicationAttrs = true;
            }

            if (payload.ContainsKey("_ddCohort"))
            {
                eventParams.Add("cohortId", Convert.ToInt64(payload["_ddCohort"]));
                insertCommunicationAttrs = true;
            }

            if (insertCommunicationAttrs &&
                payload.ContainsKey("_ddCommunicationSender"))
            {
                eventParams.Add("communicationSender", payload["_ddCommunicationSender"]);
            }

            if (didLaunch)
            {
                eventParams.Add("notificationLaunch", true);
            }

            if (payload.ContainsKey("_ddId"))
            {
                eventParams.Add("notificationId", Convert.ToInt64(payload["_ddId"]));
            }

            if (payload.ContainsKey("_ddName"))
            {
                eventParams.Add("notificationName", payload["_ddName"]);
            }

            eventParams.Add("communicationState", "OPEN");

            m_Analytics.Record("notificationOpened", eventParams, 1, k_PackageName);
        }
    }
}
