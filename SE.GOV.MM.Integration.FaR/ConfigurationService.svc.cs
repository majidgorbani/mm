using SE.GOV.MM.Integration.FaR.Contract;
using SE.GOV.MM.Integration.FaR.DataTransferObjects.BusinessObject;
using SE.GOV.MM.Integration.FaR.DataTransferObjects.Request;
using SE.GOV.MM.Integration.FaR.DataTransferObjects.Response;
using SE.GOV.MM.Integration.FaR.Helper;
using System.ServiceModel;

namespace SE.GOV.MM.Integration.FaR
{
    [ServiceBehavior(Namespace = "https://SE.GOV.MM.Integration.Configuration/2015/04")]
    public class ConfigurationService : IConfiguration
    {
        /// <summary>
        /// Client configuration.
        /// </summary>
        public ConfigurationEntityResponse ConfigurationEntity(ConfigurationEntityRequest request)
        {
            var configurationEntity = new ConfigurationEntity()
            {
                AttachmentExtensions = ConfigHelper.AttachmentExtensions,
                MaxTotalAttachmentSize = ConfigHelper.MaxTotalAttachmentSize,
                RegexValidationOrganizationNo = ConfigHelper.RegexValidationOrganizationNo,
                RegexValidationPersonNo = ConfigHelper.RegexValidationPersonNo,
                Reference = ConfigHelper.Reference,
                UseExternalDispatcher = ConfigHelper.UseExternalDispatcher,
                MaxAllowedStatusMessages = ConfigHelper.MaxAllowedStatusMessages,
                RelativePathToSignature = ConfigHelper.RelativePathToSignature
            };

            var response = new ConfigurationEntityResponse()
            {
                ConfigurationEntity = configurationEntity
            };

            return response;
        }
    }
}
