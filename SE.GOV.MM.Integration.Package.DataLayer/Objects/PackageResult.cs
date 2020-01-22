using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Package.DataLayer.Objects
{
    public class PackageResult
    {
        public int DatabaseId { get; set; }
        public string DistributionId { get; set; }
        public PackageStatus PackageStatus { get; set; }
        public string Recipient { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
