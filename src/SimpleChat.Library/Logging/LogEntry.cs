using Newtonsoft.Json;

namespace SimpleChat.Library.Logging
{
    public class LogEntry
    {
        public string Class { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string Parameters { get; set; } = string.Empty;

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
