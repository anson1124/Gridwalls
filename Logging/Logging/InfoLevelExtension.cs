using System;

namespace Logging
{
    internal static class InfoLevelExtension
    {
        public static string Shortversion(this InfoLevel infoLevel)
        {
            int maxSubstringLength = Math.Min(infoLevel.ToString().Length, 5);
            return $"{infoLevel.ToString().Substring(0, maxSubstringLength).ToUpper().PadRight(5)}";
        }
    }
}