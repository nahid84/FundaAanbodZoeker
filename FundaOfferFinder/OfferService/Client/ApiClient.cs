using System.Net.Http;
using System.Threading.Tasks;

namespace OfferService.Client
{
    /// <summary>
    /// Api client class
    /// </summary>
    public abstract class ApiClient
    {
        /// <summary>
        /// Http client to be used for REST calls
        /// </summary>
        public HttpClient Client { get; protected set; }

        /// <summary>
        /// Get the data by doing REST get call
        /// </summary>
        /// <typeparam name="TReturn">The type of object to return</typeparam>
        /// <param name="requestUri">Request URI</param>
        /// <returns>Return the object of type TReturn</returns>
        public virtual async Task<TReturn> GetData<TReturn>(string requestUri)
        {
            var response = await Client.GetAsync(requestUri);
            return await response.Content.ReadAsAsync<TReturn>();
        }
    }
}
