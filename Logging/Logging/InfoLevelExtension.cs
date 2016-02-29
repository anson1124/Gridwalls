namespace Logging
{
    internal static class InfoLevelExtension
    {
        public static string Shortversion(this InfoLevel infoLevel)
        {
            return $"{infoLevel.ToString().Substring(0, 4).ToUpper().PadRight(5)}";
        }
    }
}