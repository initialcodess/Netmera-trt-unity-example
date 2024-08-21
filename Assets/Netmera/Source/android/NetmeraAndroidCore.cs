using System.Collections.Generic;
using UnityEngine;

namespace Netmera
{
    public class NetmeraAndroidCore : NetmeraCore
    {
        private AndroidJavaClass _unityPlayer;
        private AndroidJavaObject _pluginJava, _activity;
        private NetmeraPluginUnityBridge _javaProxy;
        private Netmera.Callback _callback;

        ///////////////// Caller Methods Start ///////////////////////////////////////
        public override void Init(bool loggingEnabled, Netmera.Callback callback = null)
        {
            
            if (Instance == null)
            {
                return;
            }
            Instance.LoggingEnabled = loggingEnabled;
            Instance.log("Unity method call: Init");
            if (callback == null)
            {
                Instance.log("Callback parameter can not be null");
                return;
            }
        
            _callback = callback;
            initAndroidClasses();
        }

        public override void RequestNotificationPermissionForIOS() {
        }


        public override void SendEvent(string eventCode, JSONNode eventJson)
        {
            Instance.log("Unity method call: SendEvent");
            string prms = null;
            if (eventJson != null)
            {
                prms = eventJson.ToString();
            }

            _pluginJava.Call("sendEvent", eventCode, prms);
        }

        public override void EnablePopupPresentation(bool isEnabled)
        {
            Instance.log("Unity method call: EnablePopupPresentation");
            _pluginJava.Call("enablePopupPresentation", isEnabled);
        }

        public override void RequestPermissionsForLocation()
        {
            Instance.log("Unity method call: RequestPermissionsForLocation");
            _pluginJava.Call("requestPermissionsForLocation", true);
        }

        public override void UpdateUser(JSONNode jsonObject)
        {
            Instance.log("Unity method call: UpdateUser");
            _pluginJava.Call("updateUser", jsonObject.ToString());
        }

        public override void Destroy()
        {
            Instance.log("Unity method call: Destroy");
            _pluginJava.Call("destroy", true);
        }

        public override void FetchInbox(int pageSize, NetmeraEnum.PushStatus status, List<string> categories, bool includeExpiredObjects)
        {
            Instance.log("Unity method call: FetchInbox");
            string categoriesCommaSeparated = "";
            if (categories.Count > 0)
            {
                categoriesCommaSeparated = categories[0];
                for (var i = 1; i < categories.Count; i++)
                {
                    categoriesCommaSeparated += "," + categories[i];
                }
            }

            _pluginJava.Call("fetchInbox", pageSize, (int) status, categoriesCommaSeparated, includeExpiredObjects);
        }

        public override void FetchNextPage()
        {
            Instance.log("Unity method call: FetchNextPage");
            _pluginJava.Call("fetchNextPage", true);
        }

        public override void ChangeInboxItemStatuses(int startIndex, int endIndex, NetmeraEnum.PushStatus status)
        {
            Instance.log("Unity method call: ChangeInboxItemStatuses");
            _pluginJava.Call("changeInboxItemStatuses", startIndex, endIndex, (int) status);
        }

        public override void GetStatusCount(NetmeraEnum.PushStatus status)
        {
            Instance.log("Unity method call: GetStatusCount");
            _pluginJava.Call("getStatusCount", (int) status);
        }

        public override void ChangeAllInboxItemStatuses(NetmeraEnum.PushStatus status)
        {
            Instance.log("Unity method call: ChangeAllInboxItemStatuses");
            _pluginJava.Call("changeAllInboxItemStatuses", (int) status);
        }
        ///////////////// Caller Methods End ///////////////////////////////////////

        ///////////////// Push Callback Methods Start ///////////////////////////////////////
        public override void OnPushRegister(string gcmSenderId, string pushToken)
        {
            //Instance.log($"OnPushRegister {gcmSenderId} {pushToken}");
            _callback.OnPushRegister(gcmSenderId, pushToken);
        }

        public override void OnPushReceive(JSONNode rawPush)
        {
            //Instance.log($"OnPushReceive {rawPush}");
            _callback.OnPushReceive(rawPush, new NetmeraPushObject(rawPush));
        }

        public override void OnPushOpen(JSONNode rawPush)
        {
            //Instance.log($"OnPushOpen {rawPush}");
            _callback.OnPushOpen(rawPush, new NetmeraPushObject(rawPush));
        }

        public override void OnPushDismiss(JSONNode rawPush)
        {
            //Instance.log($"OnPushDismiss {rawPush}");
            _callback.OnPushDismiss(rawPush, new NetmeraPushObject(rawPush));
        }

        public override void OnPushButtonClicked(JSONNode rawPush)
        {
            //Instance.log($"OnPushButtonClicked {rawPush}");
            _callback.OnPushButtonClicked(rawPush, new NetmeraPushObject(rawPush));
        }
        ///////////////// Push Callback Methods End ///////////////////////////////////////


        ///////////////// Inbox Callback Methods Start ///////////////////////////////////////
        public override void OnInboxFetchSuccess(JSONNode netmeraPushInboxJSON)
        {
            Instance.log($"OnInboxFetchSuccess {netmeraPushInboxJSON}");
            _callback.OnInboxFetchSuccess(netmeraPushInboxJSON);
        }

        public override void OnInboxFetchFail(int errorCode, string errorMessage)
        {
            Instance.log($"OnInboxFetchFail code: {errorCode} message: {errorMessage}");
            _callback.OnInboxFetchFail(errorCode, errorMessage);
        }

        public override void OnInboxNextPageFetchSuccess(JSONNode netmeraPushInboxJSON)
        {
            Instance.log($"OnInboxNextPageFetchSuccess {netmeraPushInboxJSON}");
            _callback.OnInboxNextPageFetchSuccess(netmeraPushInboxJSON);
        }

        public override void OnInboxNextPageFetchFail(int errorCode, string errorMessage)
        {
            Instance.log($"OnInboxNextPageFetchFail code: {errorCode} message: {errorMessage}");
            _callback.OnInboxNextPageFetchFail(errorCode, errorMessage);
        }

        public override void OnInboxStatusChangeSuccess()
        {
            Instance.log("OnInboxStatusChangeSuccess");
            _callback.OnInboxStatusChangeSuccess();
        }

        public override void OnInboxStatusChangeFail(int errorCode, string errorMessage)
        {
            Instance.log($"OnInboxStatusChangeFail code: {errorCode} message: {errorMessage}");
            _callback.OnInboxStatusChangeFail(errorCode, errorMessage);
        }

        public override void OnInboxStatusCount(int countWithThatStatus)
        {
            Instance.log($"OnInboxStatusCount {countWithThatStatus}");
            _callback.OnInboxStatusCount(countWithThatStatus);
        }
        ///////////////// Inbox Callback Methods End ///////////////////////////////////////


        public void initAndroidClasses()
        {
            _javaProxy = new NetmeraPluginUnityBridge();
            AndroidJavaClass player = getUnityPlayerClass();
            AndroidJavaObject activity = getCurrentActivity();
            getPluginJavaClass(_javaProxy);
            player.Dispose();
            activity.Dispose();
        }

        private AndroidJavaObject getCurrentActivity()
        {
            if (_unityPlayer == null)
            {
                Debug.Log("Unity Player is null");
                return null;
            }

            if (_activity == null)
            {
                lock (classLock)
                {
                    if (_activity == null)
                    {
                        _activity = _unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                    }
                }
            }

            return _activity;
        }

        private AndroidJavaObject getPluginJavaClass(NetmeraPluginUnityBridge pluginImpl)
        {
            if (_activity == null)
            {
                Debug.Log("Activity class is null");
                return null;
            }

            if (_pluginJava == null)
            {
                lock (classLock)
                {
                    if (_pluginJava == null)
                    {
                        _pluginJava = new AndroidJavaObject("com.netmera.unity.sdk.core.NetmeraPluginImpl", _activity, pluginImpl);
                    }
                }
            }

            return _pluginJava;
        }

        private AndroidJavaClass getUnityPlayerClass()
        {
            if (_unityPlayer == null)
            {
                lock (classLock)
                {
                    if (_unityPlayer == null)
                    {
                        _unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    }
                }
            }

            return _unityPlayer;
        }
    }
}