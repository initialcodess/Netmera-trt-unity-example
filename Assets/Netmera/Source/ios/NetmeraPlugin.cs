

using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class NetmeraPlugin : MonoBehaviour
{

#if UNITY_IOS

    [DllImport("__Internal")]
    private static extern void _RequestPushNotificationAuthorization();

    [DllImport("__Internal")]
    private static extern void _SendEvent(string key, string parameters);

    [DllImport("__Internal")]
    private static extern void _UpdateUser(string parameters);

    [DllImport("__Internal")]
    private static extern void _EnablePopupPresentation(bool isEnabled);

    [DllImport("__Internal")]
    private static extern void _RequestLocationAuthorization();

    [DllImport("__Internal")]
    private static extern void _SetListener(string listenerName);

    [DllImport("__Internal")]
    private static extern void _Log(string message);

    [DllImport("__Internal")]
    public static extern void _FetchInbox(int pageSize, int status, string categories, bool includeExpiredObjects);

    [DllImport("__Internal")]
    public static extern void _FetchNextPage();

    [DllImport("__Internal")]
    public static extern void _ChangeInboxItemStatuses(int startIndex, int endIndex, int status);

    [DllImport("__Internal")]
    public static extern void _GetStatusCount(int status);

    [DllImport("__Internal")]
    public static extern void _ChangeAllInboxItemStatuses(int status);

    #endif

    public static void RequestPushNotificationAuthorization()
    {
        #if UNITY_IOS
            _RequestPushNotificationAuthorization();
        #endif
    }

    public static void SetListener(string listenerName)
    {
        #if UNITY_IOS
            _SetListener(listenerName);
        #endif
    }

    public static void SendEvent(string key, string parameters)
    {
        #if UNITY_IOS
            _SendEvent(key, parameters);
        #endif
    }

    public static void UpdateUser(string parameters)
    {
        #if UNITY_IOS
            _UpdateUser(parameters);
        #endif
    }

    public static void Log(string message)
    {
        #if UNITY_IOS
            _Log(message);
        #endif
    }

    public static void EnablePopupPresentation(bool isEnabled)
    {
        #if UNITY_IOS
            _EnablePopupPresentation(isEnabled);
        #endif
    }

    public static void RequestLocationAuthorization()
    {
        #if UNITY_IOS
            _RequestLocationAuthorization();
        #endif
    }

    public static void FetchInbox(int pageSize, int status, string categories, bool includeExpiredObjects)
    {
        #if UNITY_IOS
            _FetchInbox(pageSize, status, categories, includeExpiredObjects);
        #endif
    }

    public static void FetchNextPage()
    {
        #if UNITY_IOS
            _FetchNextPage();
        #endif
    }

    public static void ChangeInboxItemStatuses(int startIndex, int endIndex, int status)
    {
        #if UNITY_IOS
            _ChangeInboxItemStatuses(startIndex, endIndex, status);
        #endif
    }

    public static void GetStatusCount(int status)
    {
        #if UNITY_IOS
            _GetStatusCount(status);
        #endif
    }

    public static void ChangeAllInboxItemStatuses(int status)
    {
        #if UNITY_IOS
            _ChangeAllInboxItemStatuses(status);
        #endif
    }

}