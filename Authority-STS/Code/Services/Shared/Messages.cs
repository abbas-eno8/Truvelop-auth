namespace AuthoritySTS.Services.Shared
{
    public static class Messages
    {
        public const string ok = "Ok";

        public const string EmailConfirmationTokenProviderName = "ConfirmEmail";
        public const string AuthorityConnectionString = "1AuthorityConnection";

        #region Sql Procedure name

        public const string GetCompanyWiseAzureAdDetails = "usp_getCompanyWiseAzureAdDetails";
        public const string GetAzureADProviderByUserEmail = "usp_getAzureADProviderByUserEmail";
        public const string GetUserClaimsByEmail = "usp_GetUserClaimsByEmail";
        public const string GetTemplateByName = "usp_GetTemplateByName";

        #endregion

        #region Messages
        public const string ContactAdministrator = "Please contact to administrator.";
        public const string LoggedUserNotFound = "Logged user  {0}  not found, Please contact to administrator.";
        public const string ExternalLoginNotAllowed = "You are not allowed to do External Login.";
        public const string EnteredUserNotFound = "Entered user not found, Please contact to administrator.";
        public const string ClaimValueNotFound = "Claim Value not found, Please contact to administrator.";
        public const string SomethingWentWrong = "Something went wrong, Please contact to administrator.";
        #endregion

        #region External Provider
        public const string Google = "Google";
        public const string AzureAD = "AzureAD_";
        public const string UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
        public const string TriggerMobileApp = "TriggerMobileApp";
        #endregion
    }
}
