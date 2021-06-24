using LinguaSnapp.Services.DataPackets.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Services.DataPackets
{
    class VerifyPacket : WebApiUserPacket
    {
        public string userUUID { get; set; }

        public bool confirmed { get; set; } = true;
    }
}
