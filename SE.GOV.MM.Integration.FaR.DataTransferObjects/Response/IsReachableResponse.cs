using SE.GOV.MM.Integration.FaR.DataTransferObjects.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.FaR.DataTransferObjects.Response
{
    public class IsReachableResponse : ResponseBase
    {
        public bool IsReachable { get; set; }
        public ServiceSupplier ServiceSupplier { get; set; }
    }
}
