using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.DeliveryMailbox.DataTransferObject.BusinessObject
{
    [DataContract(Namespace = "https://SE.GOV.MM.Integration.DeliveryMailbox/2015/04")]
    public class Mailbox
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Logo { get; set; }
        [DataMember]
        public string ServiceAdress { get; set; }
        [DataMember]
        public string UIAdress { get; set; }

    }
}
