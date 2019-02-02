using System.Net;

namespace Dna
{
    /// <summary>
    /// WebResponse from a call to get specific data from HTTP server
    /// </summary>
    public class WebRequestResult
    {
        #region Public Properties

        /// <summary>
        /// Indicates whether the call was successful or not
        /// </summary>
        public bool Successful => ErrorMessage == null;

        /// <summary>
        /// If something fails, this is the error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The status code
        /// </summary>
        public HttpStatusCode StatusCode{ get; set; }

        /// <summary>
        /// The status description
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// The type of content returned by the server
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// All the response headers 
        /// </summary>
        public WebHeaderCollection Headers { get; set; }

        /// <summary>
        /// Any cookies sent in the response
        /// </summary>
        public CookieCollection Cookies { get; set; }

        /// <summary>
        /// The raw server response body
        /// </summary>
        public string RawServerResponse { get; set; }

        /// <summary>
        /// The actual server response as an object
        /// </summary>
        public object ServerResponse { get; set; }

        #endregion
    }

    /// <summary>
    /// WebResponse from a call to get generic data from HTTP server
    /// </summary>
    /// <typeparam name="T">Type of data to deserialize the raw body into</typeparam>
    public class WebRequestResult<T> : WebRequestResult
    {
        /// <summary>
        /// Casts the specified object to the specified type
        /// </summary>
        public new T ServerResponse { get; set; }
    }
}
