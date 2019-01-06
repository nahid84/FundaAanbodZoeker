using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models;
using OfferService;
using OfferService.Client;
using OfferService.Settings;
using System;

namespace OfferFinderConsole
{
    internal class Program
    {
        private const string SettingsFileName = "appsettings.json";

        internal static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile(SettingsFileName, optional: false, reloadOnChange: true)
                .Build();

            IServiceProvider serviceProvider = new ServiceCollection()
                .AddOptions()
                .Configure<FundaApiSettings>(configuration.GetSection(nameof(FundaApiSettings)))
                .AddSingleton<ApiClient, JsonApiClient>()
                .AddTransient<OfferFilter>()
                .BuildServiceProvider();

            OfferFilter offerFilter = serviceProvider.GetService<OfferFilter>();

            string regionName = "amsterdam";
            var agents = offerFilter.GetEstateAgentsByHighestSaleOrder(regionName).Result;

            int totalCount = 0;
            foreach (EstateAgentInfo agent in agents)
            {
                totalCount += agent.OfferCount;
                Console.WriteLine($"{agent.Name} has {agent.OfferCount} items in sale");
            }

            Console.WriteLine($"Total {totalCount} items in sale for {regionName}");

            Console.ReadKey();
        }
    }
}
