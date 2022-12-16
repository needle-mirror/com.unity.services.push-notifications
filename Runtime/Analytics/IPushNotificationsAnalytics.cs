using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    public interface IPushNotificationsAnalytics
    {
        void RecordPushTokenUpdated(string pushToken);
        void RecordNotificationOpened(Dictionary<string, object> payload, bool didLaunch);
    }
}
