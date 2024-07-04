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
        public const string UrlImageUser = "UserImage";
        public const string UrlImageDefault = "https://placehold.co/600x400";
        public const string UrlImageAvatarDefault = "default-avatar.png";

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