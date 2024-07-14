namespace SimpleChat.Library.Requests
{
    public class UpdateUserRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
