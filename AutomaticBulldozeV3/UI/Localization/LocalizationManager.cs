using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using ColossalFramework.Globalization;

namespace AutomaticBulldozeV3.UI.Localization
{
    public class LocalizationManager
    {
        private static readonly string DEFAULT_TRANSLATION_PREFIX = "lang";

        private static LocalizationManager instance;
        private readonly string assemblyPath;
        private static Dictionary<string, string> translations;
        private Assembly assembly = Assembly.GetExecutingAssembly();

        private LocalizationManager()
        {
            assemblyPath = $"{assembly.GetName().Name}.Resources.";
            translations = new Dictionary<string, string>();

            LoadTranslations(LocaleManager.instance.language);
        }

        public delegate void LocaleChangedEventHandler(string language);

        public event LocaleChangedEventHandler eventLocaleChanged;

        public static LocalizationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LocalizationManager();
                }
                return instance;
            }
        }

        private string GetTranslatedFileName(string language)
        {
            switch (language)
            {
                case "jaex":
                    language = "ja";
                    break;
            }

            var filenameBuilder = new StringBuilder(DEFAULT_TRANSLATION_PREFIX);
            if (language != null)
            {
                filenameBuilder.Append("_");
                filenameBuilder.Append(language.Trim().ToLower());
            }
            filenameBuilder.Append(".xml");

            var translatedFilename = filenameBuilder.ToString();

            if (assembly.GetManifestResourceNames().Contains(assemblyPath + translatedFilename))
                return translatedFilename;

            if (language != null && !"en".Equals(language))
                Logger.LogWarning($"Translated file {translatedFilename} not found!");
            return DEFAULT_TRANSLATION_PREFIX + "_en.xml";
        }

        private void LoadTranslations(string language)
        {
            translations.Clear();
            try
            {
                var filename = assemblyPath + GetTranslatedFileName(language);
                string xml;
                using (var rs = assembly.GetManifestResourceStream(filename))
                {
                    using (var sr = new StreamReader(rs))
                    {
                        xml = sr.ReadToEnd();
                    }
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);

                var nodes = xmlDoc.SelectNodes(@"//Language/LocaleResource");

                foreach (XmlNode node in nodes)
                {
                    var name = node.Attributes["Name"].InnerText.Trim();
                    var value = "";
                    var valueNode = node.SelectSingleNode("Value");
                    if (valueNode != null)
                        value = valueNode.InnerText;

                    translations.Add(name, value);
                }
                Logger.LogDebug(() => $"{filename} translations loaded.");
                eventLocaleChanged?.Invoke(language);
            }
            catch (Exception e)
            {
                Logger.LogError($"Error while loading translations: {e}");
            }
        }

        public string GetString(string key)
        {
            string ret;
            try
            {
                translations.TryGetValue(key, out ret);
            }
            catch (Exception e)
            {
                Logger.LogError($"Error fetching the key {key} from the translation dictionary: {e}");
                return key;
            }
            if (ret == null)
                return key;
            return ret;
        }

        public void CheckAndUpdateLocales()
        {
            if (LocaleManager.instance.language != language)
            {
                LoadTranslations(LocaleManager.instance.language);
            }
        }

        internal static int GetButtonWidth()
        {
            switch (LocaleManager.instance.language)
            {
                case null:
                case "fr":
                case "en":
                default:
                    return 200;
                case "pr":
                case "de":
                case "pt":
                    return 210;
                case "es":
                    return 215;
                case "ru":
                case "pl":
                    return 220;
            }
        }
    }
}
