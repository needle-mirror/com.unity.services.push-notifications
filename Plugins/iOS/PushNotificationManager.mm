#import <Foundation/Foundation.h>
#import <objc/runtime.h>
#import <UIKit/UIKit.h>

#import "PushNotificationManager.h"

//MARK: Internal Class. Not publicly supported. Do not use directly.
@implementation PushNotificationManager : NSObject

NSArray<NSDictionary *> *bufferedNotifications;

NSString *launchedNotificationString;

+ (void)load {
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        [PushNotificationManager sharedInstance];

        UNUserNotificationCenter* center = [UNUserNotificationCenter currentNotificationCenter];
        center.delegate = [PushNotificationManager sharedInstance];
    });
}

// MARK: Interface implementations

+ (instancetype)sharedInstance
{
    static PushNotificationManager *sharedInstance = nil;

    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        sharedInstance = [[PushNotificationManager alloc] init];
        bufferedNotifications = [[NSArray alloc] init];
        launchedNotificationString = nil;

        NSNotificationCenter *nc = [NSNotificationCenter defaultCenter];

        [nc addObserver:sharedInstance
               selector:@selector(didRegisterForRemoteNotifications:)
                   name:kUnityDidRegisterForRemoteNotificationsWithDeviceToken
                 object:nil];

        [nc addObserver:sharedInstance
               selector:@selector(didFailToRegisterForRemoteNotifications:)
                   name:kUnityDidFailToRegisterForRemoteNotificationsWithError
                 object:nil];

        [nc addObserver:sharedInstance
               selector:@selector(didReceiveNotification:)
                   name:kUnityDidReceiveRemoteNotification
                 object:nil];

        [nc addObserver:sharedInstance
               selector:@selector(createLaunchedNotificationChecker:)
                   name:@"UIApplicationDidFinishLaunchingNotification"
                 object:nil];
    });

    return sharedInstance;
}

/// Called externally to register for push notifications. Initally requests notification permissions,
/// then registers to get a device token, which will be handled by one of the notification handlers.
- (void) registerForRemoteNotifications : (REGISTRATION_CALLBACK) callback
{
    // If we've already registered for a token previously, and have one ready, then just return it without
    // requesting authorisation again.
    if (_deviceToken != NULL && _deviceToken.length > 0) {
        callback((char *)_deviceToken.UTF8String);
        return;
    }

    _registrationCallback = callback;

    // Register for the permissions to display notifications from the iOS system. Without this, we'd receive
    // the pushes but wouldn't be able to display them to a user.
    UNUserNotificationCenter *center = [UNUserNotificationCenter currentNotificationCenter];
    NSInteger authOptions = (UNAuthorizationOptionSound + UNAuthorizationOptionAlert + UNAuthorizationOptionBadge);
    [center requestAuthorizationWithOptions:authOptions completionHandler:^(BOOL granted, NSError *error) {
        if (granted) {
            dispatch_async(dispatch_get_main_queue(), ^{
                [[UIApplication sharedApplication] registerForRemoteNotifications];
            });
        }
    }];
}

// MARK: Delegate methods

- (void) userNotificationCenter:(UNUserNotificationCenter *)center willPresentNotification:(UNNotification *)notification withCompletionHandler:(void (^)(UNNotificationPresentationOptions))completionHandler
{
    NSDictionary *userInfo = notification.request.content.userInfo;
    [self handleReceivedNotificationUserInfo:userInfo];
    completionHandler((UNAuthorizationOptionSound + UNAuthorizationOptionAlert + UNAuthorizationOptionBadge));
}

// MARK: Internal methods

- (void) didRegisterForRemoteNotifications: (NSNotification *) notification
{
    if ([notification.userInfo isKindOfClass:[NSData class]])
    {
        NSString *token = [[PushNotificationManager sharedInstance] getDeviceTokenFromNSData: (NSData*)notification.userInfo];
        [PushNotificationManager sharedInstance].deviceToken = token;
        [PushNotificationManager sharedInstance].registrationCallback((char *)token.UTF8String);
    }
}

- (void) didFailToRegisterForRemoteNotifications: (NSNotification *) notification
{
    NSLog(@"Failed to register for push notifications");
}

- (void) didReceiveNotification: (NSNotification *) notification
{
    NSDictionary *userInfoDictionary = notification.userInfo;
    [self handleReceivedNotificationUserInfo:userInfoDictionary];
}

- (void) handleReceivedNotificationUserInfo: (NSDictionary *)userInfoDictionary
{
    if ([PushNotificationManager sharedInstance].notificationCallback == nil) {
        // Unity hasn't had time to register the callback yet, so let's store the notification data until we're ready for it.
        bufferedNotifications = [bufferedNotifications arrayByAddingObject:userInfoDictionary];
        return;
    }

    char *jsonString = [self serializeRemoteNotification:userInfoDictionary];

    [PushNotificationManager sharedInstance].notificationCallback(jsonString);
}

- (void) flushNotificationBuffer
{
    NSMutableArray *tempBuffer = [NSMutableArray arrayWithArray:bufferedNotifications];
    NSMutableArray *emptyBuffer = [NSMutableArray arrayWithArray:bufferedNotifications];

    [emptyBuffer removeAllObjects];
    bufferedNotifications = [NSArray arrayWithArray:emptyBuffer];

    for (NSDictionary *bufferedData in tempBuffer) {
            [self handleReceivedNotificationUserInfo:bufferedData];
    }
}

- (char *) serializeRemoteNotification: (NSDictionary *)notificationDictionary
{
    NSError *error = nil;
    NSData *data = [NSJSONSerialization dataWithJSONObject:notificationDictionary options:kNilOptions error:&error];

    if (error != nil)
    {
        NSLog(@"Failed to serialize notification user info to string. User info was: %@", notificationDictionary);
        return nil;
    }

    char *jsonString = (char *) [[[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding] UTF8String];

    return jsonString;
}

/// Checks if app was launched from notification and sends to notification handler
- (void)createLaunchedNotificationChecker:(NSNotification *)notification
{
    NSDictionary *launchOptions = [notification userInfo];
    NSDictionary *launchedNotification = [launchOptions objectForKey: @"UIApplicationLaunchOptionsRemoteNotificationKey"];

    if (launchedNotification)
    {
        char *serializedString = [self serializeRemoteNotification:launchedNotification];

        launchedNotificationString = [NSString stringWithFormat:@"%s", serializedString];
    }
}

- (char *) getLaunchedNotificationString
{
    return [[PushNotificationManager sharedInstance] convertNSStringToCString: launchedNotificationString];
}

- (void) resetLaunchedNotificationString
{
    launchedNotificationString = nil;
}

// MARK: Helper methods

- (NSString *)getDeviceTokenFromNSData:(NSData *)deviceTokenData {
    const char *data = (const char *)[deviceTokenData bytes];
    NSMutableString *token = [NSMutableString string];

    for (NSUInteger i = 0; i < [deviceTokenData length]; i++) {
        [token appendFormat:@"%02.2hhX", data[i]];
    }

    return [token copy];
}

- (char *) convertNSStringToCString: (NSString*) nsString
{
    if (nsString == nil)
        return nil;

    const char* nsStringUtf8 = [nsString UTF8String];
    //create a null terminated C string on the heap so that our string's memory isn't wiped out right after method's return
    char* cString = (char*)malloc(strlen(nsStringUtf8) + 1);
    strcpy(cString, nsStringUtf8);

    return cString;
}

@end
