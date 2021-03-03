namespace Casino_Royale_Api.Constants
{
    public static class ResponseMessages
    {
        public static string InternalServerErrorMessage = "Internal Database failure. Please try again later.";
        public static string PostNullObjectErrorMessage = "Cannot POST null object";
        public static string PutNullObjectErrorMessage = "Cannot PUT null object";

        public static string PlayerDoesNotExistMessage(string username)
        {
            return $"Player with username {username} does not exist.";
        }
    }
}