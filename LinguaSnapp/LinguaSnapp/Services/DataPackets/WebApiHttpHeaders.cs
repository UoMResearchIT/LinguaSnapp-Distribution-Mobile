using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Services.DataPackets
{
    class WebApiHttpHeaders
    {
        public string Accept { get; }
        public string Token { get; }
        public string Content { get; }

        public WebApiHttpHeaders(string content = null, string accept = null, string token = null)
        {
            Content = content;
            Accept = accept;
            Token = token;
        }
    }
}
