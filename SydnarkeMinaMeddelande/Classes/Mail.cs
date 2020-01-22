using SE.GOV.MM.Integration.Package.DataLayer.DeliveryMailbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace DigitalEmployment.Lekeberg
{
    [DataContract(Namespace = "https://SE.GOV.MM.Integration.Package/2015/04")]
    public class Mail
    {
        [DataMember]
        public string CorrealationId { get; set; }
        [DataMember]
        public string MessageHeaderId { get; set; }
        [DataMember]
        public string Sender { get; set; }
        [DataMember]
        public string Reference { get; set; }
        [DataMember]
        public Recipient Recipient { get; set; }
        [DataMember]
        public string Subject { get; set; }
        [DataMember]
        public Body Body { get; set; }
        [DataMember]
        public List<Attachment> Attachments { get; set; }
        [DataMember]
        public List<MetaData> MetaData { get; set; }
    }

}
