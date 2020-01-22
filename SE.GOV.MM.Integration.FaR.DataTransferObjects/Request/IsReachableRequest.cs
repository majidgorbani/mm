using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.FaR.DataTransferObjects.Request
{
    public class IsReachableRequest : RequestBase
    {
        public string RecipientNumber { get; set; }
    }
}
