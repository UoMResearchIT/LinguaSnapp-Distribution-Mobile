using System;
using System.Collections.Generic;
using System.Text;

namespace LinguaSnapp.Services
{
    struct UploadResult
    {
        public enum UploadAttemptResult
        {
            Unknown,
            NotInDatabase,
            SubmissionInvalid,
            ServerError,
            Success
        }

        public UploadAttemptResult Result { get; }

        public string Message { get; }

        public DataServiceReply DataServiceReply { get; }

        public UploadResult(UploadAttemptResult result, string message, DataServiceReply reply)
        {
            Result = result;
            Message = message;
            DataServiceReply = reply;
        }
    }
}
