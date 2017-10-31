using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Viime.Portable.WebServices
{
    public class ChannelsContract
    {
        [JsonProperty("odata.metadata")]
        public string ODataMetadata { get; set; }

        [JsonProperty("value")]
        public List<ValueContract> Value { get; set; }
    }
}


