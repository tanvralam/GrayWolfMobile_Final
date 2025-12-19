using CommunityToolkit.Mvvm.DependencyInjection;
using GrayWolf.Extensions;
using GrayWolf.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GrayWolf.Services
{
    public class ApiService : IApiService
    {
        private const string TAG = "ApiService";

        public static IApiService Instance => Ioc.Default.GetService<IApiService>();

        public static GrayWolf.Interfaces.IFileSystem FileSystem => Ioc.Default.GetService<GrayWolf.Interfaces.IFileSystem>();

        public HttpClient HttpClient { get; }

        public IAnalyticsService AnalyticsService { get; }

        public ApiService(HttpClient client)
        {
            HttpClient = client;
            AnalyticsService = Ioc.Default.GetService<IAnalyticsService>();
        }

        #region implementation
        public Task<T> GetAsync<T>(string path, NameValueCollection collection = null, CancellationToken cToken = default, List<KeyValuePair<string, string>> headers = null, Func<string, T> deserializeAction = null)
        {
            //collection contains query parameters that needs to be added to path in valid format
            path += collection?.ToQuery();

            var message = new HttpRequestMessage(HttpMethod.Get, path);
            return SendAsync<T>(message, cToken, headers, deserializeAction);
        }

        public Task<T> PostAsync<T>(string path, object obj = null, CancellationToken cToken = default, List<KeyValuePair<string, string>> headers = null, Func<string, T> deserializeAction = null) 
        {
            var message = new HttpRequestMessage(HttpMethod.Post, path)
            {
                //CreateStringContent will serialize object into json string
                Content = CreateStringContent(obj)
            };
            return SendAsync<T>(message, cToken, headers, deserializeAction);
        }

        public Task<T> PostFormDataAsync<T>(string path, Dictionary<string, string> arguments, CancellationToken cToken = default, List<KeyValuePair<string, string>> headers = null, Func<string, T> deserializeAction = null)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = CreateMultiPartFormDataContent(arguments)
            };
            return SendAsync<T>(message, cToken, headers, deserializeAction);
        }

        public Task<T> PutAsync<T>(string path, object obj = null, CancellationToken cToken = default, List<KeyValuePair<string, string>> headers = null, Func<string, T> deserializeAction = null) 
        {
            var message = new HttpRequestMessage(HttpMethod.Put, path)
            {
                //CreateStringContent will serialize object into json string
                Content = CreateStringContent(obj)
            };
            return SendAsync<T>(message, cToken, headers, deserializeAction);
        }


        public Task<T> DeleteAsync<T>(string path, object obj = null, CancellationToken cToken = default, List<KeyValuePair<string, string>> headers = null, Func<string, T> deserializeAction = null)
        {
            var message = new HttpRequestMessage(HttpMethod.Delete, path)
            {
                //CreateStringContent will serialize object into json string
                Content = CreateStringContent(obj)
            };
            return SendAsync<T>(message, cToken, headers, deserializeAction);
        }

        public async Task<T> UploadAsync<T>(string path, string filePath, string fileName, object obj = null, string httpMethod = "", CancellationToken cToken = default, List<KeyValuePair<string, string>> headers = null, Func<string, T> deserializeAction = null) 
        {
            //Modifying path if object is collection of parameters
            if (obj is NameValueCollection nvc)
                path += nvc.ToQuery();

            var message = new HttpRequestMessage(string.IsNullOrWhiteSpace(httpMethod) ? HttpMethod.Post : new HttpMethod(httpMethod), path)
            {
                Content = new MultipartFormDataContent
                {
                    //CreateStreamContent will create content for specified file
                    await CreateStreamContentAsync(filePath, fileName)
                }
            };
            return await SendAsync<T>(message, cToken, headers, deserializeAction);
        }

        public void SetAuthToken(string token)
        {
            //Changing authorization token for http requests
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        #endregion

        #region internal methods
        /// <summary>
        /// Creates StringContent from object
        /// </summary>
        private StringContent CreateStringContent(object obj = null)
        {
            var json = JsonConvert.SerializeObject(obj);
            return new StringContent(
                json,
                Encoding.UTF8,
                "application/json");
        }

        private async Task<StreamContent> CreateStreamContentAsync(string filePath, string name)
        {
            var bytes = await FileSystem.ReadAllBytesAsync(filePath);
            var stream = new MemoryStream(bytes);
            var content = new StreamContent(stream);
            var fileName = Path.GetFileName(filePath);
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue(name)
            {
                FileName = fileName
            };
            return content;
        }

        private MultipartFormDataContent CreateMultiPartFormDataContent(Dictionary<string, string> arguments)
        {
            var result = new MultipartFormDataContent();
            foreach(var entry in arguments)
            {
                result.Add(new StringContent(entry.Value), entry.Key);
            }
            return result;
        }

        /// <summary>
        /// Sends HTTP request
        /// </summary>
        /// <typeparam name="T">Expected response type</typeparam>
        /// <param name="message">Message with Content, HttpMethod and Url</param>
        /// <param name="cToken">Request cancellation token</param>
        /// <param name="headers"></param>
        /// <returns>Instance of T if success, throws exception otherwise</returns>
        private async Task<T> SendAsync<T>(HttpRequestMessage message, CancellationToken cToken, List<KeyValuePair<string, string>> headers, Func<string, T> deserializeAction)
        {
            var resultContent = "";
            try
            {
                //Sending request
                var response = await GetHttpResponseAsync(message, cToken, headers);

                resultContent = await response.Content.ReadAsStringAsync();

                //Check if response contains UnAuthorized header and throw exception which indicates it
                CheckAuthorization(response);

                //Some responses return strings instead of object
                //So we need deserialize content as string first
                return deserializeAction == null ? DeserializeResponseString<T>(resultContent) : deserializeAction.Invoke(resultContent);
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, TAG, new Dictionary<string, object>
                {
                    { "Url", $"{message.RequestUri}" },
                    { "HttpMethod", $"{message.Method}" }
                });
                throw ex;
            }
        }

        private Task<HttpResponseMessage> GetHttpResponseAsync(HttpRequestMessage message, CancellationToken cToken = default, List<KeyValuePair<string, string>> headers = null)
        {
            //cToken sets timeout for request
            if (cToken == default)
            {
                //TODO add timeout
                cToken = new CancellationTokenSource().Token;
            }

            //setting headers for message if they're not null
            headers?.ForEach(x => message.Headers.Add(x.Key, x.Value));
            return Task.Run(async () => await HttpClient.SendAsync(message, cToken));
        }

        private T DeserializeResponseString<T>(string resultContent)
        {
            try
            {
                resultContent = JsonConvert.DeserializeObject<string>(resultContent);
            }
            catch (Exception) { }
            return string.IsNullOrWhiteSpace(resultContent) ? default : JsonConvert.DeserializeObject<T>(resultContent, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            });
        }

        private void CheckAuthorization(HttpResponseMessage response)
        {
            
        }
        #endregion
    }
}
