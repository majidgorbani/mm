using SE.GOV.MM.Integration.Package.DataTransferObjects.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Package.DataTransferObjects.Request
{
    public class SendPackageRequest : RequestBase
    {
        public Mail Mail { get; set; }
        public MailBox MailBox { get; set; }
    }
}
