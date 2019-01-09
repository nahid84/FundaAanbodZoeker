using System;

namespace OfferFinderConsole.Extensions
{
    /// <summary>
    /// String extension class
    /// </summary>
    internal static class StringExtension
    {
        /// <summary>
        /// Send to message to the delegated method
        /// </summary>
        /// <param name="msg">Message to send</param>
        /// <param name="method">Delegate method</param>
        /// <returns>Returns the message back</returns>
        internal static string SendTo(this string msg, Action<string> method)
        {
            method(msg);
            return msg;
        }

        /// <summary>
        /// Send to message to the delegated method
        /// </summary>
        /// <param name="msg">Message to send</param>
        /// <param name="objects">Objects to send</param>
        /// <param name="method">Delegate method</param>
        /// <returns>Returns the message back</returns>
        internal static string SendTo(this string msg, object[] objects, Action<string,object[]> method)
        {
            method(msg, objects);
            return msg;
        }
    }
}
