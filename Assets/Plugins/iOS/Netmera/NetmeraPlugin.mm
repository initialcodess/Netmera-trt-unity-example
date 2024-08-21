
#import "NetmeraPlugin.h"
#import <Netmera/Netmera.h>
#import <objc/runtime.h>
#import "FNetmeraUser.h"
#import "FNetmeraEvent.h"
#include <sys/types.h>
#include <sys/sysctl.h>
#import <UserNotifications/UserNotifications.h>


@implementation FNetmeraPlugin

BOOL logsEnabled = NO;
NSString* unityListener;

NSString* waitingOnPushRegister;
NSString* waitingOnPushReceive;
NSString* waitingOnPushOpen;
NSString* waitingOnPushDismiss;
NetmeraInbox* netmeraInbox;
BOOL hasNoMoreNextPage;
// receiver methods

NSString* generatePushObject(NetmeraPushObject* push) {
    NSMutableDictionary* dictionary = generatePushDictionary(push);
    if(dictionary == nil) {
        return nil;
    }
    NSError * err;
    NSData * jsonData = [NSJSONSerialization dataWithJSONObject:dictionary options:0 error:&err];
    if(err != nil) {
        [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"generatePushObject error: %@", err]];
        return nil;
    }
    NSString * pushString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    return pushString;
}

NSString* convertDictionaryToJSON(NSDictionary* dictionary) {
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dictionary
                                                       options:NSJSONWritingPrettyPrinted // Pass 0 if you don't care about the readability of the generated string
                                                         error:&error];
    if (! jsonData) {
        return nil;
    } else {
        return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
    }
}


NSMutableDictionary* generatePushDictionary(NetmeraPushObject* push) {
    if(push == nil) {
        [FNetmeraPlugin NMLog:@"generatePushObject recent push object nil"];
        return nil;
    }
    [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"recentpush: %@", push]];
    NSString* sendDate;
    NSMutableDictionary* dictionary = [NSMutableDictionary dictionary];
    if(push.sendDate != nil){
            NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
            [dateFormatter setDateFormat:@"yyyy-MM-dd HH:mm:ss"];
            dictionary[@"sendDate"] = [dateFormatter stringFromDate:push.sendDate];
    }
    
    if(push.action !=nil) {
        dictionary[@"deeplinkUrl"] = push.action.deeplinkURLString;

    }
    if(push.alert != nil) {
        dictionary[@"body"] = push.alert.body;
        dictionary[@"title"] = push.alert.title;
        dictionary[@"subtitle"] = push.alert.subtitle;
    }
    dictionary[@"pushType"] = [NSNumber numberWithInteger:[push pushType]];
    dictionary[@"inboxStatus"] = [NSNumber numberWithInteger:[push inboxStatus]];
    dictionary[@"customJson"] = push.customDictionary;
    dictionary[@"pushId"] = push.pushId;
    dictionary[@"pushInstanceId"] = push.pushInstanceId;
    return dictionary;
}

+(void) onPushReceive:(UNNotification *)notification {
    NSDictionary *userInfo = notification.request.content.userInfo;
    NetmeraPushObject *pushObj = [[NetmeraPushObject alloc] initWithDictionary:userInfo];
    [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"onPushReceive pushObj: %@", pushObj]];
    
    NSString* push = generatePushObject(pushObj);
    
    [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"onPushReceive: %@", push]];
    if (unityListener == nil) {
        waitingOnPushReceive = [NSString stringWithFormat:@"%@", push];
    } else {
        SendMessageToUnity(unityListener, @"onPushReceive", [NSString stringWithFormat:@"%@", push]);
    }
}

+(void) onPushOpen:(UNNotificationResponse *)response{
    NSDictionary *userInfo = response.notification.request.content.userInfo;
    NetmeraPushObject *pushObj = [[NetmeraPushObject alloc] initWithDictionary:userInfo];
    [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"onPushOpen pushObj: %@", pushObj]];

    NSString* push = generatePushObject(pushObj);
    
    [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"onPushOpen: %@", push]];
    if (unityListener == nil) {
        waitingOnPushOpen = [NSString stringWithFormat:@"%@", push];
    } else {
        SendMessageToUnity(unityListener, @"onPushOpen", [NSString stringWithFormat:@"%@", push]);
    }
}

+(void) onPushDismiss:(UNNotificationResponse *)response{
     NSDictionary *userInfo = response.notification.request.content.userInfo;
     NetmeraPushObject *pushObj = [[NetmeraPushObject alloc] initWithDictionary:userInfo];
     [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"onPushDismiss pushObj: %@", pushObj]];
 
     NSString* push = generatePushObject(pushObj);
     
     [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"onPushDismiss: %@", push]];
     if (unityListener == nil) {
         waitingOnPushDismiss = [NSString stringWithFormat:@"%@", push];
     } else {
         SendMessageToUnity(unityListener, @"onPushDismiss", [NSString stringWithFormat:@"%@", push]);
     }
 }

+(void) onPushRegister:(NSString *)deviceToken {
    [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"onPushRegister: %@", deviceToken]];
    if (unityListener == nil) {
        waitingOnPushRegister = [NSString stringWithFormat:@"%@", deviceToken];
    } else {
        SendMessageToUnity(unityListener, @"onPushRegister", [NSString stringWithFormat:@"%@", deviceToken]);
    }
}

// caller methods

+(void)requestPushNotificationAuthorization
{
    [Netmera requestPushNotificationAuthorizationForTypes:UIUserNotificationTypeAlert | UIUserNotificationTypeBadge | UIUserNotificationTypeSound];
}

 +(void)init:(BOOL)printLogs {
    logsEnabled = printLogs;
 }

+(void)setListener:(NSString*)unityListenerName {
    unityListener = unityListenerName;
    [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"setListener: %@", unityListener]];

    if(waitingOnPushRegister !=nil) {
        SendMessageToUnity(unityListener, @"onPushRegister", [NSString stringWithFormat:@"%@", waitingOnPushRegister]);
        waitingOnPushRegister = nil;
    }
    if(waitingOnPushReceive !=nil) {
        SendMessageToUnity(unityListener, @"onPushReceive", [NSString stringWithFormat:@"%@", waitingOnPushReceive]);
        waitingOnPushReceive = nil;
    }
    if(waitingOnPushOpen != nil) {
        SendMessageToUnity(unityListener, @"onPushOpen", [NSString stringWithFormat:@"%@", waitingOnPushOpen]);
        waitingOnPushOpen = nil;
    }
    if(waitingOnPushDismiss != nil) {
        SendMessageToUnity(unityListener, @"onPushDismiss", [NSString stringWithFormat:@"%@", waitingOnPushDismiss]);
        waitingOnPushDismiss = nil;
    }
}

+(void)sendEvent:(NSString*)eventKey withParams:(NSDictionary*)params
{
    FNetmeraEvent *event = [FNetmeraEvent event];
    event.netmeraEventKey = eventKey;
    event.eventParameters = params;
    [Netmera sendEvent:event];
}

+(void)enablePopupPresentation:(BOOL)isEnabled {
    [Netmera setEnabledPopupPresentation: isEnabled];
}

+(void) log:(NSString*)message {
    [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"C# log: %@", message]];
}


+(void)requestLocationAuthorization {
    [Netmera requestLocationAuthorization];
}

+(void)updateUser:(NSDictionary*)params {
    NetmeraUser *user = [[NetmeraUser alloc] init];

    try {
        
        for(id key in params) {
            NSObject* value = [params objectForKey:key];
            [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"key %@ value %@", key,value]];
            if([key isEqualToString:@"user_id"] && value != nil) {
                user.userId = [NSString stringWithFormat:@"%@", value];
            }else if([key isEqualToString:@"email"]) {
                user.email = [NSString stringWithFormat:@"%@", value];
            }else if([key isEqualToString:@"msisdn"]) {
                user.MSISDN = [NSString stringWithFormat:@"%@", value];
            }else if([key isEqualToString:@"name"]) {
                user.name = [NSString stringWithFormat:@"%@", value];
            }else if([key isEqualToString:@"surname"]) {
                user.surname = [NSString stringWithFormat:@"%@", value];
            }else if([key isEqualToString:@"language"]) {
                user.language = [NSString stringWithFormat:@"%@", value];
            }else if([key isEqualToString:@"date_of_birth"] ) {
                if(value != nil) {
                    NSArray *commasArr = [[NSString stringWithFormat:@"%@", value] componentsSeparatedByString:@"-"];
                    NSMutableArray* arr = [commasArr mutableCopy];
                    for(int i=0;i<[arr count];i++){
                        NSString *str= [arr objectAtIndex:i];
                        if([str hasPrefix:@"0"]) {
                            arr[i] = [str substringFromIndex:1];
                        }
                    }
                    NSDateComponents *birthComponents = [[NSDateComponents alloc] init];
                    birthComponents.year = [arr[0] intValue];
                    birthComponents.month = [arr[1] intValue];
                    birthComponents.day = [arr[2] intValue];
                    NSCalendar *calendar = [[NSCalendar alloc] initWithCalendarIdentifier:NSCalendarIdentifierGregorian];
                    user.dateOfBirth = [calendar dateFromComponents:birthComponents];
                }
            }else if([key isEqualToString:@"gender"]) {
                if(value == nil) {
                    user.gender = NetmeraProfileAttributeGenderNotSpecified;
                }else if([[NSString stringWithFormat:@"%@", value] isEqualToString:@"male"]) {
                    user.gender = NetmeraProfileAttributeGenderMale;
                }else if([[NSString stringWithFormat:@"%@", value] isEqualToString:@"female"]) {
                    user.gender = NetmeraProfileAttributeGenderFemale;
                }

            }else if([key isEqualToString:@"marital_status"]) {
                if(value == nil) {
                    user.maritalStatus = NetmeraProfileAttributeMaritalStatusNotSpecified;
                }else if([[NSString stringWithFormat:@"%@", value] isEqualToString:@"single"]) {
                    user.maritalStatus = NetmeraProfileAttributeMaritalStatusSingle;
                }else if([[NSString stringWithFormat:@"%@", value] isEqualToString:@"married"]) {
                    user.maritalStatus = NetmeraProfileAttributeMaritalStatusMarried;
                }
            }else if([key isEqualToString:@"child_count"] && value != nil) {
                NSString* numOfChildren = [NSString stringWithFormat:@"%@", value];
                user.numberOfChildren = [numOfChildren intValue];
            }else if([key isEqualToString:@"country"]) {
                user.country = [NSString stringWithFormat:@"%@", value];
            }else if([key isEqualToString:@"state"]) {
                user.state = [NSString stringWithFormat:@"%@", value] ;
            }else if([key isEqualToString:@"city"]) {
                user.city = [NSString stringWithFormat:@"%@", value];
            }else if([key isEqualToString:@"district"]) {
                user.district = [NSString stringWithFormat:@"%@", value];
            }else if([key isEqualToString:@"occupation"]) {
                user.occupation =[NSString stringWithFormat:@"%@", value];
            }else if([key isEqualToString:@"industry"]) {
                user.industry = [NSString stringWithFormat:@"%@", value];
            }else if([key isEqualToString:@"favorite_team"]) {
                user.favouriteTeam =[NSString stringWithFormat:@"%@", value];
            }else if([key isEqualToString:@"external_segments"] ) {
                if(value != nil) {
                    NSArray *commasArr = [[NSString stringWithFormat:@"%@", value] componentsSeparatedByString:@","];
                    user.externalSegments = commasArr;
                }else {
                    user.externalSegments = [NSMutableArray new];
                }
            }
        }
        // Send data to Netmera
        [Netmera updateUser:user];
    } catch (NSException *exception) {
        [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"exception for user update: %@", exception]];
    }
}

+(void) fetchInbox:(int) pageSize status:(int) pStatus categories:(NSString*) pCategories expired:(BOOL) includeExpired
{
    NetmeraInboxFilter *filter = [[NetmeraInboxFilter alloc] init];
    NSArray *categories = [[NSString stringWithFormat:@"%@", pCategories] componentsSeparatedByString:@","];
    if(categories != nil && categories.count > 0) {
        filter.categories = categories;

    }
    filter.status = pStatus;
    filter.pageSize = pageSize;
    filter.shouldIncludeExpiredObjects = includeExpired;
    
    [Netmera fetchInboxUsingFilter:filter
                        completion:^(NetmeraInbox *inbox, NSError *error) {
        if(error) {
            [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"Fetch Inbox Error : %@", [error debugDescription]]];
            SendMessageToUnity(unityListener, @"fetchInboxFail", @"");
        }
        else {
            netmeraInbox = inbox;
            NSDictionary *inboxDictionary = [self getInboxList:inbox];
            if(inboxDictionary == nil || inboxDictionary.count == 0) {
                SendMessageToUnity(unityListener, @"fetchInboxFail", @"");
                return;
            }
            SendMessageToUnity(unityListener, @"fetchInboxSuccess", convertDictionaryToJSON(inboxDictionary));
        }
    }];
}

+(void) fetchNextPage {
    if(netmeraInbox == nil) {
        [FNetmeraPlugin NMLog:@"Fetch Next Page Error -> Fetch Netmera Inbox first, Netmera Inbox object is nil!"];
        SendMessageToUnity(unityListener, @"fetchNextPageFail", @"");
        return;
    }
    [netmeraInbox fetchNextPageWithCompletionBlock:^(NSError *error) {
            if(error) {
                [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"fetchNextPageFail Error : %@", [error debugDescription]]];
                SendMessageToUnity(unityListener, @"fetchNextPageFail", @"");
                return;
            }else {
                NSDictionary *inboxDictionary = [self getInboxList:netmeraInbox];
                if(inboxDictionary == nil || inboxDictionary.count == 0) {
                    [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"Error No more messages : %@", [error debugDescription]]];
                    SendMessageToUnity(unityListener, @"fetchNextPageFail", @"");
                    return;
                }
                SendMessageToUnity(unityListener, @"fetchNextPageSuccess", convertDictionaryToJSON(inboxDictionary));
            }
        }];
}

+ (NSDictionary*)getInboxList:(NetmeraInbox*)inbox
{
    NSMutableArray* inboxList = [self getInboxListArray:inbox];
    NSDictionary *dictionary = @{
        @"hasNextPage": @(inbox.hasNextPage),
        @"inbox": inboxList
    };
    hasNoMoreNextPage= inbox.hasNextPage;
    return dictionary;
}

+ (NSMutableArray*)getInboxListArray:(NetmeraInbox*)inbox
{
    NSMutableArray *inboxList = [NSMutableArray array];
    for(NetmeraPushObject *pushObject in inbox.objects)
    {
        NSDictionary* dict = generatePushDictionary(pushObject);
        [inboxList addObject:dict];
    }
    return inboxList;

}


+(void) changeInboxItemStatuses:(int)pstartIndex endIndex:(int)pendIndex status:(int)pstatus {
    if (netmeraInbox == nil) {
        [FNetmeraPlugin NMLog:@"Change Inbox Item Statuses Error -> Fetch Netmera Inbox first, Netmera Inbox object is nil!"];
        SendMessageToUnity(unityListener, @"changeInboxItemStatusesFail", @"");
        return;
    }
    NSMutableArray* inboxList = [self getInboxListArray:netmeraInbox];
    if(inboxList == nil || inboxList.count == 0) {
        [FNetmeraPlugin NMLog:@"Change Inbox Item Statuses Error -> inbox is empty"];
        SendMessageToUnity(unityListener, @"changeInboxItemStatusesFail", @"");
        return;
    }else if (pstartIndex < 0 || pstartIndex > pendIndex || pendIndex > inboxList.count) {
        [FNetmeraPlugin NMLog:@"Change Inbox Item Statuses Error -> Indexes are invalid!"];
        SendMessageToUnity(unityListener, @"changeInboxItemStatusesFail", @"");
        return;
    }
    NSLog(@"changeInboxItemStatuses %d %d %d %lu", pstartIndex, pendIndex, pstatus, (unsigned long)inboxList.count);
    NSIndexSet *set = [NSIndexSet indexSetWithIndexesInRange:NSMakeRange(pstartIndex, pendIndex - pstartIndex)];
    NSArray *objectList = [netmeraInbox.objects objectsAtIndexes:set];
    [netmeraInbox updateStatus:pstatus
                forPushObjects:objectList
                    completion:^(NSError *error) {
        if(error) {
            [FNetmeraPlugin NMLog:@"Change Inbox Item Statuses Error -> Indexes are invalid!"];
            SendMessageToUnity(unityListener, @"changeInboxItemStatusesFail", @"");
        } else {
            SendMessageToUnity(unityListener, @"changeInboxItemStatusesSuccess", @"OK");
        }
    }];
    
}

+(void) getStatusCount:(int)status {
    NSUInteger result = [netmeraInbox countForStatus:status];
    SendMessageToUnity(unityListener, @"getStatusCount", [NSString stringWithFormat:@"%d",(int)result]);
}

+(void) changeAllInboxItemStatuses:(int)status {
    [Netmera updateStatus:status forAllWithCompletion:^(NSError * _Nonnull error) {
        if(error) {
            [FNetmeraPlugin NMLog:@"Change All Inbox Item Statuses Error -> Indexes are invalid!"];
            SendMessageToUnity(unityListener, @"changeAllInboxItemStatusesFail", @"");
        } else {
            SendMessageToUnity(unityListener, @"changeAllInboxItemStatusesSuccess", @"OK");
        }
     }];
}

+(void) NMLog:(NSString*) message {
    NSBundle* mainBundle = [NSBundle mainBundle];
    NSNumber* loggingDisabled = [mainBundle objectForInfoDictionaryKey:@"netmera_logging_disabled"];
    if([loggingDisabled intValue] != 1) {
        NSLog(@"NetmeraUnity: %@", message);
    }

}

void SendMessageToUnity(NSString* unityListener,NSString* methodVal, NSString* messageVal) {
    [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"sender sends message to callback: %@ %@ %@", unityListener, methodVal, messageVal]];
    const char* methodValC =[methodVal UTF8String];
    const char* messageValC =[messageVal UTF8String];
    const char* unityListenerC =[unityListener UTF8String];
    UnitySendMessage(unityListenerC, methodValC, messageValC);

}

NSString* CreateNSString(const char* string) {
    return [NSString stringWithUTF8String: string ? string : ""];
}

@end

extern "C"
{
    void _SetListener(const char *listener) {
        [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"_SetListener called %@", [NSString stringWithUTF8String:listener]]];
        [FNetmeraPlugin setListener:[NSString stringWithFormat:@"%s", listener]];
    }

    void _RequestPushNotificationAuthorization() {
        [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"_RequestPushNotificationAuthorization called"]];
        [FNetmeraPlugin requestPushNotificationAuthorization];
    }

    void _SendEvent(const char *key,const char *params)
    {
        NSString* eventKey = [NSString stringWithUTF8String:key];
        NSString* paramsValue = [NSString stringWithUTF8String:params];
        NSData *jsonData = [paramsValue dataUsingEncoding:NSUTF8StringEncoding];
        NSError *error;
        NSDictionary *jsonDic = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingAllowFragments error:&error];
        if(error == nil) {
            [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"_SendEvent called key: %@ params: %@", eventKey, paramsValue]];
            [FNetmeraPlugin sendEvent:eventKey withParams:jsonDic];
        }else {
            [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"error while creating event JSON!! %@", error]];
        }
        
    }

    void _UpdateUser(const char *params) {
        NSString* paramsValue = [NSString stringWithUTF8String:params];
        NSData *jsonData = [paramsValue dataUsingEncoding:NSUTF8StringEncoding];
        NSError *error;
        NSDictionary *jsonDic = [NSJSONSerialization JSONObjectWithData:jsonData options:NSJSONReadingAllowFragments error:&error];
        if(error == nil) {
            [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"_UpdateUser called json: %@", jsonDic]];
            [FNetmeraPlugin updateUser:jsonDic];
        }else {
            [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"error while creating event JSON!! %@", error]];
        }
    }

    void _Log(const char *message) {
        NSString* logMessage = [NSString stringWithUTF8String:message];
        [FNetmeraPlugin log:logMessage];
    }
    
    void _EnablePopupPresentation(const BOOL isPopupEnabled)
    {
        [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"_EnablePopupPresentation called params: %d", isPopupEnabled]];
        [FNetmeraPlugin enablePopupPresentation:isPopupEnabled];
    }

    void _RequestLocationAuthorization() {
        [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"_RequestLocationAuthorization called"]];
        [FNetmeraPlugin requestLocationAuthorization];
    }

    void _FetchInbox(const int pageSize, const int pstatus, const char *pcategories, const BOOL pincludeExpiredObjects){
        [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"_FetchInbox called"]];
        [FNetmeraPlugin fetchInbox:pageSize status:pstatus categories:[NSString stringWithUTF8String:pcategories] expired:pincludeExpiredObjects];
    }

    void _FetchNextPage(){
        [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"_FetchNextPage called"]];
        [FNetmeraPlugin fetchNextPage];
    }

    void _ChangeInboxItemStatuses(int pstartIndex, int pendIndex, int pstatus){
        [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"_ChangeInboxItemStatuses called"]];
        [FNetmeraPlugin changeInboxItemStatuses:pstartIndex endIndex:pendIndex status:pstatus];
    }

    void _GetStatusCount(int status){
        [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"_GetStatusCount called"]];
        [FNetmeraPlugin getStatusCount:status];
    }

    void _ChangeAllInboxItemStatuses(int status){
        [FNetmeraPlugin NMLog:[NSString stringWithFormat:@"_ChangeAllInboxItemStatuses called"]];
        [FNetmeraPlugin changeAllInboxItemStatuses:status];
    }
}
