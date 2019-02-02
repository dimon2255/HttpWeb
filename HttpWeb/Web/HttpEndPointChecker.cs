using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Dna.Web
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpEndPointChecker : IDisposable
    {
        #region Protected Members

        /// <summary>
        /// Callback to use if provided by the caller
        /// </summary>
        protected Action<bool> mStateChangedCallback;

        /// <summary>
        /// Flag to poll for cancellation
        /// </summary>
        protected bool mDisposing;

        #endregion

        #region Public Properties

        /// <summary>
        /// Endpoint to check for connection
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Indicates if the endpoint is responsive
        /// </summary>
        public bool Responsive { get; set; }

        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="interval"></param>
        /// <param name="stateChangedCallback"></param>
        /// <param name="validResponseParser"></param>
        /// <param name="logger"></param>
        public HttpEndPointChecker(string endpoint, 
                                   int interval, 
                                   Action<bool> stateChangedCallback, 
                                   Func<HttpWebResponse, Exception, bool> validResponseParser = null, 
                                   ILogger logger = null)
        {
            //Set the endpoint
            Endpoint = endpoint;

            //Store callback
            mStateChangedCallback = stateChangedCallback;

            logger?.LogTraceSource($"HttpEndpointChacker started for {endpoint}");

            // Start task
            Task.Run(async () =>
            {
                while (!mDisposing)
                {
                    //Create defaults
                    var webResponse = default(HttpWebResponse);
                    var exception = default(Exception);

                    // Start by calling the endpoint

                    try
                    {
                        //Log it
                        logger?.LogTraceSource($"HttpEndpointChecker fetching {endpoint}");

                        //
                        // By default, presume any response that doesn't throw
                        // (so the server replied, even if its 401 for example
                        // meaning the server we hit up actually responded
                        // with something even if it was a page not found or server
                        // error.
                        //
                        // The user is free to override this default behavior
                        //
                        webResponse = await WebRequests.GetAsync(endpoint);

                    }
                    catch (Exception ex)
                    {
                        exception = ex;                        
                    }

                    // Figure out the new state
                    // - If we have a custom parser, ask it for the state based on the response
                    // - Otherwise, so long as we have a response of any kind, it's valid
                    var responsive = validResponseParser?.Invoke(webResponse, exception) ?? webResponse != null;

                    //Close web response
                    webResponse?.Close();

                    //Log it
                    logger.LogTraceSource($"HttpEndpointChecker {endpoint} { (responsive ? "is" : "is not")} responsive");

                    // If the state has changed...
                    if (responsive != Responsive)
                    {
                        // Set new value
                        Responsive = responsive;

                        // Inform listener
                        mStateChangedCallback?.Invoke(responsive);
                    }

                    // if not disposing, wait interval.. then poll again
                    if (!mDisposing)
                        await Task.Delay(interval);
                }
            });
        }

        /// <summary>
        /// Default Destructor
        /// </summary>
        ~HttpEndPointChecker()
        {
            mDisposing = true;
        }    

        #region IDisposable

        /// <summary>
        /// Dispose of the resources
        /// </summary>
        public void Dispose()
        {
            mDisposing = true;
        }

        #endregion
    }
}
