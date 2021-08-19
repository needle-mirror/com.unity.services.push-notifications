using System;
using Unity.Services.Core;
using Unity.Services.PushNotifications;
using UnityEngine;

public class PushNotificationExample : MonoBehaviour
{    
    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();

        // Update the below settings to match a project you use.
        PushNotificationSettings settings = new PushNotificationSettings()
        {
            AndroidApiKey = "API_KEY",
            AndroidSenderId = "SENDER_ID",
            AndroidApplicationId = "APPLICATION_ID",
            AndroidProjectId = "PROJECT_ID"
        };

        string token = await PushNotifications.RegisterForPushNotificationsAsync(settings);
        Debug.Log($"The push notification token is {token}");

        PushNotifications.OnNotificationReceived += notificationData =>
        {
            Debug.Log("Data retrieved!");
        };
    }
}
