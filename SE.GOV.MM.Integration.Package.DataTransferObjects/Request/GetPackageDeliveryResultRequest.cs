using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Package.DataTransferObjects.Request
{
    public class GetPackageDeliveryResultRequest : RequestBase
    {
        [DataMember]
        public string Sender {  get; set; }

        [DataMember]
        public string PnrOrgNr { get; set; }

        [DataMember]
        public int MaxStatusMessages { get; set; }
    }
}
