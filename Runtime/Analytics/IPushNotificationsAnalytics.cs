using System;
using System.Collections.Generic;

namespace Unity.Services.PushNotifications
{
	/// <summary>
	/// This interface is no longer required. Notification events are recorded for you automatically when you register for push notifications
	/// using RegisterForPushNotificationsAsync.
	/// </summary>
    [Obsolete("This interface should not be used. It will be deleted in the future version. Notification events are recorded for you automatically.")]
    public interface IPushNotificationsAnalytics
    {
        /// <summary>
        /// RecordPushTokenUpdated is no longer required. Notification events are recorded for you automatically when you register for push notifications using <see cref="RegisterForPushNotificationsAsync"/>.
        /// </summary>
        /// <param name="pushToken">Please use the RegisterForPushNotificationsAsync method</param>
        void RecordPushTokenUpdated(string pushToken);

        /// <summary>
        /// RecordNotificationOpened is no longer required. Notification events are recorded for you automatically when you register for push notifications using <see cref="RegisterForPushNotificationsAsync"/>.
        /// </summary>
		/// <param name="payload">Please use the RegisterForPushNotificationsAsync method</param>
		/// <param name="didLaunch">Please use the RegisterForPushNotificationsAsync method</param>
        void RecordNotificationOpened(Dictionary<string, object> payload, bool didLaunch);
    }

    [Obsolete("Stub to help transition away from IPushNotificationsAnalytics. This should ensure deprecation warnings appear only for the correct (exposed) elements, not for any internal parts.")]
    internal class PushNotificationsAnalyticsStub : IPushNotificationsAnalytics
    {
        readonly IPushAnalytics m_Actual;

        public PushNotificationsAnalyticsStub(IPushAnalytics actual)
        {
            m_Actual = actual;
        }

        public void RecordPushTokenUpdated(string pushToken)
        {
            m_Actual.RecordPushTokenUpdated(pushToken);
        }

        public void RecordNotificationOpened(Dictionary<string, object> payload, bool didLaunch)
        {
            m_Actual.RecordNotificationOpened(payload, didLaunch);
        }
    }
}
