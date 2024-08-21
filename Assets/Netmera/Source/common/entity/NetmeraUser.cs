using System.Collections.Generic;

namespace Netmera
{
    public class NetmeraUser
    {
        public static readonly string USER_ID = "user_id";
        public static readonly string MSISDS = "msisdn";
        public static readonly string EMAIL = "email";
        public static readonly string NAME = "name";
        public static readonly string SURNAME = "surname";
        public static readonly string EXTERNAL_SEGMENTS = "external_segments";
        public static readonly string GENDER = "gender";
        public static readonly string DATE_OF_BIRTH = "date_of_birth";
        public static readonly string MARITAL_STATUS = "marital_status";
        public static readonly string CHILD_COUNT = "child_count";
        public static readonly string COUNTRY = "country";
        public static readonly string STATE = "state";
        public static readonly string CITY = "city";
        public static readonly string DISTRICT = "district";
        public static readonly string OCCUPATION = "occupation";
        public static readonly string INDUSTRY = "industry";
        public static readonly string FAVORITE_TEAM = "favorite_team";
        public static readonly string LANGUAGE = "language";

        private JSONObject _jsonNode;

        public JSONObject JsonNode => _jsonNode;

        public NetmeraUser()
        {
            _jsonNode = new JSONObject();
        }

        public void SetUserID(string userID)
        {
            _jsonNode[USER_ID] = userID;
        }

        public void SetMSISDN(string msisdn)
        {
            _jsonNode[MSISDS] = msisdn;
        }

        public void SetEmail(string email)
        {
            _jsonNode[EMAIL] = email;
        }

        public void SetName(string name)
        {
            _jsonNode[NAME] = name;
        }

        public void SetSurname(string surname)
        {
            _jsonNode[SURNAME] = surname;
        }

        public void SetExternalSegments(List<string> externalSegments)
        {
            string result = "";
            if (externalSegments != null && externalSegments.Count > 0)
            {
                result = externalSegments[0];
                for (var i = 1; i < externalSegments.Count; i++)
                {
                    result += "," + externalSegments[i];
                }
            }
            else
            {
                result = null;
            }

            _jsonNode[EXTERNAL_SEGMENTS] = result;
        }

        public void SetGender(NetmeraEnum.Gender gender)
        {
            _jsonNode[GENDER] = (int) gender;
        }


        public void SetBirthday(string year, string month, string day)
        {
            _jsonNode[DATE_OF_BIRTH] = $"{year}-{month}-{day}";
        }

        public void SetMaritalStatus(NetmeraEnum.MaritalStatus status)
        {
            _jsonNode[MARITAL_STATUS] = (int) status;
        }

        public void SetChildCount(int childCount)
        {
            _jsonNode[CHILD_COUNT] = childCount;
        }

        public void SetCountry(string country)
        {
            _jsonNode[COUNTRY] = country;
        }

        public void SetState(string state)
        {
            _jsonNode[STATE] = state;
        }

        public void SetCity(string city)
        {
            _jsonNode[CITY] = city;
        }

        public void SetDistrict(string district)
        {
            _jsonNode[DISTRICT] = district;
        }

        public void SetOccupation(string occupation)
        {
            _jsonNode[OCCUPATION] = occupation;
        }

        public void SetIndustry(string industry)
        {
            _jsonNode[INDUSTRY] = industry;
        }

        public void SetFavoriteTeam(string team)
        {
            _jsonNode[FAVORITE_TEAM] = team;
        }

        public void SetLanguage(string language)
        {
            _jsonNode[LANGUAGE] = language;
        }
        
    }
}