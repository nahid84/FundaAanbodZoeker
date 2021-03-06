﻿using Microsoft.Extensions.Options;
using OfferService.Settings;

namespace OfferService.IntegrationTests
{
    public class FundaApiSettingsFixture : IOptions<FundaApiSettings>
    {
        public FundaApiSettings Value =>
            new FundaApiSettings
            {
                BaseUri = "http://partnerapi.funda.nl",
                Key = "ac1b0b1572524640a0ecc54de453ea9f",
                OfferUriTemplate = "feeds/Aanbod.svc/json/{key}?type=koop&zo={searchQuery}&page={pageIndex}&pagesize={pageSize}"
            };
    }
}
