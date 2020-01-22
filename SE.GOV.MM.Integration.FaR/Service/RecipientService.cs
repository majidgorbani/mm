using SE.GOV.MM.Integration.FaR.Helper;
using System;
using System.ServiceModel;
using System.ServiceModel.Security;
using SE.GOV.MM.Integration.Log;
using System.Configuration;
using System.Net;

namespace SE.GOV.MM.Integration.FaR.Service
{
    public class RecipientService
    {

        public RecipientService()
        {
            if (!ValidateServerCertificate())
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                (se, cert, chain, sslerror) =>
                {
                    return true;
                };
            }
        }

        /// <summary>
        /// ALWAYS TRUE IF NOT TEST
        /// For test purpose, ValidateServerCertificate validates server certificate throws an exception if something is wrong. In test environment a wildcard test certificate is used, 
        /// which will throw exception. 
        /// </summary>
        /// <returns></returns>
        private bool ValidateServerCertificate()
        {
            bool serverCertificateValidationCallback;
            try
            {
                serverCertificateValidationCallback = bool.Parse(ConfigurationManager.AppSettings["ValidateServerCertificate"].ToString());
            }
            catch
            {
                //default value
                serverCertificateValidationCallback = true;
            }
            return serverCertificateValidationCallback;
        }

        /// <summary>
        /// Call webservice RecipientV3 in FaR
        /// </summary>
        /// <param name="recipient">Recipient to check if reachable</param>
        /// <param name="requestId">Guid</param>
        /// <returns></returns>
        public ReachabilityStatus[] IsUserReachableInFaRV3(string recipient, Guid requestId, string senderOrg)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.FaR.Service.RecipientService: incoming IsUserReachableInFaRV3 for requestId: {0}", requestId));

            ReachabilityStatus[] status = null;
            RecipientPortv3Client client = null;

            try
            {

                client = new RecipientPortv3Client("RecipientPortv3");

                //var senderOrganizationNumber = ConfigHelper.SenderOrganizationNumber;
                var senderOrganizationNumber = senderOrg;

                // Call FaR, check if the recipient is reachable
                status = client.isReachable(senderOrganizationNumber, new string[] { recipient });
            }
            catch (SecurityNegotiationException se)
            {
                client.Abort();
                string errorMessage = string.Format("SE.GOV.MM.Integration.FaR.Service.RecipientService: SecurityNegotiationException during request to Recipient, Exception: {0}, Trying to check recipient: {1}. With requestId: {2}", se.Message, recipient, requestId);
                LogManager.Log(new Log.Log() { Message = errorMessage, Exception = se, EventId = EventId.CommunicationExceptionWithRecipient, Level = Level.Error });

                throw se;
            }
            catch (TimeoutException te)
            {
                string errorMessage = string.Format("SE.GOV.MM.Integration.FaR.Service.RecipientService: TimeoutException during request to Recipient, Exception: {0}, Trying to check recipient: {1}. With requestId: {2}", te.Message, recipient, requestId);
                LogManager.Log(new Log.Log() { Message = errorMessage, Exception = te, EventId = EventId.CommunicationExceptionWithRecipient, Level = Level.Error });

                throw te;
            }
            catch (CommunicationException ce)
            {
                string errorMessage = string.Format("SE.GOV.MM.Integration.FaR.Service.RecipientService: CommunicationException during request to Recipient, Exception: {0}, Trying to check recipient: {1}. With requestId: {2}", ce.Message, recipient, requestId);
                LogManager.Log(new Log.Log() { Message = errorMessage, Exception = ce, EventId = EventId.CommunicationExceptionWithRecipient, Level = Level.Error });

                throw ce;
            }
            catch (Exception e)
            {
                string errorMessage = string.Format("SE.GOV.MM.Integration.FaR.Service.RecipientService: Exception during request to Recipient, Exception: {0}, Trying to check recipient: {1}. With requestId: {2}", e, recipient, requestId);
                LogManager.Log(new Log.Log() { Message = errorMessage, Exception = e, EventId = EventId.CommunicationExceptionWithRecipient, Level = Level.Error });

                throw e;
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

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.FaR.Service.RecipientService: leaving IsUserReachableInFaRV3 for requestId: {0}", requestId));
            return status;

        }
       
    }
}