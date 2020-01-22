using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Package.DataTransferObjects.Response
{
    public class SendPackageResponse : ResponseBase
    {
        [DataMember]
        public string DeliveryStatus { get; set; }

        [DataMember]
        public string DistributionId { get; set; }
    }
}
