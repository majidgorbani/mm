using SE.GOV.MM.Integration.FaR.DataTransferObjects.BusinessObject;
using SE.GOV.MM.Integration.FaR.DataTransferObjects.Request;
using SE.GOV.MM.Integration.FaR.DataTransferObjects.Response;
using System.ServiceModel;

namespace SE.GOV.MM.Integration.FaR.Contract
{
    [ServiceContract(Namespace = "https://SE.GOV.MM.Integration/Configuration/2015/04")]
    public interface IConfiguration
    {
        [OperationContract]
        ConfigurationEntityResponse ConfigurationEntity(ConfigurationEntityRequest request);       
    }
}
