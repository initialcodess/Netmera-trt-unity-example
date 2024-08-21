using UnityEngine;
using System;

namespace Netmera
{
    public class NetmeraIosCallback : MonoBehaviour
    {
        public static void NewInstance(string callbackName)
        {
            GameObject nmCallback = new GameObject();
            nmCallback.name = callbackName;
            nmCallback.AddComponent<NetmeraIosCallback>();
        }
        
        public void onPushRegister(string token)
        {
            NetmeraCore.Instance.OnPushRegister("", token);
        }

        public void onPushReceive(string netmeraPushObjectJSON)
        {
            NetmeraCore.Instance.OnPushReceive(JSON.Parse(netmeraPushObjectJSON));
        }
        
        public void onPushDismiss(string netmeraPushObjectJSON)
        {
            NetmeraCore.Instance.OnPushDismiss(JSON.Parse(netmeraPushObjectJSON));
        }
        public void onPushOpen(string netmeraPushObjectJSON)
        {
            NetmeraCore.Instance.OnPushOpen(JSON.Parse(netmeraPushObjectJSON));
        }
        
        public void fetchInboxFail(string empty)
        {
            NetmeraCore.Instance.OnInboxFetchFail(1, "fetchInboxFailed");
        }

        public void fetchInboxSuccess(string inboxJSON)
        {
            NetmeraCore.Instance.OnInboxFetchSuccess(JSON.Parse(inboxJSON));
        }

        public void fetchNextPageFail(string empty)
        {
            NetmeraCore.Instance.OnInboxNextPageFetchFail(1, "fetchNextPageFailed");
        }

        public void fetchNextPageSuccess(string inboxJSON)
        {
            NetmeraCore.Instance.OnInboxNextPageFetchSuccess(JSON.Parse(inboxJSON));
        }

        public void changeInboxItemStatusesFail(string empty)
        {
            NetmeraCore.Instance.OnInboxStatusChangeFail(1, "changeInboxItemStatusesFailed");
        }

        public void changeInboxItemStatusesSuccess(string ok)
        {
            NetmeraCore.Instance.OnInboxStatusChangeSuccess();
        }

        public void changeAllInboxItemStatusesFail(string empty)
        {
            NetmeraCore.Instance.OnInboxStatusChangeFail(1, "changeAllInboxItemStatusesFailed");
        }

        public void changeAllInboxItemStatusesSuccess(string ok)
        {
            NetmeraCore.Instance.OnInboxStatusChangeSuccess();
        }


        public void getStatusCount(string count)
        {
            int countInt = Convert.ToInt32(count);
            NetmeraCore.Instance.OnInboxStatusCount(countInt);
        }

        /*

        
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

        */
    }
}