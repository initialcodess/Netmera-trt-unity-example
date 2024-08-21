using System;

namespace Netmera
{
    public class NetmeraPushObject
    {
        private JSONNode customJson;
        private string deeplinkUrl;
        private string title;
        private string subtitle;
        private string body;
        private string pushId;
        private string pushInstanceId;
        private int pushType;
        private int inboxStatus;
        private long sendDate;

        public NetmeraPushObject(JSONNode json)
        {
            string customData = json["customJson"];
            if (!string.IsNullOrEmpty(customData))
            {
                try
                {
                    customJson = JSON.Parse(customData);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            deeplinkUrl = json["deeplinkUrl"];
            title = json["title"];
            subtitle = json["subtitle"];
            body = json["body"];
            pushId = json["pushId"];
            pushInstanceId = json["pushInstanceId"];
            pushType = json["pushType"];
            inboxStatus = json["inboxStatus"];
            sendDate = json["sendDate"];
        }
    }
}