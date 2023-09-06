using AuthoritySTS.Models.AccountViewModels;

namespace AuthoritySTS
{
    public class JsonSettings
    {
        public static JsonData UserDataWithStatusMessage(object obj, int iStatus, string message)
        {
            JsonData jsonData = new JsonData();
            jsonData.status = iStatus;
            jsonData.message = message;
            if (obj == null)
            {
                obj = new object();
            }
            jsonData.data = new object[1];
            jsonData.data[0] = obj;
            return jsonData;
        }

        public static CustomJsonData UserCustomDataWithStatusMessage(object obj, int iStatus, string message)
        {
            CustomJsonData jsonData = new CustomJsonData();
            jsonData.status = iStatus;
            jsonData.message = message;
            if (obj == null)
            {
                obj = new object();
            }

            jsonData.data = obj;
            return jsonData;
        }

        public static string ResponseStatusMessage(int iStatus)
        {
            string strMessage = null;
            switch (iStatus)
            {
                case 200:
                    strMessage = "Ok";
                    break;
                case 400:
                    strMessage = "Bad Request";
                    break;
                case 401:
                    strMessage = "Unauthorized Access";
                    break;
                case 404:
                    strMessage = "Not Found";
                    break;
                case 500:
                    strMessage = "Internal Server Error";
                    break;
            }
            return strMessage;
        }

    }
}
