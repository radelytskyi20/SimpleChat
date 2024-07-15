namespace SimpleChat.Library.Responses
{
    public class ServiceResponse<T>
    {
        public ServiceResponse(T payload)
        {
            Payload = payload;
        }

        public ServiceResponse(ICollection<string> errors)
        {
            Errors = errors;
        }

        public T Payload { get; set; } = default!;
        public ICollection<string> Errors { get; init; } = new List<string>();
        public bool IsSuccessfull => Errors.Count == 0;
    }
}
