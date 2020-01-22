using SE.GOV.MM.Integration.FaR.DataTransferObjects.BusinessObject;
using SE.GOV.MM.Integration.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;

namespace SE.GOV.MM.Integration.FaR.Helper
{
    /// <summary>
    /// Reads web.config appSettings values.
    /// </summary>
    public static class ConfigHelper
    {
        private static int _defaultMaxAllowedStatusMessages = 10;

        // The default total maximum size of attachments in a message, in bytes.
        // It is NOT recommended to exceed 2MB.
        private static int _defaultMaxTotalAttachmentSize = 50480000;

        public static int MaxAllowedStatusMessages
        {
            get
            {
                var allowedMessages = getConfigurationAppSetting("MaxAllowedStatusMessages");

                var max = 0;
                int.TryParse(allowedMessages, out max);

                if (max <= 0)
                {
                    max = _defaultMaxAllowedStatusMessages;
                }

                return max;
            }
        }

        public static string RelativePathToSignature
        {
            get
            {
                var relativePathToSignature = getConfigurationAppSetting("RelativePathToSignature");

                if (string.IsNullOrEmpty(relativePathToSignature))
                {
                    throw new ConfigurationErrorsException("Missing value for key RelativePathToSignature");
                }

                return relativePathToSignature;
            }
        }

        public static string SenderOrganizationNumber 
        { 
            get 
            { 
                
                var senderOrgNr = getConfigurationAppSetting("SenderOrganizationNumber").Replace("-", string.Empty);

                if (string.IsNullOrEmpty(senderOrgNr))
                {
                    throw new ConfigurationErrorsException("Missing value for key SenderOrganizationNumber");
                }
                return (senderOrgNr.Length > 11) ? senderOrgNr : "16" + senderOrgNr;
            } 
        }

        public static bool UseExternalDispatcher
        {
            get
            {
                var externalDispatcher = getConfigurationAppSetting("UseExternalDispatcher");

                var useExternalDispatcher = false;
                bool.TryParse(externalDispatcher, out useExternalDispatcher);

                return useExternalDispatcher;
            }
        }

        public static string ApplicationName
        {
            get
            {
                var applicationName = getConfigurationAppSetting("ApplicationName");

                if (string.IsNullOrEmpty(applicationName))
                {
                    applicationName = "FaRService";
                }

                return applicationName;
            }
        }

        public static string Reference
        {
            get
            {
                var reference = getConfigurationAppSetting("Reference");

                if (string.IsNullOrEmpty(reference))
                {
                    reference = "Outlook integration";
                }

                return reference;
            }
        }

        public static string RegexValidationPersonNo 
        { 
            get 
            { 
                var regexValidationPersonNo = getConfigurationAppSetting("RegexValidationPersonNo");

                if (string.IsNullOrEmpty(regexValidationPersonNo))
                {
                    regexValidationPersonNo = @"^(18|19|20)[0-9][0-9](0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])(\d{4})$";
                }

                return regexValidationPersonNo;
            } 
        }

        public static string RegexValidationOrganizationNo 
        { 
            get 
            {
                var regexValidationOrganizationNo = getConfigurationAppSetting("RegexValidationOrganizationNo");

                if (string.IsNullOrEmpty(regexValidationOrganizationNo))
                {
                    regexValidationOrganizationNo = @"(16[2-9]\d{9})";
                }

                return regexValidationOrganizationNo;
            } 
        }

        public static string ConfigurationsFilePath 
        { 
            get 
            {
                var configurationsFilePath = getConfigurationAppSetting("ConfigurationsFilePath");

                if (string.IsNullOrEmpty(configurationsFilePath))
                {
                    throw new ConfigurationErrorsException("Missing value for key ConfigurationsFilePath");
                }

                return configurationsFilePath;
            } 
        }

        public static int MaxTotalAttachmentSize
        {
            get
            {
                var maxTotalAttachmentSize = getConfigurationAppSetting("MaxTotalAttachmentSize");

                var max = 0;
                int.TryParse(maxTotalAttachmentSize, out max);
                
                if (max <= 0)
                {
                    max = _defaultMaxTotalAttachmentSize;
                }

                return max;
            }
        }

        public static List<AttachmentExtension> AttachmentExtensions { get { return GetListOfValidAttachments(); } }

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
                LogManager.Log(new Log.Log(){ Message = errorMessage, EventId = EventId.ConfigurationExceptionMissingKey, Exception = cee, Level = Level.Error });
            }
            return null;
        }

        /// <summary>
        /// Gets the allowed file extensions from Configurations.xml. Path to where file is saved is located in web.config appSettings.
        /// </summary>
        /// <returns></returns>
        private static List<AttachmentExtension> GetListOfValidAttachments()
        {
            var attachments = new List<AttachmentExtension>();
            try
            {
                XDocument attachment = XDocument.Load(ConfigurationsFilePath);

                attachments = (from extension in attachment.Descendants("Extension")
                                   select new AttachmentExtension
                                   {
                                       ContentType = extension.Attribute("ContentType").Value,
                                       Extension = extension.Attribute("Name").Value,
                                       Filter = extension.Attribute("Filter").Value
                                   }).ToList();              
            }
            catch (Exception e)
            {
                string errorMessage = string.Format("SE.GOV.MM.Integration.FaR.Helper.ConfigHelper: Error loading Attachments, check filepath: {0} in config. Exception: {1}", ConfigurationsFilePath, e.Message);
                LogManager.Log(new Log.Log() { Message = errorMessage, EventId = EventId.ConfigurationException, Exception = e, Level = Level.Error });
            }
            return attachments;
        }
    }
}