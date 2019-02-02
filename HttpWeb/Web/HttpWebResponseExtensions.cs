using System.IO;
using System.Net;

namespace Dna
{
    /// <summary>
    /// Extension methods for <see cref="HttpWebResponse>"/>
    /// </summary>
    public static class HttpWebResponseExtensions
    {
        /// <summary>
        /// Returns a <see cref="WebRequestResult{T}"/> pre-populated with the <see cref="HttpWebResponse"/> information
        /// </summary>
        /// <typeparam name="TResponse">Type of response to create </typeparam>
        /// <param name="serverResponse">The server response</param>
        /// <returns></returns>
        public static WebRequestResult<TResponse> CreateWebRequestResult<TResponse>(this HttpWebResponse serverResponse)
        {
            var result = new WebRequestResult<TResponse>()
            {
                ContentType = serverResponse.ContentType,
                Headers = serverResponse.Headers,
                Cookies = serverResponse.Cookies,
                StatusCode = serverResponse.StatusCode,
                StatusDescription = serverResponse.StatusDescription,
            };

            //If we got a successful response...
            if (result.StatusCode == HttpStatusCode.OK)
            {
                //Read in the response body
                using(var responseStream = serverResponse.GetResponseStream())
                {
                    using(var streamReader = new StreamReader(responseStream))
                    {
                        result.RawServerResponse = streamReader.ReadToEnd();
                    }
                }
            }

            return result;
        }
    }
}
