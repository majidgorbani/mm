using SE.GOV.MM.Integration.DeliveryMailbox.DataLayer.Service;
using SE.GOV.MM.Integration.DeliveryMailbox.DataTransferObject.BusinessObject;
using SE.GOV.MM.Integration.DeliveryMailbox.DataTransferObject.Response;
using SE.GOV.MM.Integration.Log;
using System;

namespace SE.GOV.MM.Integration.DeliveryMailbox.DataLayer
{
    public class DataManager
    {
        /// <summary>
        /// Calls MailboxOperatorService to deliver SealedDelivery1 and converts response to known object in DeliveryMailbox.
        /// </summary>
        /// <param name="delivery"></param>
        /// <param name="box"></param>
        /// <param name="certificateFindByThumbprint"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public SendPackageToMailboxResponse SendPackageToMailboxV3(SealedDelivery2 delivery, Mailbox box, string certificateFindByThumbprint, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.DataLayer.DataManager: incoming SendPackageToMailboxV3 with RequestId: {0}", requestId));
            var service = new MailboxOperatorService();

            var result = service.SendMessageToMailboxOperatorV3(delivery, box, certificateFindByThumbprint, requestId);
          

            var sendMessageToMailboxResponse = createNewSendMessageToMailboxResponse(result);

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.DataLayer.DataManager: leaving SendPackageToMailboxV3 with RequestId: {0}", requestId));
            return sendMessageToMailboxResponse;
        }

        /// <summary>
        /// Creates a SendPackageToMailboxResponse
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private SendPackageToMailboxResponse createNewSendMessageToMailboxResponse(DeliveryResult result)
        {
            var sendMessageToMailboxResponse = new SendPackageToMailboxResponse();

            if (result != null)
            {
                    sendMessageToMailboxResponse.Delivered = result.Status[0].Delivered;
                    sendMessageToMailboxResponse.RecipientId = result.Status[0].RecipientId;
                    sendMessageToMailboxResponse.TransId = result.TransId;  
            }
            
             return sendMessageToMailboxResponse;
        }
    }
}
