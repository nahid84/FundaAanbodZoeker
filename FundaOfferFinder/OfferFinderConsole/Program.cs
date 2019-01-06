using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

        private static IConfiguration CreateConfiguration => 
            new ConfigurationBuilder().AddJsonFile(SettingsFileName, optional: false, reloadOnChange: true)
                                      .Build();

        private static IServiceProvider RegisterServices(IConfiguration configuration) =>
            new ServiceCollection().AddOptions()
                                   .Configure<FundaApiSettings>(configuration.GetSection(nameof(FundaApiSettings)))
                                   .AddSingleton<ApiClient, JsonApiClient>()
                                   .AddTransient<OfferFilter>()
                                   .AddLogging(x=> x.AddConsole())
                                   .BuildServiceProvider();


        internal static void Main(string[] args)
        {
            IServiceProvider serviceProvider = RegisterServices(CreateConfiguration);

            ILogger logger = serviceProvider.GetService<ILogger<Program>>();
            OfferFilter offerFilter = serviceProvider.GetService<OfferFilter>();

            string regionName = "amsterdam";
            logger.LogInformation($"Started with region {regionName}, Fetching...");

            var agents = offerFilter.GetEstateAgentsByHighestSaleOrder(regionName).Result;

            int totalCount = 0;
            foreach (EstateAgentInfo agent in agents)
            {
                totalCount += agent.OfferCount;
                Console.WriteLine($"{agent.Name} has {agent.OfferCount} items in sale");
            }

            Console.WriteLine($"Total {totalCount} items for sale in {regionName}");

            Console.ReadKey();
        }
    }
}
