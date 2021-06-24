using SqlBaseLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Models
{
    [SQLite.Table("table_descriptor")]
    public class DescriptorConfigModel : DatabaseItem
    {
        public int DescriptorType { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        internal DescriptorConfigModel(int descriptorType, string code, string name)
        {
            DescriptorType = descriptorType;
            Code = code;
            Name = name;
        }

        public DescriptorConfigModel()
        {
        }
    }
}
