using Microsoft.Extensions.Logging;
using System;

namespace OfferService.Exceptions
{
    /// <summary>
    /// Exception used to indicate operation not completed
    /// </summary>
    public class NotCompletedException : Exception
    {
        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="msg">The exception message</param>
        /// <param name="logger">Logger to be used to print exception</param>
        public NotCompletedException(string msg, ILogger logger = null) : base(msg)
        {
            logger?.LogError(msg);
        }
    }
}
