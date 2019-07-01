using Ecatalog.CoreApi.Concrete;
using log4net;
using log4net.Config;
using System;
using System.Configuration;
using System.Globalization;

namespace Ecatalog.ConsoleApp
{
    /// <summary>
    /// Main Program class
    /// </summary>
    class Program
    {
        /// <summary>
        /// Logger log4Net
        /// </summary>
        private static ILog _logger = LogManager.GetLogger("ConsoleLog");

        static void Main(string[] args)
        {
            InitConsoleApp();
            var catalog = EcatalogManager.GetCatalogInMamory();

            Console.WriteLine(LocalizedStrings.ChooseLanguage);

            var lang = Console.ReadLine().Trim();
            if (lang == Languages.RU || lang == Languages.EN)
            {
                SetLocalizedStrings(lang);
            }
            Console.WriteLine(LocalizedStrings.Welcome);
            while (true)
            {
                try
                {
                    Console.WriteLine();
                    Console.WriteLine(LocalizedStrings.CommandsDescription);
                    Console.WriteLine();
                    var command = Console.ReadLine().Trim().ToUpper();
                    ConsoleUserInteractionClass.HandleCommand(command, catalog, _logger);
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    var exStr = $"{LocalizedStrings.ExceptionIntro}{Environment.NewLine} {ex.Message}";
                    _logger.Error(exStr);
                }
            }
        }

        /// <summary>
        /// Set language to resource file to change language of strings
        /// </summary>
        /// <param name="lang">Language in short format ("ru" or "en")</param>
        private static void SetLocalizedStrings(string lang)
        {
            LocalizedStrings.Culture = CultureInfo.GetCultureInfoByIetfLanguageTag(lang);
        }

        /// <summary>
        /// Init console application settings
        /// </summary>
        private static void InitConsoleApp()
        {
            XmlConfigurator.Configure();
            SetLocalizedStrings(ConfigurationManager.AppSettings["LanguageByDefault"].ToString());
        }
    }
}
