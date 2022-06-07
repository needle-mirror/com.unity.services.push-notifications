using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;


namespace Unity.Services.PushNotifications
{
    [Obsolete("The interface provided by PushNotifications has moved to PushNotificationsService.Instance, and should be accessed from there instead. This API will be removed in an upcoming release.")]
    public class PushNotifications
    {
        [Obsolete("The interface provided by PushNotifications.OnNotificationReceived has moved to PushNotificationsService.Instance.OnNotificationReceived, and should be accessed from there instead. This API will be removed in an upcoming release.")]
        public static event Action<Dictionary<string, object>> OnNotificationReceived
        {
            add => PushNotificationsService.Instance.OnNotificationReceived += value;
            remove => PushNotificationsService.Instance.OnNotificationReceived -= value;
        }

        [Obsolete("The interface provided by PushNotifications.Analytics has moved to PushNotificationsService.Instance.Analytics, and should be accessed from there instead. This API will be removed in an upcoming release.")]
        public static PushNotificationAnalytics Analytics => PushNotificationsService.Instance.Analytics;

        /// <summary>
        /// Registers for push notifications with the appropriate mechanism for the current platform.
        ///
        /// This method will automatically handle platform specific intricacies of getting a push notification token, and will
        /// send the appropriate analytics events to Unity Analytics 2.
        /// </summary>
        /// <param name="settings">A PushNotificationSettings object with the settings for the SDK. See the documentation on that class for more information.</param>
        /// <returns>(Asynchronously via a Task) The device token as a string.</returns>
        [Obsolete("The interface provided by PushNotifications.RegisterForPushNotificationsAsync has moved to PushNotificationsService.Instance.RegisterForPushNotificationsAsync, and should be accessed from there instead. This API will be removed in an upcoming release.")]
        public static Task<string> RegisterForPushNotificationsAsync()
        {
            return PushNotificationsService.Instance.RegisterForPushNotificationsAsync();
        }
    }
}
