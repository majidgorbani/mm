using SE.GOV.MM.Integration.Log;
using System;
using System.Configuration;
using System.Net;

namespace SE.GOV.MM.Integration.Delivery.DataLayer.Service
{
    public class MessageService
    {
        public MessageService()
        {
            // Validate servercertificate if in 'production', the other environments use uXXXXX:XXXX missing the .rsv.se.  
            // Since using a wildcard certificate *.rsv.se, the validation hits with an unknown error and you cant do anything.
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
        /// For test purpose, ValidateServerCertificate validates server certificate throws an exception if something is wrong. In test environment a generalized test certificate is used, 
        /// which will throw exception and you cant do anything. 
        /// </summary>
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


        public DistributionStatus CheckDistibutionStatusV3(string distributionId)
        {
            var client = new MessagePortv3Client();
            DistributionStatus[] result = null;

            try
            {
                //Call method.
                result = client.checkDistributionStatus(ConfigHelper.SenderOrganizationNumber, distributionId);
            }
            catch (Exception ex)
            {
                // in case of exception, log and call Abort
                string errorMessage = string.Format("SE.GOV.MM.Integration.Delivery.DataLayer: Error checking distributionstatus for distributionId: {0}, ExceptionMessage: {1}", distributionId, ex.Message);
                LogManager.Log(new Log.Log() { EventId = EventId.CommunicationExceptionWithMessage, Exception = ex, Level = Level.Error, Message = errorMessage });
                client.Abort();
            }
            finally
            {
                client.Close();
            }
            LogManager.LogTrace(string.Format("Checked distribution status for id: {0}", distributionId));
            LogManager.LogTrace(string.Format("The result was: {0}, and the result list: {1}", result[0], result));

            return result != null ? result[0] : null;
        }
    }
}
