using System.Collections.Generic;
using UnityEngine;

namespace Netmera
{
    /*
    sample app ids:
    <key>netmera_mobile_sdkkey</key>
	<string>FolJgBCBQJRAbEO42Dit77FBlTxmZ9mQIgcjbioswAiNnVj6dMMAeV3hacxNNpeL</string>
    <key>netmera_logging_disabled</key>
    <false/>
    */
    public class NetmeraIosCore : NetmeraCore
    {
        public readonly string CALLBACK_NAME = "netmeracallback";
        private Netmera.Callback _callback;

        public override void Init(bool loggingEnabled, Netmera.Callback callback = null)
        {
           if (Instance == null)
           {
               return;
           }
           Instance.LoggingEnabled = loggingEnabled;
            _callback = callback;
           
           Netmera.NetmeraIosCallback.NewInstance(CALLBACK_NAME);
           NetmeraPlugin.SetListener(CALLBACK_NAME);
           NetmeraPlugin.Log("Unity method call: Init");
        }

        public override void SendEvent(string eventCode, JSONNode eventJson)
        {
            string prms = null;
            if (eventJson != null)
            {
                prms = eventJson.ToString();
            }
            NetmeraPlugin.SendEvent(eventCode, prms);
        }

        public override void RequestNotificationPermissionForIOS() {
            NetmeraPlugin.RequestPushNotificationAuthorization();
        }

        public override void EnablePopupPresentation(bool isEnabled)
        {
            NetmeraPlugin.EnablePopupPresentation(isEnabled);
        }

        public override void RequestPermissionsForLocation()
        {
            NetmeraPlugin.RequestLocationAuthorization();
        }

        public override void UpdateUser(JSONNode jsonObject)
        {
            NetmeraPlugin.UpdateUser(jsonObject.ToString());
        }

        public override void Destroy()
        {
             
        }

        public override void FetchInbox(int pageSize, NetmeraEnum.PushStatus status, List<string> categories, bool includeExpiredObjects)
        {
            string categoriesCommaSeparated = "";
            if (categories.Count > 0)
            {
                categoriesCommaSeparated = categories[0];
                for (var i = 1; i < categories.Count; i++)
                {
                    categoriesCommaSeparated += "," + categories[i];
                }
            }
            NetmeraPlugin.FetchInbox(pageSize, (int) status, categoriesCommaSeparated, includeExpiredObjects);
        }

        public override void FetchNextPage()
        {
            NetmeraPlugin.FetchNextPage();
        }

        public override void ChangeInboxItemStatuses(int startIndex, int endIndex, NetmeraEnum.PushStatus status)
        {
            NetmeraPlugin.ChangeInboxItemStatuses(startIndex, endIndex, (int) status );
        }

        public override void GetStatusCount(NetmeraEnum.PushStatus status)
        {
            NetmeraPlugin.GetStatusCount((int)status);
        }

        public override void ChangeAllInboxItemStatuses(NetmeraEnum.PushStatus status)
        {
            NetmeraPlugin.ChangeAllInboxItemStatuses((int) status);
        }

        
        ///////////////// Push Callback Methods Start ///////////////////////////////////////
        public override void OnPushRegister(string gcmSenderId, string pushToken)
        {
            Instance.log($"OnPushRegister {gcmSenderId} {pushToken}");
            _callback.OnPushRegister(gcmSenderId, pushToken);
        }

        public override void OnPushReceive(JSONNode rawPush)
        {
            Instance.log($"OnPushReceive {rawPush}");
            _callback.OnPushReceive(rawPush, new NetmeraPushObject(rawPush));
        }

        public override void OnPushOpen(JSONNode rawPush)
        {
            Instance.log($"OnPushOpen {rawPush}");
            _callback.OnPushOpen(rawPush, new NetmeraPushObject(rawPush));
        }

        public override void OnPushDismiss(JSONNode rawPush)
        {
            Instance.log($"OnPushDismiss {rawPush}");
            _callback.OnPushDismiss(rawPush, new NetmeraPushObject(rawPush));
        }

        public override void OnPushButtonClicked(JSONNode rawPush)
        {
            Instance.log($"OnPushButtonClicked {rawPush}");
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
    }
}