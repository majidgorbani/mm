using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.DeliveryMailbox.DataTransferObject.Response
{
    /// <summary>
    /// Response object to SendPackageToMailbox.
    /// </summary>
    public class SendPackageToMailboxResponse : ResponseBase
    {
        [DataMember]
        public string TransId { get; set; }

        [DataMember]
        public string RecipientId { get; set; }

        [DataMember]
        public bool Delivered { get; set; }
    }
}
