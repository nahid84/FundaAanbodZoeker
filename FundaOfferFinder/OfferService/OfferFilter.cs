using OfferService.Client;
using System;

namespace OfferService
{
    public class OfferFilter
    {
        private ApiClient apiClient;
        public OfferFilter(ApiClient apiClient)
        {
            this.apiClient = apiClient;
        }
    }
}
