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
using OfferService.Exceptions;

namespace OfferService
{
    /// <summary>
    /// Class provides functionality to filter offers
    /// </summary>
    public class OfferFilter
    {
        /// <summary>
        /// Client to make REST calls
        /// </summary>
        private ApiClient apiClient;
        /// <summary>
        /// Settings for the service
        /// </summary>
        private FundaApiSettings apiSettings;
        /// <summary>
        /// Logger to log information
        /// </summary>
        private ILogger<OfferFilter> logger;
        /// <summary>
        /// To report progress back to caller
        /// </summary>
        private IProgress<int> progress;

        /// <summary>
        /// Number of items on each page
        /// </summary>
        private const int PageSize = 10;
        /// <summary>
        /// Number of retries in case of failure
        /// </summary>
        private const int RetryCount = 3;
        /// <summary>
        /// Wait in seconds to overcome exceed limit
        /// </summary>
        private const int ExceedlimitWaitInSec = 15;

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
        public void UpdateProgress(Action<int> updateHandler)
        {
            progress = new Progress<int>(updateHandler);
        }

        /// <summary>
        /// Retrieve the estate agents list and sort them in highest order of sale offer
        /// </summary>
        /// <param name="searchParams">Comma seperated search parameters used to query offers</param>
        /// <returns>A task returning enumerator of EstateAgentInfo model</returns>
        public async Task<IEnumerable<EstateAgentInfo>> GetEstateAgentsByHighestSaleOrder(params string[] searchParams)
        {
            int retryCount = RetryCount;
            int pageIndex = 0, totalPageIndexes = 0;
            OfferModel offerModel;
            List<OfferObjectModel> offers = new List<OfferObjectModel>();

            do
            {
                pageIndex++; // Target the page to fetch
                var templateMap = TemplateMap(pageIndex, searchParams);
                string relativeUri = Regex.Replace(apiSettings.OfferUriTemplate, "{[^{}]+}", x => GetReplacement(x, templateMap));

                logger.LogDebug($"Hitting URL: {apiClient.Client?.BaseAddress}{relativeUri}");

                offerModel = await apiClient.GetData<OfferModel>(relativeUri);

                if (offerModel == null)
                {
                    if (--retryCount < 0)
                    {
                        throw new NotCompletedException("Reached maximum retries", logger);
                    }

                    logger.LogInformation("Waiting to overcome exceed limit");
                    await Task.Delay(new TimeSpan(hours: 0, minutes: 0, seconds: ExceedlimitWaitInSec));
                    pageIndex--; // Adjust the target to re-fetch 
                    continue;
                }

                retryCount = RetryCount; // Rest retry count
                totalPageIndexes = offerModel.Paging.AantalPaginas;
                progress?.Report(CalculateProgress(offerModel.Paging.HuidigePagina, offerModel.Paging.AantalPaginas));

                if (offerModel.Objects != null)
                    offers.AddRange(offerModel.Objects);

            } while (pageIndex < totalPageIndexes);

            return offers.GroupBy(x => x.MakelaarNaam)
                         .OrderByDescending(x => x.Count())
                         .Select(x => new EstateAgentInfo { Name = x.Key, OfferCount = x.Count() });
        }

        /// <summary>
        /// Map used to prepare actual relative uri
        /// </summary>
        /// <param name="pageIndex">Target page to fetch information</param>
        /// <param name="searchParams">Query parameters used to search</param>
        /// <returns>Key/value pair</returns>
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

        /// <summary>
        /// Get replacement value from template
        /// </summary>
        /// <param name="match">Contains the key from the template</param>
        /// <param name="templateMap">The template map</param>
        /// <returns>The value to replace</returns>
        private string GetReplacement(Match match, Dictionary<string, string> templateMap)
        {
            templateMap.TryGetValue(match.Value, out string replacement);

            return replacement;
        }

        /// <summary>
        /// Calculate the progress in percentage
        /// </summary>
        /// <param name="current">The current value</param>
        /// <param name="total">The total value</param>
        /// <returns>Current value presented as percentage</returns>
        private int CalculateProgress(int current, int total) =>
            (int) Math.Ceiling((double)(current * 100) / total);
    }
}
