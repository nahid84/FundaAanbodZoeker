using Microsoft.Extensions.Options;
using OfferService.Settings;
using System;
using System.Net.Http;

namespace OfferService.Client
{
    public class JsonApiClient : ApiClient
    {
        public JsonApiClient(IOptions<FundaApiSettings> settings)
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri(settings.Value.BaseUri);
        }

        public JsonApiClient(HttpClient httpClient, IOptions<FundaApiSettings> settings)
        {
            Client = httpClient;
            Client.BaseAddress = new Uri(settings.Value.BaseUri);
        }
    }
}
