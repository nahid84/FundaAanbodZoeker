using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OfferFinderConsole.Extensions;
using OfferService;
using OfferService.Client;
using OfferService.Exceptions;
using OfferService.Settings;
using Serilog;
using System;
using System.Collections;
using System.Linq;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace OfferFinderConsole
{
    /// <summary>
    /// Runs the service and prints data into console including usage
    /// </summary>
    internal class ServiceRunner
    {
        /// <summary>
        /// Application settings file name
        /// </summary>
        private const string SettingsFileName = "appsettings.json";
        /// <summary>
        /// Row number in the console to start printing table
        /// </summary>
        private const int TableStart = 4;

        /// <summary>
        /// Create the configuration for the application
        /// </summary>
        /// <returns>Key/value application configuration properties</returns>
        private static IConfiguration CreateConfiguration() =>
            new ConfigurationBuilder().AddJsonFile(SettingsFileName, optional: false, reloadOnChange: true)
                                      .Build();

        /// <summary>
        /// Register the services for dependency injection
        /// </summary>
        /// <param name="configuration">Key/value application configuration properties</param>
        /// <returns>Mechanism for retrieving a service object</returns>
        private static IServiceProvider RegisterServices(IConfiguration configuration) =>
            new ServiceCollection().AddOptions()
                                   .Configure<FundaApiSettings>(configuration.GetSection(nameof(FundaApiSettings)))
                                   .AddSingleton<ApiClient, JsonApiClient>()
                                   .AddTransient<OfferFilter>()
                                   .AddLogging(x => x.AddSerilog())
                                   .BuildServiceProvider();

        /// <summary>
        /// Prints progress data into console
        /// </summary>
        /// <param name="progressData">Percentage of the progress</param>
        private static void PrintProgress(int progressData)
        {
            Console.Write("\rProgress = {0,3}%", progressData);
        }

        /// <summary>
        /// Prints usage data into console
        /// </summary>
        internal static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("dotnet OfferFinderConsole.dll searchParam1, searchParam2,... [topResults]");
        }

        /// <summary>
        /// Print agents having most offers into console
        /// </summary>
        /// <param name="args"></param>
        /// <param name="numberOfItemsToShow"></param>
        internal static void PrintData(string[] args, int numberOfItemsToShow)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File($"{Environment.CurrentDirectory}\\Logs\\log-{DateTime.Now.ToString("dd.MM.yyyy-HH.mm.ss")}.txt")
                .CreateLogger();

            IServiceProvider serviceProvider = RegisterServices(CreateConfiguration());

            ILogger logger = serviceProvider.GetService<ILogger<Program>>();
            OfferFilter offerFilter = serviceProvider.GetService<OfferFilter>();

            "Funda offer finder started..."
                .SendTo(Console.WriteLine);
            $"Searching offers by {string.Join(',', args)}"
                .SendTo(Console.WriteLine)
                .SendTo(objects: null, method: logger.LogInformation);
            $"Fetching Top {numberOfItemsToShow} selling agents, Please wait..."
                .SendTo(Console.WriteLine);

            string[] headers = new string[] { "Estate Agent Name", "Offer Count" };
            ConsoleUI consoleUI = new ConsoleUI(TableStart, ConsoleUI.Align.Left, headers);

            offerFilter.UpdateProgress(PrintProgress);

            try
            {
                var topAgents = offerFilter.GetEstateAgentsByHighestSaleOrder(args)
                                        .GetAwaiter()
                                        .GetResult()
                                        .Take(numberOfItemsToShow);

                ArrayList tableData = new ArrayList(topAgents.Select(x => new string[] { x.Name, x.OfferCount.ToString() }).ToList());
                consoleUI.RePrint(tableData);
            }
            catch (NotCompletedException<OfferFilter> ncEx)
            {
                Console.WriteLine("\r\nAn error occured, please try again later");
                Console.WriteLine($"Reason: {ncEx.Message}");
            }
        }
    }
}
