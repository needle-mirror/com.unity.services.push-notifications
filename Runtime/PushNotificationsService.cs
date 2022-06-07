using System;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
    public static class PushNotificationsService
    {
        internal static PushNotificationsServiceInstance internalInstance;

        public static IPushNotificationsService Instance
        {
            get
            {
                if (internalInstance == null)
                {
                    throw new Exception("The SDK hasn't been initialised - please make sure you've called `await UnityServices.InitializeAsync()` before using the Push Notifications SDK.");
                }

                return internalInstance;
            }
        }
    }
}
