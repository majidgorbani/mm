using SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer;
using SE.GOV.MM.Integration.DeliveryMailbox.DataTransferObject.Request;
using SE.GOV.MM.Integration.DeliveryMailbox.DataTransferObject.Response;
using SE.GOV.MM.Integration.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SE.GOV.MM.Integration.DeliveryMailbox
{
    [ServiceBehavior(Namespace = "https://SE.GOV.MM.Integration.DeliveryMailbox/2015/12")]
    public class DeliveryMailboxService : IDeliveryMailbox
    {
        /// <summary>
        /// Incoming external calls from client.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public SendPackageToMailboxResponse SendPackageToMailbox(SendPackageToMailboxRequest request)
        {
            request.RequestId = Guid.NewGuid();
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox: incoming SendMessageToMailbox with RequestId: {0}", request.RequestId));

            var businessManager = new BusinessManager();
            var result = businessManager.SendPackageToMailbox(request.Mail, request.Mailbox, request.RequestId);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox: leaving SendMessageToMailbox with RequestId: {0}", request.RequestId));
            return result;
        }
    }
}
