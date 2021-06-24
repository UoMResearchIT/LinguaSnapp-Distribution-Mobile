using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Enums
{
    enum DataServiceReplyType
    {
        Unknown,
        NoInternet,
        RegisterFail,
        LoginFail,
        VerifyFail,
        UploadFail,
        Success,
        TimedOut
    }
}
