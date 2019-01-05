using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace OfferService.Client
{
    public class JsonApiClient : ApiClient
    {
        public JsonApiClient()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("");
        }
    }
}
