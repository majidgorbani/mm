using SE.GOV.MM.Integration.DeliveryMailbox.DataTransferObject.BusinessObject;
using SE.GOV.MM.Integration.Log;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml;

namespace SE.GOV.MM.Integration.DeliveryMailbox.DataLayer.Service
{
    public class MailboxOperatorService
    {

        public MailboxOperatorService()
        {
            // Validate servercertificate if in 'production', the other environments use uXXXXX:XXXX missing the .rsv.se.  
            // Since using a wildcard certificate *.rsv.se, the validation hits and you cant do anything.
            if (!ValidateServerCertificate())
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                (se, cert, chain, sslerror) => true;
            }
        }

        /// <summary>
        /// ALWAYS TRUE IF NOT TEST
        /// For test purpose, ValidateServerCertificate validates server certificate throws an exception if something is wrong. In test environment a generalized(wildcard) test certificate is used, 
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

        /// <summary>
        /// Sends a SealedDelivery object to the recipient mailbox using the certificate configured in web.config.
        /// </summary>
        /// <param name="delivery"></param>
        /// <param name="mailbox"></param>
        /// <param name="SSLCertificate_FindByThumbprint"></param>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public DeliveryResult SendMessageToMailboxOperatorV3(SealedDelivery2 delivery, Mailbox mailbox, string SSLCertificate_FindByThumbprint, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.DataLayer.MailboxOperatorService: incoming SendMessageToMailboxOperatorV3 with RequestId: {0}", requestId));
            DeliveryResult result = null;

            var binding = new BasicHttpBinding()
            {
                Security = new BasicHttpSecurity()
                {
                    Transport = new HttpTransportSecurity()
                    {
                        ClientCredentialType = HttpClientCredentialType.Certificate
                    },
                    Mode = BasicHttpSecurityMode.Transport
                }
            };


            try
            {
                //MG Relight ändrade FindByThumbprint  X509FindType.FindBySubjectName

              var client = new ServicePortv3Client(binding, new EndpointAddress(mailbox.ServiceAdress));
                client.Endpoint.Behaviors.Add(new FaultFormatingBehavior());
 
                client.ClientCredentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, SSLCertificate_FindByThumbprint);
                result = client.deliverSecure(delivery);
            }
            catch (FaultException fe)
            {
                string errorMessage = string.Format("SE.GOV.MM.Integration.DeliveryMailbox.DataLayer.MailboxOperatorService: SOAPFAULT SendMessageToMailboxOperatorV3 with RequestId: {0}", requestId);
                LogManager.Log(new Log.Log() { EventId = EventId.CommunicationExceptionWithService, Exception = fe, Level = Level.Error, Message = errorMessage });
                throw fe;
            }
            catch (Exception e)
            {
                string errorMessage = string.Format("SE.GOV.MM.Integration.DeliveryMailbox.DataLayer.MailboxOperatorService: EXCEPTION in SendMessageToMailboxV3 with RequestId: {0}", requestId);
                LogManager.Log(new Log.Log() { EventId = EventId.CommunicationExceptionWithService, Exception = e, Level = Level.Error, Message = errorMessage });
                throw e;
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.DataLayer.MailboxOperatorService: leaving SendMessageToMailboxV3 with RequestId: {0}", requestId));

            return result;
        }
    }

    internal class FaultFormatingBehavior : IEndpointBehavior
    {
        public void Validate(ServiceEndpoint endpoint) { }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) { }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new FaultMessageInspector());
        }
    }

    public class FaultMessageInspector : IClientMessageInspector
    {
        public object BeforeSendRequest(ref Message request, IClientChannel channel) => null;

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.DataLayer.FaultMessageInspector: incoming AfterReceiveReply "));
            if (reply.IsFault)
            {
                MessageBuffer buffer = reply.CreateBufferedCopy(Int32.MaxValue);
                reply = buffer.CreateMessage();
                Message m = buffer.CreateMessage();
                var bodyContententsAsString = string.Empty;

                using (XmlDictionaryReader reader = m.GetReaderAtBodyContents())
                {
                    XmlDocument document = new XmlDocument();
                    document.Load(reader);

                    using (var stringWriter = new StringWriter())
                    {
                        using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                        {
                            document.WriteTo(xmlTextWriter);
                            xmlTextWriter.Flush();
                            bodyContententsAsString = stringWriter.GetStringBuilder().ToString();
                        }
                    }
                }

                LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.DataLayer.FaultMessageInspector: ReplyMessage: {0}", bodyContententsAsString));
            }
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.DataLayer.FaultMessageInspector: leaving AfterReceiveReply"));
        }
    }
}
