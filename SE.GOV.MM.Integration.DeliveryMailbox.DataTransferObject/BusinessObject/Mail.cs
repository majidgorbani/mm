using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SE.GOV.MM.Integration.DeliveryMailbox.DataTransferObject.BusinessObject
{
    [DataContract(Namespace = "https://SE.GOV.MM.Integration.DeliveryMailbox/2015/04")]
    public class Mail
    {
        [DataMember]
        public string SenderOrgNumber { get; set; }     // organisation number
        [DataMember]
        public string SenderOrgName { get; set; }       // sender organisation number
        [DataMember]
        public string SupInfoUrI { get; set; }          // Suport informatijon uri WWW.organisation.com
        [DataMember]
        public string SupInfoText { get; set; }         // Suport info Text
        [DataMember]
        public string SupInfoPhoneNumber { get; set; }  // Support information number
        [DataMember]
        public string SupInfoEmailAddress { get; set; } // organisation information Mail Adress
        [DataMember]
        public string CertificationBySubjectName { get; set; } // certnamen input information Mail         
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
