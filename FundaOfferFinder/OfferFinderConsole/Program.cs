using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Models;
using OfferService;
using OfferService.Client;
using OfferService.Settings;
using Serilog;
using System;
using System.Collections;
using System.Linq;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace OfferFinderConsole
{
    internal class Program
    {
        private const string SettingsFileName = "appsettings.json";
        private const int TableStart = 3;
        private const int NumberOfDefaultItemsToShow = 10;

        private static IConfiguration CreateConfiguration => 
            new ConfigurationBuilder().AddJsonFile(SettingsFileName, optional: false, reloadOnChange: true)
                                      .Build();

        private static IServiceProvider RegisterServices(IConfiguration configuration) =>
            new ServiceCollection().AddOptions()
                                   .Configure<FundaApiSettings>(configuration.GetSection(nameof(FundaApiSettings)))
                                   .AddSingleton<ApiClient, JsonApiClient>()
                                   .AddTransient<OfferFilter>()
                                   .AddLogging(x=> x.AddSerilog())
                                   .BuildServiceProvider();


        internal static void Main(string[] args)
        {
            if(args.Count() < 1)
            {
                PrintUsage();

            } else
            {
                PrintData(args[0].Split(','), string.IsNullOrEmpty(args[1]) ? NumberOfDefaultItemsToShow : int.Parse(args[1]));
            }

            Console.WriteLine("Press enter to exit...");
            Console.ReadKey();
        } 

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("dotnet OfferFinderConsole.dll searchParam1, searchParam2,... [topResults]");
        }

        private static void PrintData(string[] args, int numberOfItemsToShow)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File($"{Environment.CurrentDirectory}\\Logs\\log-{DateTime.Now.ToString("dd.MM.yyyy-HH.mm.ss")}.txt")
                .CreateLogger();

            IServiceProvider serviceProvider = RegisterServices(CreateConfiguration);

            ILogger logger = serviceProvider.GetService<ILogger<Program>>();
            OfferFilter offerFilter = serviceProvider.GetService<OfferFilter>();

            "Funda offer finder started..."
                .SendTo(Console.WriteLine);
            $"Searching offers by {string.Join(',', args)}"
                .SendTo(Console.WriteLine)
                .SendTo(objects: null, method: logger.LogInformation);
            $"Fetching Top {numberOfItemsToShow} selling agents, Please wait...".SendTo(Console.WriteLine);

            string[] headers = new string[] { "Estate Agent Name", "Offer Count" };
            ConsoleUI consoleUI = new ConsoleUI(TableStart, ConsoleUI.Align.Left, headers);

            var agents = offerFilter.GetEstateAgentsByHighestSaleOrder(args).Result.Take(numberOfItemsToShow);

            ArrayList tableData = new ArrayList(agents.Select(x => new string[] { x.Name, x.OfferCount.ToString() }).ToList());
            consoleUI.RePrint(tableData);
        }
    }
}
