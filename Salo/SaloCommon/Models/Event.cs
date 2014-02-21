using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaloCommon.Models
{
    public class Event
    {
        [JsonProperty("event")]
        public string _event { get; set; }
        public Report report { get; set; }
    }
}
