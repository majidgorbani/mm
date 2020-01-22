using SE.GOV.MM.Integration.Log;
using SE.GOV.MM.Integration.Package.DataLayer.DeliveryMailbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace SE.GOV.MM.Integration.Package.DataLayer.Service
{
    public class DeliveryMailboxService
    {
        /// <summary>
        /// Check status of a sent message.
        /// </summary>
        public SendPackageToMailboxResponse SendPackageToMailBox(SendPackageToMailboxRequest sendPackageToMailboxRequest, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.Service.DeliveryMailboxService: incoming SendPackageToMailBox with RequestId: {0}", requestId));

            DeliveryMailboxClient client = null;
            SendPackageToMailboxResponse result = null;
            try
            {
                //Call method.
                client = new DeliveryMailboxClient();
                result = client.SendPackageToMailbox(sendPackageToMailboxRequest);
            }
            catch (CommunicationException ce)
            {
                string errorMessage = string.Format("SE.GOV.MM.Integration.Package.DataLayer.Service.DeliveryMailboxService: Error sending package to DeliveryMailbox, RequestId: {0}", requestId);
                LogManager.Log(new Log.Log() { EventId = EventId.CommunicationExceptionWithDeliveryMailbox, Exception = ce, Level = Level.Error, Message = errorMessage });
                throw ce;
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("SE.GOV.MM.Integration.Package.DataLayer.Service.DeliveryMailboxService: Error sending package to DeliveryMailbox, RequestId: {0}", requestId);
                LogManager.Log(new Log.Log() { EventId = EventId.CommunicationExceptionWithDeliveryMailbox, Exception = ex, Level = Level.Error, Message = errorMessage });
                throw ex;
            }
            finally
            {
                if (client != null && client.State == CommunicationState.Faulted)
                {
                    client.Abort();
                    client.Close();
                }
                client = null;
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.Service.DeliveryMailboxService: leaving SendPackageToMailBox with RequestId: {0}", requestId));
            return result;
        }
    }
}
