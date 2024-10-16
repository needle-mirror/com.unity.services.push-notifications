using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unity.Services.PushNotifications
{
    /// <summary>
    /// Interface representing a collection of methods to interact with the Push Notifications service.
    /// </summary>
    public interface IPushNotificationsService
    {
        /// <summary>
        /// Subscribe to this event to be notified when a remote notification is used to launch or reopen the app.
        ///
        /// You must set up event subscriptions before calling RegisterForPushNotificationsAsync.
        ///
        /// If the application was started from a remote notification, this event will be invoked
        /// once the RegisterForPushNotificationsAsync process has completed.
        /// </summary>
        public event Action<Dictionary<string, object>> OnRemoteNotificationReceived;

		/// <summary>
		/// This is no longer required. Notification events are recorded for you automatically when you register for push notifications
		/// using <see cref="RegisterForPushNotificationsAsync"/>.
		/// </summary>
        [Obsolete("Do not use this. It will be removed in a future version. Events are recorded for you automatically.")]
        public IPushNotificationsAnalytics Analytics { get; }

        /// <summary>
        /// Registers for push notifications with the appropriate mechanism for the current platform.
        ///
        /// This method will automatically handle platform specific intricacies of getting a push notification token, and will
        /// send the appropriate analytics events to Unity Analytics 2.
        /// </summary>
        /// <returns>(Asynchronously via a Task) The device token as a string.</returns>
        public Task<string> RegisterForPushNotificationsAsync();
    }
}
