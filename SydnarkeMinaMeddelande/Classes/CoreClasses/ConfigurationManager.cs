using SE.GOV.MM.Integration.Outlook.BusinessLayer.Service;

namespace SE.GOV.MM.Integration.Outlook
{
    /// <summary>
    /// Manages the external ConfigurationService 
    /// </summary>
    public class ConfigurationManager
    {
        public ConfigurationManager() { }

        /// <summary>
        /// Loads configuration into ConfigHelper.ConfigurationEntity
        /// </summary>
        /// <returns></returns>
        public void LoadConfiguration()
        {
            var service = new ConfigurationService();
            //Load configurationsettings from configurationservice
            service.GetConfigurationEntity();
        }
    }
}
