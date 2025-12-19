using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace GrayWolf.Interfaces
{
    public interface IApiService
    {
        /// <summary>
        /// Send GET request
        /// </summary>
        /// <typeparam name="T">Expected response type</typeparam>
        /// <param name="path">Part of url</param>
        /// <param name="collection">Query parameters</param>
        /// <param name="cToken">Request cancellation token</param>
        /// <param name="headers">Request headers</param>
        /// <returns>Instance of T if success, throws exception otherwise</returns>
        Task<T> GetAsync<T>(string path, NameValueCollection collection = null, CancellationToken cToken = default, List<KeyValuePair<string, string>> headers = null, Func<string, T> deserializeAction = null);

        /// <summary>
        /// Sends POST request
        /// </summary>
        /// <typeparam name="T">Expected response type</typeparam>
        /// <param name="path">Part of url</param>
        /// <param name="obj">Request parameter</param>
        /// <param name="cToken">Request cancellation token</param>
        /// <param name="headers">Request headers</param>
        /// <returns>Instance of T if success, throws exception otherwise</returns>
        Task<T> PostAsync<T>(string path, object obj = null, CancellationToken cToken = default, List<KeyValuePair<string, string>> headers = null, Func<string, T> deserializeAction = null);

        /// <summary>
        /// Sends POST request
        /// </summary>
        /// <typeparam name="T">Expected response type</typeparam>
        /// <param name="path">Part of url</param>
        /// <param name="arguments">Request data</param>
        /// <param name="cToken">Request cancellation token</param>
        /// <param name="headers">Request headers</param>
        /// <returns>Instance of T if success, throws exception otherwise</returns>
        Task<T> PostFormDataAsync<T>(string path, Dictionary<string, string> arguments, CancellationToken cToken = default, List<KeyValuePair<string, string>> headers = null, Func<string, T> deserializeAction = null);

        /// <summary>
        /// Sends PUT request
        /// </summary>
        /// <typeparam name="T">Expected response type</typeparam>
        /// <param name="path">Part of url</param>
        /// <param name="obj">Request parameter</param>
        /// <param name="cToken">Request cancellation token</param>
        /// <param name="headers">Request headers</param>
        /// <returns>Instance of T if successful, throws exception otherwise</returns>
        Task<T> PutAsync<T>(string path, object obj = null, CancellationToken cToken = default, List<KeyValuePair<string, string>> headers = null, Func<string, T> deserializeAction = null);

        /// <summary>
        /// Sends DELETE request
        /// </summary>
        /// <typeparam name="T">Expected response type</typeparam>
        /// <param name="path">Part of url</param>
        /// <param name="obj">Request parameter</param>
        /// <param name="cToken">Request cancellation token</param>
        /// <param name="headers">Request headers</param>
        /// <returns>Instance of T if success, throws exception otherwise</returns>
        Task<T> DeleteAsync<T>(string path, object obj = null, CancellationToken cToken = default, List<KeyValuePair<string, string>> headers = null, Func<string, T> deserializeAction = null);

        /// <summary>
        /// Sends UPLOAD request
        /// </summary>
        /// <typeparam name="T">Expected response type</typeparam>
        /// <param name="path">Part of url</param>
        /// <param name="filePath">Path to file that will be uploaded</param>
        /// <param name="obj">Request parameter</param>
        /// <param name="cToken">Request cancellation token</param>
        /// <param name="headers">Request headers</param>
        /// <returns>Instance of T if success, throws exception otherwise</returns>
        Task<T> UploadAsync<T>(string path, string filePath, string fileName, object obj = null, string httpMethod = "", CancellationToken cToken = default, List<KeyValuePair<string, string>> headers = null, Func<string, T> deserializeAction = null);
    }
}
