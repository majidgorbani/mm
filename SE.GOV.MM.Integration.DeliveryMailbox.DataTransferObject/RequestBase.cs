using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SE.GOV.MM.Integration.DeliveryMailbox.DataTransferObject
{
    [DataContract(Namespace = "https://SE.GOV.MM.Integration.DeliveryMailbox/2015/04")]
    public abstract class RequestBase : IExtensibleDataObject
    {
        public ExtensionDataObject ExtensionData { get; set; }

        public Guid RequestId { get; set; }
    }
}
