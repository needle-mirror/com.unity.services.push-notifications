#ifndef PushNotificationManager_h
#define PushNotificationManager_h

#import "UnityAppController.h"
#import <UserNotifications/UserNotifications.h>

// Required for internal messaging constants.
#include "Classes/PluginBase/LifeCycleListener.h"
#include "Classes/PluginBase/AppDelegateListener.h"

typedef void (REGISTRATION_CALLBACK)(char *token);
typedef void (NOTIFICATION_CALLBACK)(char *serialisedUserInfo);

@interface PushNotificationManager  : NSObject <UNUserNotificationCenterDelegate>

@property NSString *deviceToken;
@property (nonatomic) REGISTRATION_CALLBACK *registrationCallback;
@property (nonatomic) NOTIFICATION_CALLBACK *notificationCallback;

+ (instancetype)sharedInstance;
- (void) registerForRemoteNotifications : (REGISTRATION_CALLBACK) callback;
- (char *) getLaunchedNotificationString;
- (void) resetLaunchedNotificationString;
- (void) flushNotificationBuffer;

@end

#endif

