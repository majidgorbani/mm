using SE.GOV.MM.Integration.DeliveryMailbox.DataTransferObject.Request;
using SE.GOV.MM.Integration.DeliveryMailbox.DataTransferObject.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SE.GOV.MM.Integration.DeliveryMailbox
{

    [ServiceContract(Namespace = "https://SE.GOV.MM.Integration.DeliveryMailbox/2015/12")]
    public interface IDeliveryMailbox
    {
        [OperationContract]
        SendPackageToMailboxResponse SendPackageToMailbox(SendPackageToMailboxRequest request);
    }
}
