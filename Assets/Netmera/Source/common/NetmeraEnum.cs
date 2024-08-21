namespace Netmera
{
    public class NetmeraEnum
    {
        public enum CommonEvents
        {
            Login = 1,
            Register = 2,
            Search = 3,
            Share = 4,
            IapPurchase = 5,
            BannerClick = 6,
            CategoryView = 7
        }

        public enum CommerceEvents
        {
            ProductView = 1,
            ProductRate = 2,
            ProductComment = 3,
            OrderCancel = 4,
            Purchase = 5,
            CartView = 6,
            AddToChart = 7,
            RemoveFromChart = 8
        }

        public enum MediaEvents
        {
            ContentComment = 1,
            ContentRate = 2,
            ContentView = 3
        }

        public enum Gender
        {
            Male = 0,
            Female = 1,
            NotSpecified = 2
        }

        public enum MaritalStatus
        {
            Single = 0,
            Married = 1,
            NotSpecified = 2
        }


        public enum PushStatus
        {
            Read = 1,
            Unread = 2,
            ReadOrUnread =3,
            Deleted = 4,
            All = 7
        }
    }
}