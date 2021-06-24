using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace LinguaSnapp.Services.DataPackets.Base
{
    abstract class LoginRegisterPacket : WebApiUserPacket
    {
        [JsonProperty("usercode")]
        public string Usercode { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}
