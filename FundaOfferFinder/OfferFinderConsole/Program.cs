using System;
using System.Linq;

namespace OfferFinderConsole
{
    /// <summary>
    /// The application startup class
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Number of items to display in the table
        /// </summary>
        private const int NumberOfDefaultItemsToShow = 10;

        /// <summary>
        /// The application entry point
        /// </summary>
        /// <param name="args">Arguments passed to the program</param>
        internal static void Main(string[] args)
        {
            if(args.Count() < 1)
            {
                ServiceRunner.PrintUsage();

            } else
            {
                ServiceRunner.PrintData(args[0].Split(','), string.IsNullOrEmpty(args[1]) ? NumberOfDefaultItemsToShow : int.Parse(args[1]));
            }

            Console.WriteLine("Press enter to exit...");
            Console.ReadKey();
        }
    }
}
