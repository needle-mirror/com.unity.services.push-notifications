# Push Notifications SDK documentation

The Push Notifications SDK allows you to send push notification campaigns, including rich push notifications with images to your users.

## Platform support

This SDK supports both iOS and Android. iOS 10+ and Android SDK >= 26 (Oreo) are supported.

## Quick Start

The SDK comes with a sample script that will register for push notifications. Add this to your project and set the relevant settings in the editor under Project Settings.

## Registering for Push Notifications

To register for push notifications, three steps are required.

First, you need to populate some settings in the editor. These can be found in Project Settings > Services > Push Notifications.

Next, you need to initialise Unity Services, so that the required analytics events can be sent to Unity Analytics. You also need to implement the privacy flow, as detailed in the Analytics documentation, for the required events to be sent correctly.

You can then register for notifications. Ideally, to ensure no notifications are missed, this should be done in the startup code for your game. However, note that on first registration on iOS a user will be shown a permission request, so also ensure this call is made at a convenient place in your game. The SDK will handle the showing of notification content, including images, titles and message body.

A full code sample is shown below.

```cs
await UnityServices.InitializeAsync();   
// Note: This is the minimum required in Analytics version 3.0.0 and above to ensure the events with the push notification data are sent correctly.
// In a real game you would need to handle privacy consent states here, see the Analytics documentation for more details.
await AnalyticsService.Instance.CheckForRequiredConsents();

try
{
    string pushToken = await PushNotificationService.Instance.RegisterForPushNotificationsAsync();
    
    PushNotificationService.Instance.OnNotificationReceived += notificationData =>
    {
        Debug.Log("Received a notification!");
    };
}
catch (Exception e)
{
    Debug.Log("Failed to retrieve a push notification token.");
}
```

### Notification received callbacks

You can register a delegate to receive a C# event callback when a notification is received, if you wish to perform custom behavior at that point. To do this, add a delegate / method callback to `PushNotifications.OnNotificationReceived` as shown in the sample above.

### Push Notification settings

The SDK requires a number of settings in order to function correctly. Some settings are only used on a certain platform, indicated in the setting name. The following settings can be set:

* AndroidApiKey: The API key for a Firebase project to be used for Android's Firebase Cloud Messaging API. This can be found in your Firebase dashboard.
* AndroidSenderId: The sender ID to be used for Android's Firebase Cloud Messaging. This can be found in your Firebase dashboard.
* AndroidApplicationId: The application ID for a Firebase application to be used for Android's Firebase Cloud Messaging API. This can be found in your Firebase dashboard.
* AndroidProjectId: The project ID for a Firebase project to be used for Android's Firebase Cloud Messaging API. This can be found in your Firebase dashboard.

These settings are set in Project Settings -> Services -> Push Notifications.

### Analytics

The SDK will record two analytics events:

* `notificationServices`: This event is recorded whenever a new token is registered on the client. It contains the push token and is used to register this token with the backend service. This allows you to send notifications from the Unity Dashboard, and is required for proper functionality of this product.
* `notificationOpened`: This event is recorded whenever a notification is opened by a user. It contains data regarding which campaign and cohort the user was in, and whether the app was launched from the notification.

#### Analytics only mode

*WARNING: This section is only applicable if you're trying to use the Unity Dashboard Push Notification service alongside a separate Push Notification implementation. For most users this section is not required or recommended as it'll lead to reduced functionality of the product*

It's possible to integrate the SDK with an existing push notification service if required. To do so, do not call the registration methods as indicated above, and instead use the two methods in `PushNotificationService.Instance.Analytics` alongside your existing implementation.

`RecordPushTokenUpdated` should be called when you receive a new push token for a device. Note that the OS may create a new token at multiple points in the app's lifecycle, so call this whenever the token changes, and not just at startup.

`RecordNotificationOpened` should be called when a notification is opened. It takes a dictionary that is the data the notification payload contained, and a boolean flag to indicate whether the app was launched from the notification or not.

Note that this should allow you to send and schedule notifications from the Unity Dashboard. However, it greatly depends on the other push notification implementation you have implemented, and may lead to missing images or other content in notifications, so it's strongly recommended to use the standard set up, with this SDK being the only Push Notification solution integrated, if possible.
