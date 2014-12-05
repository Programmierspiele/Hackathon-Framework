using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hackathonlib
{
    class HackathonPacket
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<GameObject> scene = null;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string command = null;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? speed = null;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public double? rotation = null;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string name = null;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ping = null;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
