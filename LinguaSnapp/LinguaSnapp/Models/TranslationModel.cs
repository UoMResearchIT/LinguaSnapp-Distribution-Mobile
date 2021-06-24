using LinguaSnapp.Models.Base;
using LinguaSnapp.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LinguaSnapp.Models
{
    class TranslationModel : StringEncodableModel, IEqualityComparer<TranslationModel>
    {
        public string TranslationId { get; private set; }

        public string LanguageCode { get; private set; }

        public string LanguageOtherValue { get; private set; }

        public string AlphabetCode { get; private set; }

        public string AlphabetOtherValue { get; private set; }

        public string Translation { get; private set; }

        public TranslationModel()
        {
        }

        public TranslationModel(string langCode, string alphaCode, string trans, string langOther, string alphaOther)
        {
            TranslationId = Guid.NewGuid().ToString();
            LanguageCode = langCode;
            LanguageOtherValue = langOther;
            AlphabetCode = alphaCode;
            AlphabetOtherValue = alphaOther;
            Translation = trans;
        }

        internal override void Decode(string encodedModel)
        {
            var decodedValues = encodedModel.Split('|');
            if (decodedValues.Length < 6)
            {
                Debug.WriteLine($"Expected 6 encoded properties in encoded translation. Found only {decodedValues.Length}", "ERROR");
                return;
            }
            TranslationId = decodedValues[0];
            LanguageCode = decodedValues[1];
            LanguageOtherValue = string.IsNullOrWhiteSpace(decodedValues[2]) ? null : decodedValues[2];
            AlphabetCode = decodedValues[3];
            AlphabetOtherValue = string.IsNullOrWhiteSpace(decodedValues[4]) ? null : decodedValues[4];
            Translation = decodedValues[5];
        }

        internal override string Encode()
        {
            return string.Join("|", TranslationId, LanguageCode, LanguageOtherValue, AlphabetCode, AlphabetOtherValue, Translation);
        }

        // Equal if the ID is equal as this should be unique
        public bool Equals(TranslationModel x, TranslationModel y)
        {
            // Check whether the compared objects reference the same data
            if (ReferenceEquals(x, y)) return true;

            // Check whether any of the compared objects is null
            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            // Check ID property
            return x.TranslationId == y.TranslationId;
        }

        // If Equals() returns true for a pair of objects then GetHashCode() 
        // must return the same value for these objects
        public int GetHashCode(TranslationModel model)
        {
            // Check whether the object is null
            if (ReferenceEquals(model, null)) return 0;

            // Get hash code for the ID if it is not null
            return model.TranslationId == null ? 0 : model.TranslationId.GetHashCode();
        }
    }
}
