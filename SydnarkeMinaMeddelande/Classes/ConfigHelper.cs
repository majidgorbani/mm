using System.Configuration;
using SE.GOV.MM.Integration.FaR.DataTransferObjects.BusinessObject;
using SE.GOV.MM.Integration.Log;



namespace DigitalEmployment.Lekeberg
{
    public static class ConfigHelper
    {
        /// <summary>
        /// ConfigurationEntity includes configuration properties.
        /// </summary>
        public static ConfigurationEntity ConfigurationEntity { get; set; }

        /// <summary>
        /// Gets the configurationValue from web.configs appsettings.
        /// </summary>
        private static string getConfigurationAppSetting(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (ConfigurationErrorsException cee)
            {
                string errorMessage = string.Format("Missing key: {0} in web.config.", key);
         //       LogManager.Log(new Log.Log() { Message = errorMessage, EventId = EventId.ConfigurationExceptionMissingKey, Exception = cee, Level = Level.Error });
            }
            return null;
        }

    }
}
