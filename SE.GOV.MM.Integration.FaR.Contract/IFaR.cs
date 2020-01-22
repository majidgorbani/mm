using SE.GOV.MM.Integration.FaR.DataTransferObjects.Request;
using SE.GOV.MM.Integration.FaR.DataTransferObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.FaR.Contract
{
    [ServiceContract(Namespace = "https://SE.GOV.MM.Integration/FaR/2015/04")]
    public interface IFaR
    {
        /// <summary>
        /// key is either person or organisation number.
        /// </summary>
        [OperationContract()]
        IsReachableResponse IsReachable(IsReachableRequest request, string senderOrg);
        
    }
}
