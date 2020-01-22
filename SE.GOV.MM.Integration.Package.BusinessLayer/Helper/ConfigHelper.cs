using SE.GOV.MM.Integration.Log;
using System.Configuration;

namespace SE.GOV.MM.Integration.Package.BusinessLayer.Helper
{

    /// <summary>
    /// Configuration properties for keys in XX.config.
    /// </summary>
    public class ConfigHelper
    {
        /// <summary>
        /// ApplicationName, which application mainly used in logging.
        /// </summary>
        public static string ApplicationName => GetConfigurationAppSetting("ApplicationName");    

        /// <summary>
        /// Namespace used when signing Xml for Message version 3.
        /// </summary>
        public static string DefaultNamespaceV3 => GetConfigurationAppSetting("DefaultNamespaceV3");

        /// <summary>
        /// Language used in message to Mina meddelanden
        /// </summary>
        public static string Language => GetConfigurationAppSetting("Language");

        /// <summary>
        /// Certificate used to sign a message sent to MM:s DistributeSecure.
        /// </summary>
        public static string SigningCertificateSubjectName => GetConfigurationAppSetting("SigningCertificateSubjectName");

        /// <summary>
        /// Senders OrganizationNumber 
        /// </summary>
        public static string SenderOrganizationNumber
        {
            get
            {
                var orgNr = GetConfigurationAppSetting("SenderOrganizationNumber").Replace("-", string.Empty);
                return (orgNr.Length > 11) ? orgNr : "16" + orgNr;
            }
        }

        /// <summary>
        /// Sender organizationName
        /// </summary>
        public static string SenderOrganizationName =>  GetConfigurationAppSetting("SenderOrganizationName");

        /// <summary>
        /// Description text
        /// </summary>
        public static string SupportInfoText => GetConfigurationAppSetting("SupportInfoText");

        /// <summary>
        /// Uri that points to a webpage about supporterrand
        /// </summary>
        public static string SupportInfoUri => GetConfigurationAppSetting("SupportInfoUri");

        /// <summary>
        /// Phonenumber to support
        /// </summary>
        public static string SupportInfoPhoneNumber => GetConfigurationAppSetting("SupportInfoPhoneNumber");

       /// <summary>
       /// Email address to support
       /// </summary>
        public static string SupportInfoEmailAddress => GetConfigurationAppSetting("SupportInfoEmailAddress");

        /// <summary>
        /// Used to validate recipient if personalnumber
        /// </summary>
        public static string RegexValidationPersonNo => GetConfigurationAppSetting("RegexValidationPersonNo");

        /// <summary>
        /// Used to validate recipient if organization
        /// </summary>
        public static string RegexValidationOrganizationNo => GetConfigurationAppSetting("RegexValidationOrganizationNo");

        /// <summary>
        /// If we should sign the xml before sending.
        /// </summary>
        public static bool SignDelivery
        {
            get
            {
                //default
                bool sign = true;
                bool.TryParse(GetConfigurationAppSetting("SignDelivery"), out sign);
                return sign;
            }
        }

        /// <summary>
        /// Gets the configurationValue from web.configs appsettings.
        /// </summary>
        private static string GetConfigurationAppSetting(string key)
        {            
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch (ConfigurationErrorsException cee)
            {
                var errorMessage = string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Helper.ConfigHelper: Missing key in configuration: {0}", key);
                LogManager.Log(new Log.Log() { EventId = EventId.ConfigurationExceptionMissingKey, Exception = cee, Level = Level.Error, Message = errorMessage });
            }
            return null;
        }
    }
}
