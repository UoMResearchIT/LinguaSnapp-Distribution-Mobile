using LinguaSnapp.Enums;
using LinguaSnapp.Services.DataPackets;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;

namespace LinguaSnapp.Services
{
    class DataServiceReply
    {
        public int Code { get; }

        public string Message { get; }

        public bool Success { get; }

        public WebApiReply Data { get; }

        public DataServiceReplyType Reply { get; }

        internal DataServiceReply(bool success, DataServiceReplyType reply, HttpResponseMessage response, WebApiReply data = null)
        {
            Reply = reply;
            Success = success;
            Data = data;

            // If we have data then use the Code and Message from the data not the HTTP Response
            if (data != null)
            {
                Code = int.Parse(data.Code);
                Message = data.Message;
            }
            else
            {
                Code = response == null ? -1 : (int)response.StatusCode;
                Message = reply == DataServiceReplyType.NoInternet ? (string)Application.Current.Resources["alert_no_internet"] : response?.ReasonPhrase;
            }
        }
    }
}
