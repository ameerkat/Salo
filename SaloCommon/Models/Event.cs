using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TritonSimulator.Models
{
    public class Event
    {
        [JsonProperty("event")]
        public string _event { get; set; }
        public Report report { get; set; }
    }
}
