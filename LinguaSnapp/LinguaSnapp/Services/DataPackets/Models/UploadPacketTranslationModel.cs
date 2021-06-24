using LinguaSnapp.Models;
using LinguaSnapp.Services.DataPackets.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Services.DataPackets.Models
{
    class UploadPacketTranslationModel : ModelWithLinkKey
    {
        [JsonProperty("translation")]
        public string Translation { get; set; }

        public UploadPacketTranslationModel(TranslationModel model)
        {
            LinkKey = model.TranslationId;
            Translation = model.Translation;
        }
    }
}
