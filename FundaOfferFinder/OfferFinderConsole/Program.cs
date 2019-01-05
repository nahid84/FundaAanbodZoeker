using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OfferService;
using OfferService.Client;
using OfferService.Settings;
using System;

namespace OfferFinderConsole
{
    internal class Program
    {
        private static string SettingsFileName = "appsettings.json";

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

            Console.WriteLine("All Set!!!");
        }
    }
}
