using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using SE.GOV.MM.Integration.Package.DataTransferObjects.BusinessObjects;
using SE.GOV.MM.Integration.Package.DataTransferObjects.Request;
using SE.GOV.MM.Integration.Outlook.BusinessLayer.Helper;
using System.ComponentModel;
using SE.GOV.MM.Integration.Outlook.BusinessLayer.BusinessObject;
using SE.GOV.MM.Integration.Package.DataTransferObjects.Response;
using SE.GOV.MM.Integration.Log;
using SE.GOV.MM.Integration.Package.Contract;

namespace SE.GOV.MM.Integration.Outlook.BusinessLayer.Service
{
    public class PackageService
    {
        /// <summary>
        /// SendPackageRequest to PackageService for deliver to dispatcher.
        /// </summary>
        public SendPackageResult SendPackage(Mail mailItem, MailBox mailBox)
        {
            LogManager.LogTrace("SE.GOV.MM.Integration.Outlook.Service.MessageService: incoming SendPackage");

            var request = new SendPackageRequest() { Mail = mailItem, MailBox = mailBox };
            var result = new SendPackageResult() { MessageSent = true, Recipient = mailItem.Recipient.To };

            ServiceClient<IPackage> client = null;

            try
            {

                client = new ServiceClient<IPackage>("WSHttpBinding_IPackage");
                //client = new ServiceClient<IPackage>("BasicHttpBinding_IPackage");
               
                var response = client.Proxy.SendPackage(request);

                result.DeliveryStatus = response.DeliveryStatus;
                result.RequestId = response.RequestId;
                result.DistributionId = response.DistributionId;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                result.MessageSent = false;
                LogManager.Log(new Log.Log() { Exception = ce, Message = "SE.GOV.MM.Integration.Outlook.Service.PackageService: CommunicationException error sending package to PackageService.", EventId = EventId.CommunicationExceptionWithPackage, Level = Level.Error });
            }
            catch (Exception ex)
            {
                client.Abort();
                result.MessageSent = false;
                LogManager.Log(new Log.Log() { Exception = ex, Message = "SE.GOV.MM.Integration.Outlook.Service.PackageService: Exception error sending package to PackageService.", EventId = EventId.CommunicationExceptionWithPackage, Level = Level.Error });
            }
            finally
            {
                if (client.State == CommunicationState.Faulted)
                    client.Abort();
                client = null;
            }

            LogManager.LogTrace("SE.GOV.MM.Integration.Outlook.Service.PackageService: leaving SendPackage");
            return result;
        }

        /// <summary>
        /// SendPackageRequest to PackageService for deliver to mailboxoperator.
        /// </summary>
        public SendPackageResult SendPackageToMailBox(Mail mailItem, MailBox mailBox)
        {
            LogManager.LogTrace("SE.GOV.MM.Integration.Outlook.Service.MessageService: incoming SendPackageToMailBox");

            var request = new SendPackageRequest() { Mail = mailItem, MailBox = mailBox };
            var result = new SendPackageResult() { MessageSent = true, Recipient = mailItem.Recipient.To };
            ServiceClient<IPackage> client = null;

            try
            {
                client = new ServiceClient<IPackage>("WSHttpBinding_IPackage");
                //client = new ServiceClient<IPackage>("BasicHttpBinding_IPackage");
                
                var response = client.Proxy.SendPackageToMailbox(request);

                result.DeliveryStatus = response.DeliveryStatus;
                result.RequestId = response.RequestId;
                result.DistributionId = response.DistributionId;
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                result.MessageSent = false;
                LogManager.Log(new Log.Log() { Exception = ce, Message = "SE.GOV.MM.Integration.Outlook.Service.PackageService: CommunicationException error sending package to PackageService.", EventId = EventId.CommunicationExceptionWithPackage, Level = Level.Error });
            }
            catch (Exception ex)
            {
                client.Abort();
                result.MessageSent = false;
                LogManager.Log(new Log.Log() { Exception = ex, Message = "SE.GOV.MM.Integration.Outlook.Service.PackageService: Exception error sending package to PackageService.", EventId = EventId.CommunicationExceptionWithPackage, Level = Level.Error });
            }
            finally
            {
                if (client.State == CommunicationState.Faulted)
                    client.Abort();
                client = null;
            }
            LogManager.LogTrace("SE.GOV.MM.Integration.Outlook.Service.PackageService: leaving SendPackageToMailBox");
            return result;
        }

        /// <summary>
        /// Get status from PackageService for a already sent Package.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="distributionId"></param>
        /// <returns></returns>
        public List<DeliveryResult> GetPackageDelivery(string sender, string pnrOrgNr, int maxStatusMessages)
        {
            LogManager.LogTrace("SE.GOV.MM.Integration.Outlook.Service.PackageService: incoming GetPackageDeliveryStatus");

            var request = new GetPackageDeliveryResultRequest() { PnrOrgNr = pnrOrgNr, Sender = sender, MaxStatusMessages = maxStatusMessages };
            List<DeliveryResult> result;
            ServiceClient<IPackage> client = null;

            try
            {
                client = new ServiceClient<IPackage>("WSHttpBinding_IPackage");
                var response = client.Proxy.GetPackageDeliveryResult(request);
                result = getDeliveryStatusResultFromDeliveryResult(response);              
            }
            catch (CommunicationException ce)
            {
                client.Abort();
                LogManager.Log(new Log.Log() { Exception = ce, Message = "SE.GOV.MM.Integration.Outlook.Service.PackageService: CommunicationException error getting delivery status.", EventId = EventId.CommunicationExceptionWithPackage, Level = Level.Error });
                throw ce;
            }
            catch (Exception ex)
            {
                client.Abort();
                LogManager.Log(new Log.Log() { Exception = ex, Message = "SE.GOV.MM.Integration.Outlook.Service.PackageService: Exception error getting delivery status.", EventId = EventId.CommunicationExceptionWithPackage, Level = Level.Error });
                throw ex;
            }
            finally
            {
                if (client.State == CommunicationState.Faulted)
                    client.Abort();
                client = null;
            }

            LogManager.LogTrace("SE.GOV.MM.Integration.Outlook.Service.PackageService: leaving GetPackageDeliveryStatus");
            return result;
        }

        /// <summary>
        /// Converts a GetPackageDeliveryResultResponse to a DeliveryResult.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private List<DeliveryResult> getDeliveryStatusResultFromDeliveryResult(GetPackageDeliveryResultResponse response)
        {
            var listOfResults = new List<DeliveryResult>();

            if (response != null)
            {
                foreach (PackageResult result in response.PackageResult)
                {
                    var deliveryStatusResult = new DeliveryResult()
                    {
                        Status = ConvertStatusToDeliveryStatus(result.Status),
                        CreatedDate = result.CreatedDate,
                        DeliveredDate = result.DeliveryDate,
                        DistributionId = result.DistributionId,
                        Recipient = result.Recipient
                    };
                    listOfResults.Add(deliveryStatusResult);
                }
            }

            return listOfResults;
        }

        private DeliveryStatus ConvertStatusToDeliveryStatus(Status status)
        {
            switch (status)
            {
                case Status.Delivered:
                    return DeliveryStatus.Delivered;
                case Status.Failed:
                    return DeliveryStatus.Failed;
                case Status.Pending:
                    return DeliveryStatus.Pending;
                default:
                    return DeliveryStatus.Unknown;
            }
        }
    }
}
