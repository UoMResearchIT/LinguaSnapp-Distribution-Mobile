using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Services.DataPackets.Base
{
    abstract class ModelWithLinkKey
    {
        [JsonProperty("link_key")]
        public string LinkKey { get; set; }
    }
}
