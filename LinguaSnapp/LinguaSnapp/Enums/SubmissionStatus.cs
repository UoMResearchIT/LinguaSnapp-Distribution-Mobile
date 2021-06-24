using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace LinguaSnapp.Enums
{
    public enum SubmissionStatus
    {
        [Description("Draft")]
        Draft,

        [Description("Waiting to be Sent")]
        Outbox,

        [Description("Sent")]
        Sent,

        [Description("Unknown")]
        Unknown
    }
}
