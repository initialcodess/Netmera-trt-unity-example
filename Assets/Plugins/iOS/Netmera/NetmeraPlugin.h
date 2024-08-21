
@interface FNetmeraPlugin : NSObject
    +(void) onPushRegister:(NSString*)deviceToken;
    +(void) onPushReceive:(UNNotification*)notification;
    +(void) onPushDismiss:(UNNotificationResponse*)response;
    +(void) onPushOpen:(UNNotificationResponse *)response;
    +(void) NMLog:(NSString*) message;
    +(void) init:(BOOL)printLogs;
    +(void) setListener:(NSString*)unityListenerName;
    +(void) log:(NSString*)message;
@end
