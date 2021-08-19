# Push Notifications SDK Documentation

The Push Notifications SDK allows you to send rich push notifications to mobile devices.

## Platform Support

This SDK supports both iOS and Android. iOS 10+ and Android SDK > 26 (Oreo) are supported.

## Quick Start

The SDK comes with a sample script that will register for push notifications. Simply add this to your project to get going.

## Registering for Push Notifications

To register for push notifications, two steps are required. Firstly, you need to initialise Unity Services, so that the required analytics events can be sent to Unity Analytics 2.0.

Once that is complete, you can then register for notifications. To ensure no notifications are missed, this should be done in the startup code for your game. Note that on first registration on iOS, a user will be shown a permission request, so ensure this call is made at a convenient place in your game. The SDK will handle the showing of notification content, including images, titles and message body.

A full code sample is shown below.

```cs
// In a monobehaviour in a convenient place in your game.
async void Start()
{
    await UnityServices.InitializeAsync();        
    PushNotificationSettings settings = new PushNotificationSettings()
    {
        AndroidApiKey = "API_KEY",
        AndroidSenderId = "SENDER_ID",
        AndroidApplicationId = "APPLICATION_ID",
        AndroidProjectId = "PROJECT_ID"
    };

    try
    {

        string pushToken = await PushNotifications.RegisterForPushNotificationsAsync(settings);
        
        PushNotifications.OnNotificationReceived += notificationData =>
        {
            Debug.Log("Received a notification!");
        };
    }
    catch (Exception e)
    {
        Debug.Log("Failed to retrieve a push notification token.");
    }
}
```

### Notification Received Callbacks

You can register a delegate to receive a C# event callback when a notification is received, if you wish to perform custom behaviour at that point. To do this, add a delegate / method callback to `PushNotifications.OnNotificationReceived` as indicated in the sample above.

### Push Notification Settings

The SDK requires a number of settings in order to function correctly. Some settings are only used on a certain platform, this is indicated in the setting name. The following settings can be set:

* AndroidApiKey: The API key for a Firebase project to be used for Android's Firebase Cloud Messaging API. This can be found in your Firebase dashboard.
* AndroidSenderId: The sender ID to be used for Android's Firebase Cloud Messaging. This can be found in your Firebase dashboard.
* AndroidApplicationId: The application ID for a Firebase application to be used for Android's Firebase Cloud Messaging API. This can be found in your Firebase dashboard.
* AndroidProjectId: The project ID for a Firebase project to be used for Android's Firebase Cloud Messaging API. This can be found in your Firebase dashboard.

### Analytics

The SDK will records two analytics events:

* `notificationServices`: This event is recorded whenever a new token is registered on the client. It contains the push token and is used to register this token with the backend service.
* `notificationOpened`: This event is recorded whenever a notification is opened by a user. It contains data regarding which campaign and cohort the user was in, and whether the app was launched from the notification.

## Analytics Only Mode

> **Note:** This section is only required if you have an existing push notification solution you wish to use alongside the Unity Push Notifications package.
> This will result in reduced functionality of this package - e.g. images in notifications will not be displayed.

It is possible to integrate the SDK with an existing push notification service if required. To do so, use the two methods in the `PushNotifications.Analytics` class. 

`RecordPushTokenUpdated` should be called when you receive