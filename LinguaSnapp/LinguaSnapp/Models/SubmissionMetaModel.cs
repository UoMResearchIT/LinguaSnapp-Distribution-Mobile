using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Models
{
    class SubmissionMetaModel
    {
        public string GUID { get; private set; }

        public string DateCreated { get; private set; }

        public string EncodedPhoto { get; private set; }

        public string EncodedThumbnail { get; private set; }

        public double Latitude { get; private set; }

        public double Longitude { get; private set; }

        public string Title { get; private set; }

        public string NumLang { get; private set; }

        public string NumAlpha { get; private set; }

        public SubmissionMetaModel(
            string guid,
            string datacreated,
            string encodedPhoto,
            string encodedThumbnail,
            double latitude,
            double longitude,
            string title,
            string lang,
            string alpha
            )
        {
            GUID = guid;
            DateCreated = datacreated;
            EncodedPhoto = encodedPhoto;
            EncodedThumbnail = encodedThumbnail;
            Latitude = latitude;
            Longitude = longitude;
            Title = title;
            NumLang = lang;
            NumAlpha = alpha;
        }
    }
}
