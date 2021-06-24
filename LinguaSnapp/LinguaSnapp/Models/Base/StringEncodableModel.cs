using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Models.Base
{
    abstract class StringEncodableModel
    {
        internal abstract string Encode();

        internal abstract void Decode(string encodedModel);
    }
}
