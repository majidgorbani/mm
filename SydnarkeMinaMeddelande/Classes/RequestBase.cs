using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DigitalEmployment.Lekeberg
{
    [DataContract(Namespace = "https://SE.GOV.MM.Integration.FaR/2015/04")]
    public abstract class RequestBase : IExtensibleDataObject
    {
        public ExtensionDataObject ExtensionData { get; set; }

        public Guid RequestId { get; set; }
    }
}