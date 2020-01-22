using SE.GOV.MM.Integration.Log;
using System;
using System.Configuration;
using System.Net;
using System.ServiceModel;

namespace SE.GOV.MM.Integration.Package.DataLayer.Service
{
    public class MessageService
    {

        public MessageService()
        {
            // Validate servercertificate if in 'production', the other environments use uXXXXX:XXXX missing the .rsv.se.  
            // Since using a wildcard certificate *.rsv.se, the validation hits and you cant do anything.
            if (!ValidateServerCertificate())
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                (se, cert, chain, sslerror) =>
                {
                    return true;
                };
            }
        }


        public string DistributeSecureV3(SignedDelivery1 message, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.Service: incoming DistributeSecureV3 with RequestId: {0}", requestId));

            MessagePortv3Client client = null;
            string result = string.Empty;

            try
            {
                client = new MessagePortv3Client("MessagePortV3");
                //Call method.
                result = client.distributeSecure(message);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("SE.GOV.MM.Integration.Package.Service.MessageService: Exception have been thrown when calling servicemethod DistributeSecureV3. Recipient: {0}, RequestId: {1}", message.Delivery.Header.Recipient, requestId);
                LogManager.Log(new Log.Log() { Message = errorMessage, EventId = EventId.CommunicationExceptionWithMessage, Exception = ex, Level = Level.Error });
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
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.DataLayer.Service: leaving DistributeSecureV3 with RequestId: {0}", requestId));
            return result;
        }

        /// <summary>
        /// ALWAYS TRUE IF NOT TEST
        /// For test purpose, ValidateServerCertificate validates server certificate throws an exception if something is wrong. In test environment a generalized test certificate is used, 
        /// which will throw exception and you cant do anything. 
        /// </summary>
        /// <returns></returns>
        private bool ValidateServerCertificate()
        {
            bool serverCertificateValidation;
            try
            {
                serverCertificateValidation = bool.Parse(ConfigurationManager.AppSettings["ValidateServerCertificate"].ToString());
            }
            catch
            {
                //default value
                serverCertificateValidation = true;
            }
            return serverCertificateValidation;
        }
    }
}
