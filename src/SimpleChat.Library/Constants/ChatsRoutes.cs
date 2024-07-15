namespace SimpleChat.Library.Constants
{
    public static class ChatsRoutes
    {
        public const string GetAllByUser = $"{RepoActions.GetAll}/user";
        public const string GetAllByName = $"{RepoActions.GetAll}/{{name}}";
    }
}
