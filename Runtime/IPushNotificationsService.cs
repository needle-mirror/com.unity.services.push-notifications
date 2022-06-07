using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unity.Services.PushNotifications
{
    public interface IPushNotificationsService
    {
        public event Action<Dictionary<string, object>> OnNotificationReceived;
        public PushNotificationAnalytics Analytics { get; }
        public Task<string> RegisterForPushNotificationsAsync();
    }
}
