using LinguaSnapp.Enums;
using SqlBaseLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Models
{
    [SQLite.Table("table_server_config")]
    public class ServerEndpointConfigModel : DatabaseItem
    {
        public ServerEndpointType Type { get; set; }

        public string URL { get; set; }

        internal ServerEndpointConfigModel(ServerEndpointType type, string url)
        {
            Type = type;
            URL = url;
        }

        public ServerEndpointConfigModel()
        {

        }
    }
}
