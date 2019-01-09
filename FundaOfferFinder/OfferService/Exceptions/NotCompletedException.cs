using Microsoft.Extensions.Logging;
using System;

namespace OfferService.Exceptions
{
    public class NotCompletedException<T> : Exception
    {
        public NotCompletedException(string msg, ILogger<T> logger = null) : base(msg)
        {
            logger?.LogError(msg);
        }
    }
}
