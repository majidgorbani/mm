using SE.GOV.MM.Integration.Log;
using System.Configuration;


namespace SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Helper
{
    /// <summary>
    /// Configuration properties for keys in web.config.
    /// </summary>
    public class ConfigHelper
    {

        /// <summary>
        /// ApplicationName, which application used in logging.
        /// </summary>
        public static string ApplicationName => getConfigurationAppSetting("ApplicationName");

        /// <summary>
        /// Namespace used when signing Xml.
        /// </summary>
        public static string DefaultNamespaceV3 => getConfigurationAppSetting("DefaultNamespaceV3");

        /// <summary>
        /// Language used in message to Mina meddelanden
        /// </summary>
        public static string Language => getConfigurationAppSetting("Language");

        /// <summary>
        /// Certificate used to sign a message sent to MM:s DistributeSecure.
        /// </summary>
        public static string SigningCertificateSubjectName => getConfigurationAppSetting("SigningCertificateSubjectName");

        /// <summary>
        /// If we should sign the xml before sending.
        /// </summary>
        public static bool SignDelivery
        {
            get
            {
                //default
                bool sign = true;
                bool.TryParse(getConfigurationAppSetting("SignDelivery"), out sign);
                return sign;
            }
        }

        /// <summary>
        /// Certificate used to deliver a message to mailbox operator with DeliverSecure.
        /// </summary>
        public static string SSLCertificate_FindByThumbprint => getConfigurationAppSetting("SSLCertificate_FindByThumbprint");

        /// <summary>
        /// Senders OrganizationNumber 
        /// </summary>
        public static string SenderOrganizationNumber {
            get
            {
                var orgNr = getConfigurationAppSetting("SenderOrganizationNumber").Replace("-", string.Empty);
                return (orgNr.Length > 11) ? orgNr : "16" + orgNr;
            }
        } 

        /// <summary>
        /// Sender organizationName
        /// </summary>
        public static string SenderOrganizationName => getConfigurationAppSetting("SenderOrganizationName");

        /// <summary>
        /// Description text
        /// </summary>
        public static string SupportInfoText => getConfigurationAppSetting("SupportInfoText"); 

        /// <summary>
        /// Uri that points to a webpage about supporterrand
        /// </summary>
        public static string SupportInfoUri => getConfigurationAppSetting("SupportInfoUri");

        /// <summary>
        /// Phonenumber to support
        /// </summary>
        public static string SupportInfoPhoneNumber => getConfigurationAppSetting("SupportInfoPhoneNumber");

        /// <summary>
        /// Email address to support
        /// </summary>
        public static string SupportInfoEmailAddress => getConfigurationAppSetting("SupportInfoEmailAddress");

        /// <summary>
        /// Used to validate recipient if personalnumber
        /// </summary>
        public static string RegexValidationPersonNo => getConfigurationAppSetting("RegexValidationPersonNo");

        /// <summary>
        /// Used to validate recipient if organization
        /// </summary>
        public static string RegexValidationOrganizationNo => getConfigurationAppSetting("RegexValidationOrganizationNo");

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
                var errorMessage = string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Helper: Missing key in configuration: {0}", key);
                LogManager.Log(new Log.Log() { EventId = EventId.ConfigurationExceptionMissingKey, Exception = cee, Level = Level.Error, Message = errorMessage });
            }
            return null;
        }
    }
}
