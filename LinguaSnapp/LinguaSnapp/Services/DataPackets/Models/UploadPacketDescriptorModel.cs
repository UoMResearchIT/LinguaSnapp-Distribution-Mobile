using LinguaSnapp.Models;
using LinguaSnapp.Services.DataPackets.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Services.DataPackets.Models
{
    class UploadPacketDescriptorModel : ModelWithLinkKey
    {
        [JsonProperty("code")]
        public string DescriptorTypeCode { get; set; }

        [JsonProperty("value_code")]
        public string Code { get; set; }

        [JsonProperty("other_text")]
        public string OtherText { get; set; }

        public UploadPacketDescriptorModel()
        {
        }

        public UploadPacketDescriptorModel(DescriptorModel model, string linkKey = "")
        {
            LinkKey = linkKey;
            DescriptorTypeCode = ConfigurationService.Instance.GetDescriptorTypeCode(model.Code);
            Code = model.Code;
            OtherText = model.Value;
        }
    }
}
