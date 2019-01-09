using System;

namespace OfferFinderConsole.Extensions
{
    internal static class StringExtension
    {
        public static string SendTo(this string msg, Action<string> method)
        {
            method(msg);
            return msg;
        }
        public static string SendTo(this string msg, object[] objects, Action<string,object[]> method)
        {
            method(msg, objects);
            return msg;
        }
    }
}
