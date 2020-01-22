using System;
using System.Security.Cryptography;
using SE.GOV.MM.Integration.Log;
using SE.GOV.MM.Integration.DeliveryMailbox.Signing.Helper;

namespace SE.GOV.MM.Integration.DeliveryMailbox.Signing.Handler
{
    public class SignedDeliveryHandler
    {
        /// <summary>
        /// Handler that creates a SignedDelivery from a SecureDelivery.
        /// </summary>
        /// 
        public SignedDelivery2 GetSignedDeliveryV3(SecureDelivery2 secureDelivery, bool signDelivery, string defaultNameSpace, string signingCertificateSubjectName, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Handler.SignedDeliveryHandler: incoming GetSignedDelivery with RequestId: {0}", requestId));

            var sDelivery = new SignedDelivery2();
            sDelivery.Delivery = secureDelivery;

            //Check configfile to see if we should sign delivery.
            if (signDelivery)
            {
                try
                {
                    // Serialize SignedDelivery to XmlDocument
                    var serializeHelper = new SerializeHelperV3();
                    var xmlDocument = serializeHelper.SerializeToXmlDocumentV3(sDelivery, defaultNameSpace, requestId);

                    // Get signing certificate helper
                    var signingCertificateHelper = new SigningCertificateHelper();
                    var signXmlDocumentHandler = new SignXmlDocumentHandler();

                    // Sign xml document with certificate
                    var certificate = signingCertificateHelper.GetXMLSigningCertificate(signingCertificateSubjectName, requestId);
                    var signedXmlDocument = signXmlDocumentHandler.SignSignedDeliveryXmlDocument(xmlDocument, certificate, requestId);

                    // Deserialize signed xml document to SignedDelivery
                    sDelivery = serializeHelper.DeserializeXmlToSignedDeliveryV3(signedXmlDocument, defaultNameSpace, requestId);
                }
                catch (CryptographicException ce)
                {
                    string errorMessage = string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Handler: Certification error. RequestId: {0}. ExceptionMessage: {1}", requestId, ce.Message);
                    LogManager.Log(new Log.Log() { Exception = ce, Message = errorMessage, EventId = EventId.XmlDocumentSigningException, Level = Level.Error });
                    throw new Exception(errorMessage);
                }
                catch (Exception e)
                {
                    string errorMessage = string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Handler: Exception have been thrown. RequestId: {0}. ExceptionMessage: {1}", requestId, e.Message);
                    LogManager.Log(new Log.Log() { EventId = EventId.GenerelizedException, Exception = e, Level = Level.Error, Message = errorMessage });
                    throw e;
                }
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Handler.SignedDeliveryHandler: leaving GetSignedDelivery with RequestId: {0}", requestId));
            return sDelivery;
        }
    }
}
