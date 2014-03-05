using Newtonsoft.Json;
namespace Salo
{
    public class Event
    {
        [JsonProperty("event")]
        public string EventName { get; set; }

        [JsonProperty("report")]
        public Report Report { get; set; }
    }
}
