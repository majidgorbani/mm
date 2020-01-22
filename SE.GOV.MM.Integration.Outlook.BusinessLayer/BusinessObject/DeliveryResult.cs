using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Outlook.BusinessLayer.BusinessObject
{
    public class DeliveryResult
    {
        public DeliveryStatus Status { get; set; }
        public string DistributionId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string Recipient { get; set; }
    }
}
