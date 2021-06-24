using LinguaSnapp.Enums;
using LinguaSnapp.Models.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LinguaSnapp.Models
{
    class DescriptorModel : StringEncodableModel
    {
        public string Code { get; private set; }

        public string Value { get; private set; }

        public DescriptorType DescriptorType { get; private set; }

        internal DescriptorModel()
        {
        }

        internal DescriptorModel(DescriptorType type, string code, string otherValue = null)
        {
            DescriptorType = type;
            Code = code;
            Value = otherValue;
        }

        internal override void Decode(string encodedModel)
        {
            var decodedValues = encodedModel.Split('|');
            DescriptorType = (DescriptorType)Enum.Parse(typeof(DescriptorType), decodedValues[0]);
            Code = decodedValues[1];
            Value = decodedValues.Length > 2 && !string.IsNullOrWhiteSpace(decodedValues[2]) ?
                decodedValues[2] :
                null;
        }

        internal override string Encode()
        {
            return string.Join("|", DescriptorType, Code, Value);
        }
    }
}
