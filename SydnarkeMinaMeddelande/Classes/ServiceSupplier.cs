using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DigitalEmployment.Lekeberg
{
    [DataContract(Namespace = "https://SE.GOV.MM.Integration.FaR/2015/04")]
    public class ServiceSupplier
    {
        public ServiceSupplier() { }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string InternalServiceAdress { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ServiceAdress { get; set; }

        [DataMember]
        public string UIAdress { get; set; }
    }
}