//
//  AppDelegate+NetmeraPlugin.m
//  MyApp
//
//  Created by Enis Terzioğlu on 4.01.2021.
//

#import "NetmeraPlugin.h"
#import <objc/runtime.h>
#import <Foundation/Foundation.h>
#import <Netmera/Netmera.h>
#import <UserNotifications/UserNotifications.h>
#import "UnityAppController.h"

@interface OverrideAppDelegate : UnityAppController
@end

// this part is critical http://blog.eppz.eu/override-app-delegate-unity-ios-macos-1/
IMPL_APP_CONTROLLER_SUBCLASS(OverrideAppDelegate)

@implementation OverrideAppDelegate


static NSString *apnsToken;
static NSData *initialPushPayload;
static NSData *lastPush;

//Method swizzling
+ (void)load {
    [FNetmeraPlugin NMLog:@"swizzling load"];
    Method original =  class_getInstanceMethod(self, @selector(application:didFinishLaunchingWithOptions:));
    Method custom =    class_getInstanceMethod(self, @selector(application:customDidFinishLaunchingWithOptions:));
    method_exchangeImplementations(original, custom);
}

- (BOOL)application:(UIApplication*)application customDidFinishLaunchingWithOptions:(NSDictionary*)launchOptions {
    [self application:application customDidFinishLaunchingWithOptions:launchOptions];
    
    [FNetmeraPlugin NMLog:@"customDidFinishLaunchingWithOptions"];
    [UNUserNotificationCenter currentNotificationCenter].delegate = self;
    initNetmeraAuto(launchOptions);
 
    return YES;
}

- (BOOL)application:(UIApplication *)app openURL:(NSURL *)url options:(NSDictionary<UIApplicationOpenURLOptionsKey,id> *)options {
    return [super application:app openURL:url options:options];
}

void initNetmera(NSDictionary* launchOptions, NSString* netmeraSDKkey, NSString* baseUrl, NSNumber* logDisabled, NSNumber* popupDisabled, NSString* appGroupName) {
    BOOL printLogs = logDisabled ? [logDisabled boolValue] == 0 : false;
    if(printLogs) {
        [Netmera setLogLevel:(NetmeraLogLevelDebug)];
    }
    [FNetmeraPlugin init:printLogs];
    [Netmera start];
    // For On-premise setup
    // [Netmera setBaseURL:@"YOUR PANEL DOMAIN URL"];
    // This can be called later, see documentation for details
    [Netmera setAppGroupName:appGroupName];
    [Netmera setAPIKey:netmeraSDKkey];
    BOOL isPopupEnabled = popupDisabled ?  ![popupDisabled boolValue] : true;
    [Netmera setEnabledPopupPresentation: isPopupEnabled];
    
    
    if(baseUrl != nil && baseUrl.length > 0) {
        [Netmera setBaseURL:baseUrl];
    }
    [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"netmera init called -> netmeraSDKkey: %@ , baseUrl: %@ , logDisabled: %@ , popupDisabled: %@", netmeraSDKkey, baseUrl, logDisabled, popupDisabled]];
}


NSString* stringWithDeviceToken(NSData * deviceToken) {
    const char *data = [deviceToken bytes];
    NSMutableString *token = [NSMutableString string];

    for (NSUInteger i = 0; i < [deviceToken length]; i++) {
        [token appendFormat:@"%02.2hhX", data[i]];
    }

    return [token copy];
}


void initNetmeraAuto(NSDictionary* launchOptions) {
    NSBundle* mainBundle = [NSBundle mainBundle];
    NSString* sdkKey = [mainBundle objectForInfoDictionaryKey:@"netmera_mobile_sdkkey"];
    NSString* baseUrl = [mainBundle objectForInfoDictionaryKey:@"netmera_optional_baseurl"];
    NSNumber* loggingDisabled = [mainBundle objectForInfoDictionaryKey:@"netmera_logging_disabled"];
    NSNumber* netmera_popup_presentation_disabled = [mainBundle objectForInfoDictionaryKey:@"netmera_popup_presentation_disabled"];
    NSNumber* appGroupName = [mainBundle objectForInfoDictionaryKey:@"netmera_app_group_name"];
    initNetmera(launchOptions, sdkKey , baseUrl, loggingDisabled, netmera_popup_presentation_disabled, appGroupName);
}

NSDictionary* mapPushObject(NetmeraPushObject* pushObject)
{
    NSMutableDictionary *data = [NSMutableDictionary dictionary];
    data[@"pushId"] = [pushObject pushId];
    data[@"pushInstanceId"] = [pushObject pushInstanceId];
    data[@"pushType"] = [NSNumber numberWithInteger:[pushObject pushType]];
    data[@"title"] = [[pushObject alert] title];
    data[@"subtitle"] = [[pushObject alert] subtitle];
    data[@"body"] = [[pushObject alert] body];
    data[@"inboxStatus"] = [NSNumber numberWithInteger:[pushObject inboxStatus]];
    if(pushObject.sendDate){
        NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
        [dateFormatter setDateFormat:@"yyyy-MM-dd HH:mm:ss"];
        data[@"sendDate"] = [dateFormatter stringFromDate:pushObject.sendDate];
    }
    data[@"deeplinkUrl"] = [pushObject action].deeplinkURLString;

    
    return data;
}


// Push Delegate Methods

// Take push payload for Push clicked:
-(void)userNotificationCenter:(UNUserNotificationCenter *)center didReceiveNotificationResponse:(UNNotificationResponse *)response withCompletionHandler:(void (^)(void))completionHandler
{
    if ([response.actionIdentifier isEqual:UNNotificationDismissActionIdentifier]) {
        [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"onPushDismiss recent push: %@",response]];
        [FNetmeraPlugin onPushDismiss:response];
    } else if ([response.actionIdentifier isEqual:UNNotificationDefaultActionIdentifier]) {
        [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"onPushOpen recent push: %@",response]];
        [FNetmeraPlugin onPushOpen:response];
    }
    completionHandler();
}

// Take push payload for push Received on application foreground:
-(void)userNotificationCenter:(UNUserNotificationCenter *)center willPresentNotification:(UNNotification *)notification withCompletionHandler:(void (^)(UNNotificationPresentationOptions))completionHandler
{   
    [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"onPushReceive recent push: %@",notification]];
    [FNetmeraPlugin onPushReceive:notification];
    completionHandler(UNNotificationPresentationOptionAlert);   
}

- (void)application:(UIApplication *)application didRegisterForRemoteNotificationsWithDeviceToken:(NSData *)deviceToken
{
    if(deviceToken == nil) {
        return;
    }
    
    [FNetmeraPlugin onPushRegister:stringWithDeviceToken(deviceToken)];
}
@end
