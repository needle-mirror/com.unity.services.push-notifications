#import "NotificationService.h"

@interface NotificationService ()

@property (nonatomic, strong) void (^contentHandler)(UNNotificationContent *contentToDeliver);
@property (nonatomic, strong) UNMutableNotificationContent *bestAttemptContent;

@end

@implementation NotificationService

- (void)didReceiveNotificationRequest:(UNNotificationRequest *)request withContentHandler:(void (^)(UNNotificationContent * _Nonnull))contentHandler {
    /*
     Store the iOS notification content handler, and the best attempt we have at notification content,
     so we can then return the content for the notification to iOS either when we have finished processing
     it, or when the system is about to expire our notification extension's alloted time to handle the
     notification (e.g. if the network times out while fetching an attachment, for example).
    */
    self.contentHandler = contentHandler;
    self.bestAttemptContent = [request.content mutableCopy];
    
    NSDictionary *userInfo = [request.content.userInfo objectForKey:@"aps"];
    NSString *imageUrl = [userInfo objectForKey:@"imageUrl"];
    
    if (imageUrl == nil) {
        // If we have no image, then send a plain text notification with a header and text content
        // as provided by the system by default.
        [self contentComplete];
        return;
    }

    NSString *fileExt = [@"." stringByAppendingString:[imageUrl pathExtension]];
    NSURL *attachmentUrl = [NSURL URLWithString:imageUrl];
    
    NSURLSession *session = [NSURLSession sessionWithConfiguration:[NSURLSessionConfiguration defaultSessionConfiguration]];
    [[session downloadTaskWithURL:attachmentUrl
                completionHandler:^(NSURL *temporaryFileLocation, NSURLResponse *response, NSError *error) {
        UNNotificationAttachment *attachment = nil;
        if (error != nil) {
            NSLog(@"Failed to retrieve attachment contents for notification: %@", error.localizedDescription);
        } else {
            NSFileManager *fileManager = [NSFileManager defaultManager];
            NSURL *localUrl = [NSURL fileURLWithPath:[temporaryFileLocation.path stringByAppendingString:fileExt]];
            [fileManager moveItemAtURL:temporaryFileLocation toURL:localUrl error:&error];
            
            NSError *attachmentError = nil;
            attachment = [UNNotificationAttachment attachmentWithIdentifier:@"" URL:localUrl options:nil error:&attachmentError];
            if (attachmentError) {
                NSLog(@"Failed to create attachment from attachment contents for notification: %@", attachmentError.localizedDescription);
            }
        }
        if (attachment) {
            self.bestAttemptContent.attachments = [NSArray arrayWithObject:attachment];
        }
        [self contentComplete];
    }] resume];
}

- (void)contentComplete {
    self.contentHandler(self.bestAttemptContent);
}

- (void)serviceExtensionTimeWillExpire {
    [self contentComplete];
}

@end
