using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace LinguaSnapp.Services.DataPackets.Base
{
    abstract class WebApiUserPacket : WebApiPacket
    {
        [JsonProperty("deviceUUID")]
        public string DeviceUUID { get; set; }
    }
}
