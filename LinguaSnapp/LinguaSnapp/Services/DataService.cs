using LinguaSnapp.Enums;
using LinguaSnapp.Models;
using LinguaSnapp.Services.DataPackets;
using LinguaSnapp.Services.DataPackets.Base;
using LinguaSnapp.Services.DataPackets.Models;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LinguaSnapp.Services
{
    sealed class DataService
    {
        private static readonly Lazy<DataService> lazy = new Lazy<DataService>(() => new DataService());

        public static DataService Instance { get { return lazy.Value; } }

        private HttpClient client = new HttpClient();

        private DataService()
        {
            // Set base URL
            client.BaseAddress = new Uri(ConfigurationService.Instance.GetURL(ServerEndpointType.BaseUrl));
        }

        // Check to see whether we have valid tokens
        public async Task<bool> IsLoggedInAsync()
        {
            try
            {
                // Check for existing tokens
                var userid = await GetUserIdAsync();
                var deviceid = await GetDeviceIdAsync();
                if (userid == null || deviceid == null) return false;

                // Verify existing tokens
                var res = await VerifyAsync();

                // If we failed to verify due to no internet then carry on anyway
                return (res.Reply == DataServiceReplyType.NoInternet || res.Success);
            }
            catch (Exception e)
            {
                var msg = $"Error getting or verifying IDs: {e.Message}";
                Debug.WriteLine(msg);
                Crashes.TrackError(new Exception(msg, e));
            }
            return false;
        }

        // Public register wrapper
        public async Task<DataServiceReply> RegisterAsync(string usercode, string password, string emailAddress, bool mailingList)
        {
            return await LoginOrRegisterAsync(true, usercode, password, emailAddress, mailingList);
        }

        // Public login wrapper
        public async Task<DataServiceReply> LoginAsync(string usercode, string password)
        {
            return await LoginOrRegisterAsync(false, usercode, password);
        }

        // Public upload wrapper -- uploads a single submission
        internal async Task<UploadResult> UploadSubmissionAsync(int? id)
        {
            try
            {
                // Set submission service
                var sub = SubmissionService.Instance;

                // Switch active submission in the submision service
                var active = await sub.StartEditingAsync(id, false);

                // Should never happen but just in case
                if (!active)
                {
                    var msg = $"Cannot find submission with id {id} in the database!";
                    Debug.WriteLine(msg);
                    Crashes.TrackError(new Exception(msg));
                    return new UploadResult(UploadResult.UploadAttemptResult.NotInDatabase, (string)Application.Current.Resources["alert_upload_not_database"], null);
                }

                // Check drafts for validity
                if (sub.GetSubmissionStatus() == SubmissionStatus.Draft && !sub.IsValidSubmission())
                {
                    return new UploadResult(UploadResult.UploadAttemptResult.SubmissionInvalid, (string)Application.Current.Resources["alert_upload_invalid"], null);
                }

                // Attempt to upload active submission
                var res = await UploadAsync();

                // Set status based on result and save to DB
                await sub.StopEditingAsync(res.Success ? SubmissionStatus.Sent : SubmissionStatus.Outbox);

                // Update preferences
                if (res.Success)
                {
                    var current = Preferences.Get((string)Application.Current.Resources["key_num_sent"], 0);
                    Preferences.Set((string)Application.Current.Resources["key_num_sent"], current + 1);
                }

                // Return result
                return new UploadResult(
                    res.Success ?
                    UploadResult.UploadAttemptResult.Success :
                    UploadResult.UploadAttemptResult.ServerError,
                    (string)Application.Current.Resources["alert_upload_server"],
                    res
                );
            }
            catch (Exception e)
            {
                return new UploadResult(UploadResult.UploadAttemptResult.Unknown, $"{(string)Application.Current.Resources["alert_upload_unknown"]}\n{e.Message}", null);
            }
        }

        // Upload method
        private async Task<DataServiceReply> UploadAsync()
        {
            if (!InternetAvailable()) return new DataServiceReply(false, DataServiceReplyType.NoInternet, null);

            // Create an upload packet
            var packet = new UploadPacket();

            // Get models form the submission service
            var metaModel = SubmissionService.Instance.GetSubmissionMeta();

            // Build packet using basic model data
            packet.UniqueId = metaModel.GUID;
            packet.ImageData = metaModel.EncodedPhoto;
            packet.Latitude = metaModel.Latitude;
            packet.Longitude = metaModel.Longitude;
            packet.DateCreated = metaModel.DateCreated;
            packet.Title = metaModel.Title;
            packet.Comments = SubmissionService.Instance.GetComments();

            // Add the user details
            packet.DeviceId = await GetDeviceIdAsync();
            packet.UserId = await GetUserIdAsync();

            // Add descriptor for number of languages
            packet.Descriptors.Add(new UploadPacketDescriptorModel()
            {
                Code = "NL001",
                DescriptorTypeCode = ConfigurationService.Instance.GetDescriptorTypeCode("NL001"),
                OtherText = metaModel.NumLang
            });

            // Add descriptor for number of alphabets
            packet.Descriptors.Add(new UploadPacketDescriptorModel()
            {
                Code = "NA001",
                DescriptorTypeCode = ConfigurationService.Instance.GetDescriptorTypeCode("NA001"),
                OtherText = metaModel.NumAlpha
            });

            // Get all descriptors from submission and add to packet
            foreach (var d in SubmissionService.Instance.GetDescriptors())
            {
                packet.Descriptors.Add(new UploadPacketDescriptorModel(d));
            };

            // Add descriptor for dominance
            packet.Descriptors.Add(new UploadPacketDescriptorModel()
            {
                Code = "ON001",
                DescriptorTypeCode = ConfigurationService.Instance.GetDescriptorTypeCode("ON001"),
                OtherText = SubmissionService.Instance.GetOneLanguageDominant() ? "yes" : "no"
            });

            // Build translations into packet
            var translations = SubmissionService.Instance.GetTranslations();
            foreach (var t in translations)
            {
                // Get descriptor models for the language and translations
                var langDescConfig = ConfigurationService.Instance.GetDescriptorFromCode(t.LanguageCode);
                var langDesc = new DescriptorModel((DescriptorType)Enum.Parse(typeof(DescriptorType), langDescConfig.DescriptorType.ToString()), t.LanguageCode, t.LanguageOtherValue);
                var alphaDescConfig = ConfigurationService.Instance.GetDescriptorFromCode(t.AlphabetCode);
                var alphaDesc = new DescriptorModel((DescriptorType)Enum.Parse(typeof(DescriptorType), alphaDescConfig.DescriptorType.ToString()), t.AlphabetCode, t.AlphabetOtherValue);

                // Add language and alphabet descriptors
                packet.Descriptors.Add(new UploadPacketDescriptorModel(langDesc, t.TranslationId));
                packet.Descriptors.Add(new UploadPacketDescriptorModel(alphaDesc, t.TranslationId));

                // Add translation
                packet.Translations.Add(new UploadPacketTranslationModel(t));
            }

            // Send upload message
            HttpResponseMessage response = new HttpResponseMessage();
            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    response = await client.SendAsync(
                        CreateHttpRequestMessage(
                            ServerEndpointType.Upload,
                            packet,
                            new WebApiHttpHeaders("application/json")
                        ),
                        cts.Token
                    );
                    if (!response.IsSuccessStatusCode)
                    {
                        return new DataServiceReply(false, DataServiceReplyType.UploadFail, response);
                    }
                }

                // Catch time outs
                catch (OperationCanceledException) when (!cts.Token.IsCancellationRequested)
                {
                    return new DataServiceReply(false, DataServiceReplyType.TimedOut, response);
                }

                // Catch general issues
                catch (Exception e)
                {
                    var msg = "Upload process failed with an exception!";
                    Debug.WriteLine(msg);
                    Crashes.TrackError(new Exception(msg, e));
                }
            }

            // Get data from response
            var reply = await response.Content.ReadAsStringAsync();
            var replyData = JsonConvert.DeserializeObject<WebApiReply>(reply);

            // Check the status codes from the message
            var codeAsInt = int.Parse(replyData.Code);
            if (codeAsInt >= 200 && codeAsInt <= 299)
            {
                return new DataServiceReply(false, DataServiceReplyType.UploadFail, response, replyData);
            }

            // All done so return success
            return new DataServiceReply(true, DataServiceReplyType.Success, response);
        }

        // Pull the user ID from storage
        private async Task<string> GetUserIdAsync()
        {
            try
            {
                return await SecureStorage.GetAsync((string)Application.Current.Resources["key_user_id"]);
            }
            catch (Exception e)
            {
                var msg = $"Error getting UserId: {e.Message}";
                Debug.WriteLine(msg);
                Crashes.TrackError(new Exception(msg, e));
            }
            return null;
        }

        // Pull the device ID from storage
        private async Task<string> GetDeviceIdAsync()
        {
            try
            {
                return await SecureStorage.GetAsync((string)Application.Current.Resources["key_device_id"]);
            }
            catch (Exception e)
            {
                var msg = $"Error getting DeviceId: {e.Message}";
                Debug.WriteLine(msg);
                Crashes.TrackError(new Exception(msg, e));
            }
            return null;
        }

        // Store the IDs assuming they are in a concatenated format straight from the web service
        private async Task<bool> StoreIdsAsync(string concatIds)
        {
            try
            {
                var ids = concatIds.Split(':');
                if (ids.Length < 2 || string.IsNullOrWhiteSpace(ids[0]) || string.IsNullOrWhiteSpace(ids[1]))
                    throw new Exception("IDs not of expected length or number!");
                await SecureStorage.SetAsync((string)Application.Current.Resources["key_user_id"], ids[0]);
                await SecureStorage.SetAsync((string)Application.Current.Resources["key_device_id"], ids[1]);
                return true;
            }
            catch (Exception e)
            {
                var msg = $"Error storing IDs: {e.Message}";
                Debug.WriteLine(msg);
                Crashes.TrackError(new Exception(msg, e));
            }
            return false;
        }

        // Clear the stored IDs
        private void ClearIds()
        {
            try
            {
                SecureStorage.Remove((string)Application.Current.Resources["key_user_id"]);
                SecureStorage.Remove((string)Application.Current.Resources["key_device_id"]);
            }
            catch (Exception e)
            {
                var msg = $"Error clearing IDs: {e.Message}";
                Debug.WriteLine(msg);
                Crashes.TrackError(new Exception(msg, e));
            }
        }

        // General method to contact the server for a login or registration
        private async Task<DataServiceReply> LoginOrRegisterAsync(bool isRegister, string usercode, string password, string emailAddress = null, bool mailingList = false)
        {
            if (!InternetAvailable()) return new DataServiceReply(false, DataServiceReplyType.NoInternet, null);

            // Get GUID from preferences or generate new one
            var uuid = Preferences.Get((string)Application.Current.Resources["key_guid"], Guid.NewGuid().ToString());
            Preferences.Set((string)Application.Current.Resources["key_guid"], uuid);

            // If no email supplied then set to UUID
            if (string.IsNullOrWhiteSpace(emailAddress))
            {
                emailAddress = uuid;
            }

            // This is a legacy thing where the web app expects a particular
            // version or it tells the user to update
#if LS
            var version = "1.0.4";
#else
            var version = "1.0.0";
#endif

            // Build message
            WebApiPacket packet;
            if (isRegister)
            {
                packet = new RegisterPacket
                {
                    DeviceUUID = uuid,
                    Usercode = usercode,
                    Password = password,
                    Version = version,
                    emailAddress = emailAddress,
                    mailingList = mailingList
                };
            }
            else
            {
                packet = new LoginPacket
                {
                    DeviceUUID = uuid,
                    Usercode = usercode,
                    Password = password,
                    Version = version
                };
            }

            // Send registration message
            HttpResponseMessage response = new HttpResponseMessage();
            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    response = await client.SendAsync(
                        CreateHttpRequestMessage(
                            isRegister ? ServerEndpointType.Register : ServerEndpointType.Login,
                            packet,
                            new WebApiHttpHeaders("application/json")
                        ),
                        cts.Token
                    );
                    if (!response.IsSuccessStatusCode)
                    {
                        return new DataServiceReply(false, isRegister ? DataServiceReplyType.RegisterFail : DataServiceReplyType.LoginFail, response);
                    }
                }

                // Catch time outs
                catch (OperationCanceledException) when (!cts.Token.IsCancellationRequested)
                {
                    return new DataServiceReply(false, DataServiceReplyType.TimedOut, response);
                }

                // Catch general issues
                catch (Exception e)
                {
                    var msg = "Register/Login process failed with an exception!";
                    Debug.WriteLine(msg);
                    Crashes.TrackError(new Exception(msg, e));
                }
            }

            // Get data from response
            var reply = await response.Content.ReadAsStringAsync();
            var replyData = JsonConvert.DeserializeObject<WebApiReply>(reply);

            // Check the status codes from the message
            var codeAsInt = int.Parse(replyData.Code);
            if (codeAsInt >= 200 && codeAsInt <= 299)
            {
                // Handle 210s differently
                if (codeAsInt == 210)
                {
                    replyData.Message = $"{replyData.Message} Try an alternative such as {replyData.Details}.";
                }
                return new DataServiceReply(false, isRegister ? DataServiceReplyType.RegisterFail : DataServiceReplyType.LoginFail, response, replyData);
            }

            // Code within accepted range so store Ids
            if (!await StoreIdsAsync(replyData.Details))
            {
                replyData.Message += "\nFailed to store id details!";
                return new DataServiceReply(false, isRegister ? DataServiceReplyType.RegisterFail : DataServiceReplyType.LoginFail, response, replyData);
            }

            // Verify these new Ids
            var verifyReply = await VerifyAsync();
            if (!verifyReply.Success) return verifyReply;

            // All done so return success
            return new DataServiceReply(true, DataServiceReplyType.Success, response);
        }

        // Verify the tokens we have stored
        private async Task<DataServiceReply> VerifyAsync()
        {
            if (!InternetAvailable()) return new DataServiceReply(false, DataServiceReplyType.NoInternet, null);

            // Build verification message
            var packet = new VerifyPacket
            {
                DeviceUUID = await GetDeviceIdAsync(),
                userUUID = await GetUserIdAsync()
            };

            // Send verify message
            var response = new HttpResponseMessage();
            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    response = await client.SendAsync(
                        CreateHttpRequestMessage(
                            ServerEndpointType.Verify,
                            packet,
                            new WebApiHttpHeaders("application/json")
                        ),
                        cts.Token
                    );
                    if (!response.IsSuccessStatusCode)
                    {
                        ClearIds();
                        return new DataServiceReply(false, DataServiceReplyType.VerifyFail, response);
                    }
                }

                // Catch time outs
                catch (OperationCanceledException) when (!cts.Token.IsCancellationRequested)
                {
                    ClearIds();
                    return new DataServiceReply(false, DataServiceReplyType.TimedOut, response);
                }

                // Catch general issues
                catch (Exception e)
                {
                    var msg = "Verify process failed with an exception!";
                    Debug.WriteLine(msg);
                    Crashes.TrackError(new Exception(msg, e));
                }
            }

            // Verify has completed successfully
            return new DataServiceReply(true, DataServiceReplyType.Success, response);
        }

        // Check if internet is avaialble
        private bool InternetAvailable()
        {
            return Connectivity.NetworkAccess == NetworkAccess.Internet;
        }

        // Construct an HTTP message
        private HttpRequestMessage CreateHttpRequestMessage(ServerEndpointType msgType, WebApiPacket content, WebApiHttpHeaders headers)
        {
            // Automatically replace null values with empty strings
            var serialiserSettings = new JsonSerializerSettings()
            {
                ContractResolver = new SubstituteNullWithEmptyStringContractResolver()
            };

            // Serialise the content
            string contentString = JsonConvert.SerializeObject(content, serialiserSettings);

            // Construct message
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(ConfigurationService.Instance.GetURL(msgType), UriKind.Relative),
                Method = HttpMethod.Post
            };

            // Add the headers to the request
            if (headers.Accept != null)
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(headers.Accept));

            if (headers.Token != null)
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", headers.Token);

            // Add content to message
            request.Content = new StringContent(contentString, Encoding.UTF8);

            // Add content headers
            if (headers.Content != null)
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(headers.Content);

            return request;
        }
    }
}
