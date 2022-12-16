#import "PushNotificationManagerWrapper.h"

extern "C" {
    void NativeRegisterForPushNotifications(REGISTRATION_CALLBACK callback) {
        [[PushNotificationManager sharedInstance] registerForRemoteNotifications:callback];
    }

    void RegisterUnityCallbackForNotificationReceived(NOTIFICATION_CALLBACK callback) {
        [[PushNotificationManager sharedInstance] setNotificationCallback:callback];
        [[PushNotificationManager sharedInstance] flushNotificationBuffer];
    }

    char * GetLaunchedNotificationString() {
        return [[PushNotificationManager sharedInstance] getLaunchedNotificationString];
    }

    void ResetLaunchedNotificationString() {
        [[PushNotificationManager sharedInstance] resetLaunchedNotificationString];
    }
}


