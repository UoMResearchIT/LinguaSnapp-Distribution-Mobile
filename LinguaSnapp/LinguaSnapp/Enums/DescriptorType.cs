using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Enums
{
    // Must match the DescriptorId used in "descriptorValues.txt" config file.
    enum DescriptorType
    {
        Language = 1,
        Alphabet = 2,
        Position = 3,
        SignType = 4,
        Outlet = 5,
        Design = 6,
        Content = 7,
        Sector = 8,
        Audience = 9,
        Purpose = 10,
        Function = 11,
        Arrangement = 12,
        Dominance = 13
    }
}
