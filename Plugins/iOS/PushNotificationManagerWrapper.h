#ifndef PushNotificationManagerWrapper_h
#define PushNotificationManagerWrapper_h

#import "PushNotificationManager.h"

extern "C" void NativeRegisterForPushNotifications(REGISTRATION_CALLBACK callback);
extern "C" void RegisterUnityCallbackForNotificationReceived(NOTIFICATION_CALLBACK callback);

#endif
