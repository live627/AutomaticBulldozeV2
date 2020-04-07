namespace AutomaticBulldozeV3.UI.Localization
{
    public static class LocalizationExtensions
    {
        public static string Translate(this string key)
        {
            var localization = LocalizationManager.Instance;
            return localization.GetString(key);
        }
    }
}