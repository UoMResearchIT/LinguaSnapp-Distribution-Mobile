using LinguaSnapp.Models;
using LinguaSnapp.Services.DataPackets.Base;
using LinguaSnapp.Services.DataPackets.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Services.DataPackets
{
    class UploadPacket : WebApiPacket
    {
        [JsonProperty("unique_id")]
        public string UniqueId { get; set; }

        [JsonProperty("userid")]
        public string UserId { get; set; }

        [JsonProperty("deviceid")]
        public string DeviceId { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("date_created")]
        public string DateCreated { get; set; }

        [JsonProperty("image_data")]
        public string ImageData { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("comments")]
        public string Comments { get; set; }

        [JsonProperty("descriptors")]
        public List<UploadPacketDescriptorModel> Descriptors { get; set; } = new List<UploadPacketDescriptorModel>();

        [JsonProperty("translations")]
        public List<UploadPacketTranslationModel> Translations { get; set; } = new List<UploadPacketTranslationModel>();
    }
}
