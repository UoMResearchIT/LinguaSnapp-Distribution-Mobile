using LinguaSnapp.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Services.DataPackets.Base
{
    abstract class WebApiPacket
    {
        public bool EOT { get; set; } = true;
    }
}
