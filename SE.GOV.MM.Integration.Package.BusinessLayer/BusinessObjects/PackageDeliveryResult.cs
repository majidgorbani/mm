using SE.GOV.MM.Integration.Package.DataLayer.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Package.BusinessLayer.BusinessObjects
{
    public class PackageDeliveryResult
    {
            public DateTime? CreatedDate { get; set; }
            public DateTime? DeliveryDate { get; set; }
            public string DistributionId { get; set; }
            public PackageDeliveryStatus Status { get; set; }
            public string Recipient { get; set; }
    }
}
