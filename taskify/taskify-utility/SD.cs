namespace taskify_utility
{
    public static class SD
    {
        public enum ApiType
        {
            GET, POST, PUT, DELETE
        }

        public enum ActivityType
        {
            CREATE, UPDATE, DELETE
        }

        public enum TaskAccessbility
        {
            AssignedUser, ProjectUser
        }

        public const string Admin = "admin";
        public const string Client = "client";

        public static string AccessToken = "JWTToken";
        public static string RefreshToken = "RefreshToken";
        public static string CurrentAPIVersion = "v1";
        public enum ContentType
        {
            Json,
            MultipartFormData
        }
    }
}