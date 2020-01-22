using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Package.DataTransferObjects.Response
{
    public class GetPackageDeliveryResultResponse : ResponseBase
    {
        [DataMember]
    public List<PackageResult> PackageResult { get; set; }

    }

    [DataContract]
    public class PackageResult
    {
        [DataMember]
        public Status Status { get; set; }

        [DataMember]
        public string DistributionId { get; set; }

        [DataMember]
        public string Sender { get; set; }

        [DataMember]
        public string Recipient { get; set; }

        [DataMember]
        public DateTime? CreatedDate { get; set; }

        [DataMember]
        public DateTime? DeliveryDate { get; set; }

    }

    [DataContract]
    public enum Status
    {
        [EnumMember]
        Unknown,
        [EnumMember]
        Failed,
        [EnumMember]
        Pending,
        [EnumMember]
        Delivered
    }
}
