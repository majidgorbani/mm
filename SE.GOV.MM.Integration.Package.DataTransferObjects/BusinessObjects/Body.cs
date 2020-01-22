using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Package.DataTransferObjects.BusinessObjects
{
    [DataContract(Namespace = "https://SE.GOV.MM.Integration.Package/2015/04")]
    public class Body
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public string ContentType { get; set; }
    }
}
