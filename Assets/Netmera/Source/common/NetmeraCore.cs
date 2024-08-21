using System.Collections.Generic;
using UnityEngine;

namespace Netmera
{
    public abstract class NetmeraCore
    {
        public static readonly string SdkVersion = "1.0.0";
        protected static readonly object classLock = new object();
        private static volatile NetmeraCore _instance;
        public bool LoggingEnabled;

        public static NetmeraCore Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (classLock)
                    {
                        if (_instance == null)
                        {
                            if (Application.platform == RuntimePlatform.Android)
                            {
                                _instance = new NetmeraAndroidCore();
                            }
                            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                            {
                                _instance = new NetmeraIosCore();
                            }
                            else
                            {
                                Debug.LogError("Netmera Unity SDK only supports Android and Ios");
                            }
                        }
                    }
                }

                return _instance;
            }
        }

        public void log(string message)
        {
            if(Application.platform == RuntimePlatform.IPhonePlayer) {
                    NetmeraPlugin.Log("NetmeraUnity: " + message);
            }else if (LoggingEnabled){
                Debug.Log("NetmeraUnity: " + message); 
            }
        }

        public void logError(string message)
        {
            if (LoggingEnabled)
            {
                Debug.LogError(message);
            }
        }


        public abstract void Init(bool loggingEnabled, Netmera.Callback callback = null);

        public abstract void SendEvent(string eventCode, JSONNode eventJson);
        public abstract void EnablePopupPresentation(bool isEnabled);

        public abstract void RequestNotificationPermissionForIOS();

        public abstract void RequestPermissionsForLocation();
        public abstract void UpdateUser(JSONNode jsonObject);

        public abstract void Destroy();

        public abstract void FetchInbox(int pageSize, NetmeraEnum.PushStatus status, List<string> categories, bool includeExpiredObjects);

        public abstract void FetchNextPage();

        public abstract void ChangeInboxItemStatuses(int startIndex, int endIndex, NetmeraEnum.PushStatus status);

        public abstract void GetStatusCount(NetmeraEnum.PushStatus status);
        public abstract void ChangeAllInboxItemStatuses(NetmeraEnum.PushStatus status);

        public abstract void OnPushRegister(string gcmSenderId, string pushToken);
        public abstract void OnPushReceive(JSONNode netmeraPushObjectJSON);
        public abstract void OnPushOpen(JSONNode netmeraPushObjectJSON);
        public abstract void OnPushDismiss(JSONNode netmeraPushObjectJSON);
        public abstract void OnPushButtonClicked(JSONNode netmeraPushObjectJSON);

        public abstract void OnInboxFetchSuccess(JSONNode netmeraPushInboxJSON);
        public abstract void OnInboxFetchFail(int errorCode, string errorMessage);
        public abstract void OnInboxNextPageFetchSuccess(JSONNode netmeraPushInboxJSON);
        public abstract void OnInboxNextPageFetchFail(int errorCode, string errorMessage);
        public abstract void OnInboxStatusChangeSuccess();
        public abstract void OnInboxStatusChangeFail(int errorCode, string errorMessage);
        public abstract void OnInboxStatusCount(int countWithThatStatus);
    }
}