using System;
using UnityEngine;

namespace Unity.Services.PushNotifications
{
	/// <summary>
	/// The main entry point for the Push Notifications SDK
	/// </summary>
    public static class PushNotificationsService
    {
        internal static PushNotificationsServiceInstance internalInstance;

        /// <summary>
        /// Returns the instance of the Push Notifications SDK
        /// </summary>
        /// <exception cref="Exception">Thrown when the SDK hasn't been initialised</exception>
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
