#if UNITY_IOS || UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Notifications.iOS;

namespace Unity.Services.PushNotifications
{
    class IOSPushNotifications : IPushPlatform
    {
        TaskCompletionSource<string> s_RegisterTcSource;

        IPushPlatformCallbacks m_PushNotificationsHandler;
        readonly ICoroutineRunner m_Container;

        public IOSPushNotifications(ICoroutineRunner container)
        {
            m_Container = container;
        }

        public void Initialize(IPushPlatformCallbacks callbacks)
        {
            m_PushNotificationsHandler = callbacks;
            iOSNotificationCenter.OnRemoteNotificationReceived += OnRemoteNotificationReceived;
        }

        /// <summary>
        /// Registers for push notifications on iOS. Returns the device token for the registered device.
        /// This method checks if the user grants the notification permission, it will register for push notification and get the device token.
        /// </summary>
        /// <returns>The push notification token for this device</returns>
        public Task<string> RegisterForPushNotifications(PushNotificationSettings settings)
        {
            // Check if a Task exists before re instantiating a new TaskCompletionSource
            if (s_RegisterTcSource != null)
            {
                return s_RegisterTcSource.Task;
            }
            s_RegisterTcSource = new TaskCompletionSource<string>();

            // NOTE: Settings is only required by Android, but is taken to ensure a consistent interface
            m_Container.StartCoroutine(RequestAuthorization(AuthorizationOption.Alert | AuthorizationOption.Badge | AuthorizationOption.Sound));
            return s_RegisterTcSource.Task;
        }

        IEnumerator RequestAuthorization(AuthorizationOption options)
        {
            using (AuthorizationRequest request = new AuthorizationRequest(options, true))
            {
                while (!request.IsFinished)
                {
                    yield return null;
                }

                if (s_RegisterTcSource == null)
                {
                    yield return null;
                }

                if (request.Granted)
                {
                    s_RegisterTcSource.TrySetResult(request.DeviceToken);
					s_RegisterTcSource = null;
                }
                else
                {
                    s_RegisterTcSource.TrySetException(new Exception("Authorization request was denied"));
					s_RegisterTcSource = null;
                }
            }
        }

        public Dictionary<string, object> CheckForAppLaunchByNotification()
        {
            // in case a killed app was launched by clicking a notification
            iOSNotification notification = iOSNotificationCenter.GetLastRespondedNotification();
            if (notification != null &&
                notification.Trigger.Type == iOSNotificationTriggerType.Push)
            {
                return ParseNotification(notification);
            }
            else
            {
                return null;
            }
        }

        public void OnApplicationPause(bool isPaused)
        {
            if (isPaused == false)
            {
                iOSNotification notification = iOSNotificationCenter.GetLastRespondedNotification();
                if (notification != null &&
                    notification.Trigger.Type == iOSNotificationTriggerType.Push)
                {
                    OnRemoteNotificationReceived(notification);
                }
            }
        }

        void OnRemoteNotificationReceived(iOSNotification notification)
        {
            Dictionary<string, object> notificationData = ParseNotification(notification);
            m_PushNotificationsHandler.RemoteNotificationReceived(notificationData);
        }

        Dictionary<string, object> ParseNotification(iOSNotification notification)
        {
            Dictionary<string, object> notificationData = new Dictionary<string, object>();
            notificationData["title"] = notification.Title;
            notificationData["alert"] = notification.Body;

            if (notification.UserInfo.ContainsKey("_ddCampaign"))
            {
                notificationData["_ddCampaign"] = notification.UserInfo["_ddCampaign"];
            }
            if (notification.UserInfo.ContainsKey("_ddCohort"))
            {
                notificationData["_ddCohort"] = notification.UserInfo["_ddCohort"];
            }
            Dictionary<string, object> aps = JsonConvert.DeserializeObject<Dictionary<string, object>>(notification.UserInfo["aps"]);;
            if (aps.ContainsKey("imageUrl"))
            {
                notificationData["imageUrl"] = aps["imageUrl"];
            }

            return notificationData;
        }
    }
}
#endif
