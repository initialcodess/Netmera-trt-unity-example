namespace Netmera
{
    public interface Callback
    {
        void OnPushRegister(string gcmSenderId, string pushToken);
        void OnPushReceive(JSONNode rawJson, NetmeraPushObject pushObject);
        void OnPushOpen(JSONNode rawJson, NetmeraPushObject pushObject);
        void OnPushDismiss(JSONNode rawJson, NetmeraPushObject pushObject);
        void OnPushButtonClicked(JSONNode rawJson, NetmeraPushObject pushObject);

        void OnInboxFetchSuccess(JSONNode netmeraPushInboxJSON);
        void OnInboxFetchFail(int errorCode, string errorMessage);
        void OnInboxNextPageFetchSuccess(JSONNode netmeraPushInboxJSON);
        void OnInboxNextPageFetchFail(int errorCode,string errorMessage);
        void OnInboxStatusChangeSuccess();
        void OnInboxStatusChangeFail(int errorCode,string errorMessage);
        void OnInboxStatusCount(int countWithThatStatus);
    }
}