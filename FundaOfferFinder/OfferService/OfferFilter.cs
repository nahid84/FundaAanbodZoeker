using Microsoft.Extensions.Options;
using OfferService.Client;
using OfferService.Models;
using OfferService.Settings;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using Models;
using System.Diagnostics;
using System.Threading;
using System;

namespace OfferService
{
    public class OfferFilter
    {
        private ApiClient apiClient;
        private FundaApiSettings apiSettings;

        private const int PageSize = 25;
        private const int ThrottlingValue = 100; // Request per minute

        public OfferFilter(IOptions<FundaApiSettings> settings, ApiClient apiClient)
        {
            this.apiClient = apiClient;
            this.apiSettings = settings.Value;
        }

        private Dictionary<string,string> TemplateMap(int pageIndex, params string[] searchParams)
        {
            return new Dictionary<string, string>
            {
                { "{baseUri}", apiSettings.BaseUri },
                { "{key}" , apiSettings.Key },
                { "{searchQuery}", $"/{string.Join("/", searchParams)}" },
                { "{pageIndex}", pageIndex.ToString() },
                { "{pageSize}", PageSize.ToString() }
            };
        }

        private string GetReplacement(Match match, Dictionary<string, string> templateMap)
        {
            templateMap.TryGetValue(match.Value, out string replacement);

            return replacement;
        }
        public async Task<IEnumerable<EstateAgentInfo>> GetEstateAgentsByHighestSaleOrder(params string[] searchParams)
        {
            List<OfferObjectModel> offers = new List<OfferObjectModel>();

            int pageIndex = 1;
            OfferModel offerModel;

            Stopwatch stopWatch = Stopwatch.StartNew();
            
            do
            {
                var templateMap = TemplateMap(pageIndex, searchParams);
                string relativeUri = Regex.Replace(apiSettings.OfferUriTemplate, "{[^{}]+}", x => GetReplacement(x, templateMap));
                offerModel = await apiClient.GetData<OfferModel>(relativeUri);
                offers.AddRange(offerModel.Objects);

                if (stopWatch.Elapsed <= TimeSpan.FromSeconds(60))
                {
                    bool moreThanThreshold = (pageIndex / ThrottlingValue) >= 1;

                    if (moreThanThreshold)
                        Thread.Sleep(15000);
                }
                else
                    stopWatch.Restart();

            } while (pageIndex++ < offerModel.Paging.AantalPaginas);

            stopWatch.Stop();

            return offers.GroupBy(x => x.MakelaarNaam)
                         .OrderByDescending(x => x.Count())
                         .Select(x => new EstateAgentInfo { Name = x.Key, OfferCount = x.Count() });
        }
    }
}
