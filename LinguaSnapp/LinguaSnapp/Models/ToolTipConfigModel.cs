using SqlBaseLibrary;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Models
{
    [SQLite.Table("table_tooltip")]
    public class ToolTipConfigModel : DatabaseItem
    {
        public int DescriptorId { get; set; }

        public string Keyword { get; set; }

        public string Text { get; set; }

        internal ToolTipConfigModel(int descriptorId, string keyword, string text)
        {
            DescriptorId = descriptorId;
            Keyword = keyword;
            Text = text;
        }

        public ToolTipConfigModel()
        {

        }
    }
}
