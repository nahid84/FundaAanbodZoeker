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
using Microsoft.Extensions.Logging;

namespace OfferService
{
    /// <summary>
    /// Class provides functionality to filter offers
    /// </summary>
    public class OfferFilter
    {
        private ApiClient apiClient;
        private FundaApiSettings apiSettings;
        private ILogger<OfferFilter> logger;
        private IProgress<ProgressModel> progress;

        /// <summary>
        /// Number of items on each page
        /// </summary>
        private const int PageSize = 25;
        /// <summary>
        /// Request per minute
        /// </summary>
        private const int ThrottlingValue = 100;

        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="settings">Settings for this filter to run</param>
        /// <param name="apiClient">Client having functions to make REST calls</param>
        /// <param name="logger">Logger to log information</param>
        public OfferFilter(IOptions<FundaApiSettings> settings, ApiClient apiClient, ILogger<OfferFilter> logger)
        {
            this.apiClient = apiClient;
            apiSettings = settings.Value;
            this.logger = logger;
        }

        /// <summary>
        /// Updating progress of the operation
        /// </summary>
        /// <param name="updateHandler"></param>
        public void UpdateProgress(Action<ProgressModel> updateHandler)
        {
            progress = new Progress<ProgressModel>(updateHandler);
        }

        /// <summary>
        /// Retrieve the estate agents list and sort them in highest order of sale offer
        /// </summary>
        /// <param name="searchParams">Comma seperated search parameters used to query offers</param>
        /// <returns>A task returning enumerator of EstateAgentInfo model</returns>
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

                logger.LogDebug($"Hitting URL: {apiClient.Client?.BaseAddress}{relativeUri}");

                offerModel = await apiClient.GetData<OfferModel>(relativeUri);

                progress?.Report(new ProgressModel
                {
                    Total = offerModel.Paging.AantalPaginas,
                    Current = offerModel.Paging.HuidigePagina
                });

                if (offerModel?.Objects != null)
                    offers.AddRange(offerModel.Objects);

                if (stopWatch.Elapsed <= TimeSpan.FromSeconds(60))
                {
                    bool moreThanThreshold = (pageIndex / ThrottlingValue) >= 1;

                    if (moreThanThreshold)
                        Thread.Sleep(15000);
                }
                else
                    stopWatch.Restart();

            } while (pageIndex++ < offerModel?.Paging?.AantalPaginas);

            stopWatch.Stop();

            return offers.GroupBy(x => x.MakelaarNaam)
                         .OrderByDescending(x => x.Count())
                         .Select(x => new EstateAgentInfo { Name = x.Key, OfferCount = x.Count() });
        }

        private Dictionary<string, string> TemplateMap(int pageIndex, params string[] searchParams)
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
    }
}
