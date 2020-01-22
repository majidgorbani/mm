using SE.GOV.MM.Integration.Package.DataTransferObjects.BusinessObjects;
using SE.GOV.MM.Integration.Package.DataTransferObjects.Request;
using SE.GOV.MM.Integration.Package.DataTransferObjects.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace SE.GOV.MM.Integration.Package.Contract
{
    [ServiceContract(Namespace = "https://SE.GOV.MM.Integration/Package/2015/04")]
    public interface IPackage
    {
        [OperationContract]
        SendPackageResponse SendPackage(SendPackageRequest request);

        [OperationContract]
        SendPackageResponse SendPackageToMailbox(SendPackageRequest request);

        [OperationContract]
        GetPackageDeliveryResultResponse GetPackageDeliveryResult(GetPackageDeliveryResultRequest request);
    }
}
