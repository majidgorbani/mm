using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace SE.GOV.MM.Integration.Package.DataTransferObjects.BusinessObjects
{
    [DataContract(Namespace = "https://SE.GOV.MM.Integration.Package/2015/04")]
    public class Recipient
    {       
        [DataMember]
        public string To { get; set; }
    }
}
