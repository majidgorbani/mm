using SE.GOV.MM.Integration.DeliveryMailbox.DataTransferObject.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace SE.GOV.MM.Integration.DeliveryMailbox.DataTransferObject.Request
{
    /// <summary>
    /// Request object to DeliveryMailboxService
    /// </summary>
    public class SendPackageToMailboxRequest : RequestBase
    {
        public Mail Mail { get; set; }
        public Mailbox Mailbox { get; set; }
    }
}
