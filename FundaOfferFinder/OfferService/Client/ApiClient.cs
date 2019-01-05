using System.Net.Http;
using System.Threading.Tasks;

namespace OfferService.Client
{
    public abstract class ApiClient
    {
        public HttpClient Client { get; protected set; }

        public async Task<TReturn> GetData<TReturn>(string requestUri)
        {
            var response = await Client.GetAsync(requestUri);
            return await response.Content.ReadAsAsync<TReturn>();
        }
    }
}
