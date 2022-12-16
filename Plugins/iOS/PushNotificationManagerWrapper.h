#ifndef PushNotificationManagerWrapper_h
#define PushNotificationManagerWrapper_h

#import "PushNotificationManager.h"

extern "C" void NativeRegisterForPushNotifications(REGISTRATION_CALLBACK callback);
extern "C" void RegisterUnityCallbackForNotificationReceived(NOTIFICATION_CALLBACK callback);
extern "C" char * GetLaunchedNotificationString();
extern "C" void ResetLaunchedNotificationString();

#endif
