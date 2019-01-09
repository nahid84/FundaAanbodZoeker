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
            int argsCount = args.Count();

            if(argsCount < 1 || argsCount > 2)
            {
                ServiceRunner.PrintUsage();

            } else if(argsCount == 1)
            {
                ServiceRunner.PrintData(args[0].Split(','), NumberOfDefaultItemsToShow);

            } else if(argsCount == 2)
            {
                if(int.TryParse(args[1], out int result))
                {
                    ServiceRunner.PrintData(args[0].Split(','), result);

                } else
                {
                    Console.WriteLine("Second argument must be a number");
                }
            }

            Console.WriteLine("Press enter to exit...");
            Console.ReadKey();
        }
    }
}
