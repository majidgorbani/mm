using SE.GOV.MM.Integration.FaR.DataTransferObjects.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.FaR.DataTransferObjects.Response
{
    [DataContract(Namespace = "https://SE.GOV.MM.Integration.Contract/2015/04")]
    public class ConfigurationEntityResponse : ResponseBase
    {
        [DataMember]
        public ConfigurationEntity ConfigurationEntity { get; set; }
    }
}
