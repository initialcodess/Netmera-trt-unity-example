using UnityEngine;

namespace Netmera
{
    public class NetmeraPluginUnityBridge : AndroidJavaProxy
    {
        public NetmeraPluginUnityBridge() : base("com.netmera.unity.sdk.core.NetmeraPluginUnityBridge")
        {
        }

        public void OnPushRegister(string gcmSenderId, string pushToken)
        {
            NetmeraCore.Instance.OnPushRegister(gcmSenderId, pushToken);
        }

        public void OnPushReceive(string netmeraPushObjectJSON)
        {
            NetmeraCore.Instance.OnPushReceive(JSON.Parse(netmeraPushObjectJSON));
        }

        public void OnPushOpen(string netmeraPushObjectJSON)
        {
            NetmeraCore.Instance.OnPushOpen(JSON.Parse(netmeraPushObjectJSON));
        }

        public void OnPushDismiss(string netmeraPushObjectJSON)
        {
            NetmeraCore.Instance.OnPushDismiss(JSON.Parse(netmeraPushObjectJSON));
        }

        public void OnPushButtonClicked(string netmeraPushObjectJSON)
        {
            NetmeraCore.Instance.OnPushButtonClicked(JSON.Parse(netmeraPushObjectJSON));
        }

        public void OnInboxFetchSuccess(string netmeraPushInboxJSON)
        {
            NetmeraCore.Instance.OnInboxFetchSuccess(JSON.Parse(netmeraPushInboxJSON));
        }

        public void OnInboxFetchFail(int errorCode, string errorMessage)
        {
            NetmeraCore.Instance.OnInboxFetchFail(errorCode, errorMessage);
        }

        public void OnInboxNextPageFetchSuccess(string netmeraPushInboxJSON)
        {
            NetmeraCore.Instance.OnInboxNextPageFetchSuccess(JSON.Parse(netmeraPushInboxJSON));
        }

        public void OnInboxNextPageFetchFail(int errorCode, string errorMessage)
        {
            NetmeraCore.Instance.OnInboxNextPageFetchFail(errorCode, errorMessage);
        }

        public void OnInboxStatusChangeSuccess()
        {
            NetmeraCore.Instance.OnInboxStatusChangeSuccess();
        }

        public void OnInboxStatusChangeFail(int errorCode, string errorMessage)
        {
            NetmeraCore.Instance.OnInboxStatusChangeFail(errorCode, errorMessage);
        }

        public void OnInboxStatusCount(int countWithThatStatus)
        {
            NetmeraCore.Instance.OnInboxStatusCount(countWithThatStatus);
        }
    }
}