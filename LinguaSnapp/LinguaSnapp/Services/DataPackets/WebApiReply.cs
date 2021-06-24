using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Services.DataPackets
{
    class WebApiReply
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public string Details { get; set; }

        public bool EOT { get; set; }
    }
}
