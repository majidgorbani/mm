using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DigitalEmployment.Lekeberg
{
    [DataContract(Namespace = "https://SE.GOV.MM.Integration.FaR/2015/04")]
    public class AttachmentExtension
    {
        public AttachmentExtension() { }

        [DataMember]
        public string ContentType { get; set; }

        [DataMember]
        public string Extension { get; set; }
        [DataMember]
        public string Filter { get; set; }
    }
}