using Newtonsoft.Json;

namespace Salo.Live.Models
{
    public class Request
    {
        public static class OrderType
        {
            public const string FullUniverseReport = "full_universe_report";
        }

        [JsonProperty("type")]
        public string RequestType { get; set; }
        
        [JsonProperty("order")]
        public string Order { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("game_number")]
        public string GameId { get; set; }
    }
}
