using System;
using System.Collections;
using System.Collections.Generic;
using Netmera;
using UnityEngine;

public class NetmeraGameObject : MonoBehaviour, Netmera.Callback
{

    private void Awake()
    {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        DontDestroyOnLoad(gameObject);
        
        
        Test_Init();
    }


    public void Test_Init()
    {
        if (NetmeraCore.Instance == null)
        {
            return;
        }
        NetmeraCore.Instance.Init(true,this);
        NetmeraCore.Instance.RequestNotificationPermissionForIOS();
    }

    /* Sample method calls */
    public void Test_SendEvent()
    {
        JSONObject obj = new JSONObject();
        obj["ea"] = "anil";
        obj["ec"] = 10;
        NetmeraCore.Instance.SendEvent("zzl", obj);
    }

    public void Test_UpdateUser()
    {
        NetmeraUser user = new NetmeraUser();
        user.SetUserID("bsoykal");
        user.SetBirthday("1991", "06", "30");
        user.SetCity("Istanbul");
        NetmeraCore.Instance.UpdateUser(user.JsonNode);
    }

    public void Test_FetchInbox()
    {
        List<string> categories = new List<string>() {};
        NetmeraCore.Instance.FetchInbox(20, NetmeraEnum.PushStatus.ReadOrUnread, categories, true);
    }

    public void Test_FetchNextPage()
    {
        NetmeraCore.Instance.FetchNextPage();
    }

    public void Test_Destroy()
    {
        NetmeraCore.Instance.Destroy();
    }

    public void Test_EnablePopupPresentation(bool isEnabled)
    {
        NetmeraCore.Instance.EnablePopupPresentation(isEnabled);
    }

    public void Test_GetStatusCount()
    {
        NetmeraCore.Instance.GetStatusCount(NetmeraEnum.PushStatus.ReadOrUnread);
    }

    public void Test_ChangeInboxItemStatuses()
    {
        NetmeraCore.Instance.ChangeInboxItemStatuses(1, 2, NetmeraEnum.PushStatus.Deleted);
    }

    public void Test_RequestPermissionsForLocation()
    {
        NetmeraCore.Instance.RequestPermissionsForLocation();
    }

    public void Test_ChangeAllInboxItemStatuses()
    {
        NetmeraCore.Instance.ChangeAllInboxItemStatuses(NetmeraEnum.PushStatus.Deleted);
    }


    // Callbacks
    public void OnPushRegister(string gcmSenderId, string pushToken)
    {
        NetmeraCore.Instance.log($"gameobject callback OnPushRegister gcmSenderId: {gcmSenderId} pushToken: {pushToken}");
    }

    public void OnPushReceive(JSONNode rawJson, NetmeraPushObject pushObject)
    {
        NetmeraCore.Instance.log($"gameobject callback OnPushReceive rawJson: {rawJson} pushObject: {pushObject}");
    }

    public void OnPushOpen(JSONNode rawJson, NetmeraPushObject pushObject)
    {
        NetmeraCore.Instance.log($"gameobject callback OnPushOpen rawJson: {rawJson} pushObject: {pushObject}");
    }

    public void OnPushDismiss(JSONNode rawJson, NetmeraPushObject pushObject)
    {
        NetmeraCore.Instance.log($"gameobject callback OnPushDismiss rawJson: {rawJson} pushObject: {pushObject}");
    }

    public void OnPushButtonClicked(JSONNode rawJson, NetmeraPushObject pushObject)
    {
        NetmeraCore.Instance.log($"gameobject callback OnPushButtonClicked rawJson: {rawJson} pushObject: {pushObject}");

    }

    public void OnInboxFetchSuccess(JSONNode netmeraPushInboxJSON)
    {
        NetmeraCore.Instance.log($"gameobject callback OnInboxFetchSuccess netmeraPushInboxJSON: {netmeraPushInboxJSON}");
    }

    public void OnInboxFetchFail(int errorCode, string errorMessage)
    {
        NetmeraCore.Instance.log("gameobject callback OnInboxFetchFail errorMessage: " + errorMessage);
    }

    public void OnInboxNextPageFetchSuccess(JSONNode netmeraPushInboxJSON)
    {
        NetmeraCore.Instance.log($"gameobject callback OnInboxNextPageFetchSuccess netmeraPushInboxJSON: {netmeraPushInboxJSON}" );
    }

    public void OnInboxNextPageFetchFail(int errorCode, string errorMessage)
    {
        NetmeraCore.Instance.log("gameobject callback OnInboxNextPageFetchFail errorMessage: " + errorMessage);
    }

    public void OnInboxStatusChangeSuccess()
    {
        NetmeraCore.Instance.log("gameobject callback OnInboxStatusChangeSuccess");
    }

    public void OnInboxStatusChangeFail(int errorCode, string errorMessage)
    {
        NetmeraCore.Instance.log("gameobject callback OnInboxStatusChangeFail errorMessage: " +errorMessage);
    }

    public void OnInboxStatusCount(int countWithThatStatus)
    {
        NetmeraCore.Instance.log("gameobject callback OnInboxStatusCount countWithThatStatus: " +countWithThatStatus);
    }
}