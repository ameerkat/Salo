using Newtonsoft.Json;
namespace Salo.Live.Models
{
    public class Event
    {
        [JsonProperty("event")]
        public string _event { get; set; }
        public Report report { get; set; }
    }
}
