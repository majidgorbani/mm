using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Package.DataLayer.Objects
{
    public class PackageDeliveryMailbox
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public PackageStatus PackageStatus { get; set; }
        public string TransactionId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string MailboxOperator { get; set; }
    }
}
