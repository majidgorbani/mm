using SE.GOV.MM.Integration.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Delivery.DataLayer
{
    /// <summary>
    /// Static properties to expose App.config.
    /// </summary>
    public static class ConfigHelper
    {        

        /// <summary>
        /// Connectionstring to Sql database.
        /// </summary>
        public static string SqlDatabaseConnectionString
        {
            get
            {
                return getConnectionString("SqlDatabase");
            }
        }

        /// <summary>
        /// Sending organizationNumber
        /// </summary>
        public static int DeliveryTimeUntilFailed
        {
            get
            {
                int minutes = 0;
                int.TryParse(getConfigurationAppSetting("DeliveryTimeUntilFailed"), out minutes);
                return minutes;
            }
        }

        /// <summary>
        /// Sending organizationNumber
        /// </summary>
        public static string SenderOrganizationNumber
        {
            get
            {
                var orgNr = getConfigurationAppSetting("SenderOrganizationNumber").Replace("-", string.Empty);
                return (orgNr.Length > 11) ? orgNr : "16" + orgNr; 
            }
        }

        /// <summary>
        /// Webservice verstion of Message
        /// </summary>
        public static string MessageWebserviceVersion
        {
            get
            {
                return getConfigurationAppSetting("MessageWebserviceVersion");
            }
        }

        /// <summary>
        /// Sending organizationNumber
        /// </summary>
        public static string ApplicationName
        {
            get
            {
                return getConfigurationAppSetting("ApplicationName");
            }
        }

        /// <summary>
        /// Timer interval, how often the service checks for statuses in minutes, default is 60 minutes.
        /// </summary>
        public static int TimerInterval
        {
            get
            {
                int minutes = 0;
                int.TryParse(getConfigurationAppSetting("TimerInterval"), out minutes);
                return GetTimeFromMinuteToMillisecond(minutes, 60);
            }
        }

        private static int GetTimeFromMinuteToMillisecond(int minutes, int defaultValue)
        {           
            if (minutes == 0)
            {
                minutes = defaultValue;
            }

            minutes = minutes * 60000;
            return minutes;
        }

        /// <summary>
        /// Gets the configurationValue from app.config appsettings.
        /// </summary>
        private static string getConfigurationAppSetting(string key)
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (ConfigurationErrorsException cee)
            {
                var errorMessage = string.Format("SE.GOV.MM.Integration.Delivery: Couldnt find configurationkey: {0}", key);
                LogManager.Log(new Log.Log() { EventId = EventId.ConfigurationExceptionMissingKey, Exception = cee, Level = Level.Error, Message = errorMessage });
            }

            return null;
        }

        /// <summary>
        /// Gets the connectionstring from app.config connectionstrings.
        /// </summary>
        private static string getConnectionString(string name)
        {      
            try
            {
                return ConfigurationManager.ConnectionStrings[name].ConnectionString;
            }
            catch (ConfigurationErrorsException cee)
            {
                var errorMessage = string.Format("SE.GOV.MM.Integration.Delivery: Couldnt find Connectionstring: {0}", name);
                LogManager.Log(new Log.Log() { EventId = EventId.ConfigurationExceptionMissingKey, Exception = cee, Level = Level.Error, Message = errorMessage });
            }

            return null;
        }
    }
}
