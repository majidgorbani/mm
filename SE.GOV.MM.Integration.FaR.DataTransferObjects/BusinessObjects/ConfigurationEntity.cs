using System.ServiceModel;
using System.Configuration;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace SE.GOV.MM.Integration.FaR.DataTransferObjects.BusinessObject
{
    [DataContract(Namespace = "https://SE.GOV.MM.Integration.FaR/2015/04")]
    public class ConfigurationEntity
    {

        public ConfigurationEntity()
        {
            
        }
        [DataMember]
        public string RelativePathToSignature { get; set; }

        [DataMember]
        public int MaxAllowedStatusMessages { get; set; }

        [DataMember]
        public string Reference { get; set; }

        [DataMember]
        public string RegexValidationPersonNo { get; set; }

        [DataMember]
        public string RegexValidationOrganizationNo { get; set; }

        [DataMember]
        public List<AttachmentExtension> AttachmentExtensions { get; set; }

        [DataMember]
        public int MaxTotalAttachmentSize { get; set; }

        [DataMember]
        public bool UseExternalDispatcher { get; set; }

    }
}
