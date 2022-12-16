using System;
using Unity.Services.Core;
using Unity.Services.PushNotifications;
using UnityEngine;
using Unity.Services.Analytics;

public class PushNotificationExample : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();

        // Note: This is the minimum required to ensure the events with the push notification data are sent correctly through Analytics.
		    // In a real game you would need to handle privacy consent states here, see the Analytics documentation for more details.
        await AnalyticsService.Instance.CheckForRequiredConsents();

        // Make sure to set the required settings in Project Settings before testing
        string token = await PushNotificationsService.Instance.RegisterForPushNotificationsAsync();
        Debug.Log($"The push notification token is {token}");

        PushNotificationsService.Instance.OnNotificationReceived += notificationData =>
        {
            Debug.Log("Data retrieved!");
        };
    }
}
