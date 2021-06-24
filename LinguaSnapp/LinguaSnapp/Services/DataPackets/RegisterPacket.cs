using LinguaSnapp.Services.DataPackets.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Services.DataPackets
{
    class RegisterPacket : LoginRegisterPacket
    {
        public string emailAddress { get; set; }

        public bool mailingList { get; set; }
    }
}
