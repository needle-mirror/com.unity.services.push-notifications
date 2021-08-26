# Push Notifications SDK Documentation

The Push Notifications SDK allows you to send push notification campaigns, including rich push notifications with images, to your users.

## Platform Support

This SDK supports both iOS and Android. iOS 10+ and Android SDK >= 26 (Oreo) are supported.

## Quick Start

The SDK comes with a sample script that will register for push notifications. Simply add this to your project, and replace the relevant settings, to get going.

## Registering for Push Notifications

To register for push notifications, two steps are required. Firstly, you need to initialise Unity Services, so that the required analytics events can be sent to Unity Analytics 2.0.

Once that is complete, you can then register for notifications. Ideally, to ensure no notifications are missed, this should be done in the startup code for your game. However, note that on first registration on iOS, a user will be shown a permission request, so also ensure this call is made at a convenient place in your game. The SDK will handle the showing of notification content, including images, titles and message body.

A full code sample is shown below.

```cs
await UnityServices.InitializeAsync();   

// Replace the below values with the relevant settings for your project, as discussed in the documentation below.     
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
```

### Notification Received Callbacks

You can register a delegate to receive a C# event callback when a notification is received, if you wish to perform custom behaviour at that point. To do this, add a delegate / method callback to `PushNotifications.OnNotificationReceived` as indicated in the sample above.

### Push Notification Settings

The SDK requires a number of settings in order to function correctly. Some settings are only used on a certain platform, this is indicated in the setting name. The following settings can be set:

* AndroidApiKey: The API key for a Firebase project to be used for Android's Firebase Cloud Messaging API. This can be found in your Firebase dashboard.
* AndroidSenderId: The sender ID to be used for Android's Firebase Cloud Messaging. This can be found in your Firebase dashboard.
* AndroidApplicationId: The application ID for a Firebase application to be used for Android's Firebase Cloud Messaging API. This can be found in your Firebase dashboard.
* AndroidProjectId: The project ID for a Firebase project to be used for Android's Firebase Cloud Messaging API. This can be found in your Firebase dashboard.

It is safe to have one settings object for all platforms, as only the settings relevant to the current platform will be used by the SDK.

### Analytics

The SDK will records two analytics events:

* `notificationServices`: This event is recorded whenever a new token is registered on the client. It contains the push token and is used to register this token with the backend service. This allows you to send notifications from the Unity Dashboard, and is required for proper functionality of this product.
* `notificationOpened`: This event is recorded whenever a notification is opened by a user. It contains data regarding which campaign and cohort the user was in, and whether the app was launched from the notification.

#### Analytics Only Mode

*WARNING: This section is only applicable if you are trying to use the Unity Dashboard Push Notification service alongside a separate Push Notification implementation. For most users this section is not required or recommended as it will lead to reduced functionality of the product*

It is possible to integrate the SDK with an existing push notification service if required. To do so, do not call the registration methods as indicated above, and instead use the two methods in the `PushNotifications.Analytics` class alongside your existing implementation.

`RecordPushTokenUpdated` should be called when you receive a new push token for a device. Note that the OS may create a new token at multiple points in the app's lifecycle, so be sure to call this whenever the token changes, and not just at startup.

`RecordNotificationOpened` should be called when a notification is opened. It takes a dictionary that is the data the notification payload contained, and a boolean flag to indicate whether the app was launched from the notification or not.

Note that this should allow you to send and schedule notifications from the Unity Dashboard. However, it greatly depends on the other push notification implementation you have implemented, and may lead to missing images or other content in notifications, so it is strongly recommended to use the standard set up, with this SDK being the only Push Notification solution integrated, if possible.
