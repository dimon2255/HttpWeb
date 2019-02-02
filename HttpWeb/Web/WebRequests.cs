using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Dna
{
    /// <summary>
    /// Provides HTTP calls for sending and receiving information from HTTP server
    /// </summary>
    public static class WebRequests
    {
        /// <summary>
        /// Posts a web request to the server asynchronously
        /// </summary>
        /// <param name="url">URL where to post at</param>
        /// <param name="content">Data to post</param>
        /// <param name="sendType">The format to serialize the content to</param>
        /// <param name="returnType">The expected format of content to be returned from the server</param>
        /// <returns></returns>
        public static async Task<HttpWebResponse> PostAsync(string url,
                                                            object content = null,
                                                            KnownContentSerializers sendType = KnownContentSerializers.Json,
                                                            KnownContentSerializers returnType = KnownContentSerializers.Json,
                                                            Action<HttpWebRequest> configureRequest = null,
                                                            string bearerToken = null)
        {
            #region Setup Headers

            //Create a request
            var request = WebRequest.CreateHttp(url);

            //Make it s POST request type
            request.Method = HttpMethod.Post.ToString();

            //Set the appropriate return type
            request.Accept = returnType.ToMimeString();

            //Set the content type
            request.ContentType = sendType.ToMimeString();

            //Set up token, if any
            if(bearerToken != null)
            {
                request.Headers.Add("Authorization", $"Bearer {bearerToken}");
            }

            #endregion

            #region Configure HttpWebRequest

            //Add custom configuration to Request object
            configureRequest?.Invoke(request);

            #endregion

            #region Write Content

            //Set the content length
            if (content == null)
            {
                request.ContentLength = 0;
            }
            else
            {
                var contentString = string.Empty;

                if (sendType == KnownContentSerializers.Json)
                {
                    //Convert to JSON format
                    contentString = JsonConvert.SerializeObject(content);
                }
                else if (sendType == KnownContentSerializers.Xml)
                {
                    //Convert to XML format
                    var xmlSerializer = new XmlSerializer(content.GetType());

                    using (var stringWriter = new StringWriterUTF8())
                    {
                        xmlSerializer.Serialize(stringWriter, content);
                        contentString = stringWriter.ToString();
                    }
                }
                else
                {
                    //TODO: Throw error once we have Dna.Framework exception types
                }

                //set the content length
                request.ContentLength = contentString.Length;

                //Get the request stream and write to it
                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    using (var streamWriter = new StreamWriter(requestStream))
                    {
                        await streamWriter.WriteAsync(contentString);
                    }
                }
            }

            #endregion

            return await request.GetResponseAsync() as HttpWebResponse;
        }


        /// <summary>
        /// Posts a web request to the server asynchronously and returns with the of the expected data type
        /// </summary>
        /// <param name="url">URL where to post at</param>
        /// <param name="content">Data to post</param>
        /// <param name="sendType">The format to serialize the content to</param>
        /// <param name="returnType">The expected format of content to be returned from the server</param>
        /// <returns></returns>
        public static async Task<WebRequestResult<TResponse>> PostAsync<TResponse>(string url,
                                                            object content = null,
                                                            KnownContentSerializers sendType = KnownContentSerializers.Json,
                                                            KnownContentSerializers returnType = KnownContentSerializers.Json,
                                                            Action<HttpWebRequest> configureRequest = null,
                                                            string bearerToken = null)
        {
            //Make the standard POST call first
            var serverResponse = await PostAsync(url, content, sendType, returnType, configureRequest, bearerToken);

            //Create a result
            var result = serverResponse.CreateWebRequestResult<TResponse>();

            //If the response is not 200 OK
            if(result.StatusCode != HttpStatusCode.OK)
            {
                //TODO: Localize strings
                result.ErrorMessage = $"Server returned unsuccessful status code {result.StatusCode} => {result.StatusDescription}";

                return result;
            }

            //Deserialize Raw Response
            if (result.RawServerResponse.IsNulOrEmpty())
                return result;

            //Deserialize raw response
            try
            {
                if (!serverResponse.ContentType.ToLower().Contains(returnType.ToMimeString().ToLower()))
                {
                    result.ErrorMessage = $"Server did not return data in expected format. Expected {returnType.ToMimeString()}, received {serverResponse.ContentType}";
                    return result;
                }

                if(returnType == KnownContentSerializers.Json)
                {
                    result.ServerResponse = JsonConvert.DeserializeObject<TResponse>(result.RawServerResponse);
                }
                else if(returnType == KnownContentSerializers.Xml)
                {
                    //Convert to XML format
                    var xmlSerializer = new XmlSerializer(typeof(TResponse));

                    using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(result.RawServerResponse)))
                    {
                        result.ServerResponse = (TResponse)xmlSerializer.Deserialize(memoryStream);
                    }
                }
                else
                {
                    result.ErrorMessage = "Unknown return type, cannot deserialize server response to the expected type";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"Failed to deserialize server response => {ex.Message}";
                return result;
            }

            return result;
        }


        /// <summary>
        /// Issues a GET request to the specified URL, useful for checking INTERNET connection
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="configureRequest"></param>
        /// <param name="bearerToken"></param>
        /// <returns></returns>
        public static async Task<HttpWebResponse> GetAsync(string endpoint, Action<HttpWebRequest> configureRequest = null, string bearerToken = null)
        {
            #region Setup

            // Create the web request
            var request = WebRequest.CreateHttp(endpoint);

            // Make it a GET method
            request.Method = HttpMethod.Get.ToString();

            if (bearerToken != null)
                request.Headers.Add(HttpRequestHeader.Authorization, $"Bearer {bearerToken}");

            // Any custom work
            configureRequest?.Invoke(request);

            #endregion

            // Wrap call...
            try
            {
                // Return the raw server response
                return await request.GetResponseAsync() as HttpWebResponse;
            }
            catch (WebException ex)
            {
                //If we got a response return a response
                if (ex.Response is HttpWebResponse httpWebResponse)
                    return httpWebResponse;

                // Otherwise, we don't have any information to be able to return
                // So re-throw
                throw;
            }
        }
    }
}