using Microsoft.Extensions.Options;
using OfferService.Settings;
using System;
using System.Net.Http;

namespace OfferService.Client
{
    /// <summary>
    /// Client to deal with Json messages
    /// </summary>
    public class JsonApiClient : ApiClient
    {
        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="settings">Settings for the client</param>
        public JsonApiClient(IOptions<FundaApiSettings> settings)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(settings.Value.BaseUri);
        }
    }
}
