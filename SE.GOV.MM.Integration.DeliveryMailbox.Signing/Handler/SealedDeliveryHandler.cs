using SE.GOV.MM.Integration.DeliveryMailbox.Signing.Helper;
using SE.GOV.MM.Integration.Log;
using System;
using System.Security.Cryptography;

namespace SE.GOV.MM.Integration.DeliveryMailbox.Signing.Handler
{
    public class SealedDeliveryHandler
    {
        /// <summary>
        /// Handler that creates a SealedDelivery from a SignedDelivery.
        /// </summary>
        public SealedDelivery2 GetSealedDeliveryV3(SignedDelivery2 signedDelivery, bool signDelivery, string defaultNameSpaceV3, string signingCertificateSubjectName, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Handler.SealedDeliveryHandler: incoming GetSealedDeliveryV3 with RequestId: {0}", requestId));

            var sDelivery = new SealedDelivery2();

            sDelivery.Seal = new Seal();
            sDelivery.Seal.ReceivedTime = DateTime.Now;
            sDelivery.Seal.SignaturesOK = true;

            sDelivery.SignedDelivery = signedDelivery;

            //Check configfile to see if we should sign delivery.
            if (signDelivery)
            {
                try
                {
                    // Serialize SignedDelivery to XmlDocument
                    var serializeHelperV3 = new SerializeHelperV3();
                    var xmlDocument = serializeHelperV3.SerializeToXmlDocumentV3(sDelivery, defaultNameSpaceV3, requestId);

                    // Get signing certificate helper
                    var signingCertificateHelper = new SigningCertificateHelper();
                    var signXmlDocumentHandler = new SignXmlDocumentHandler();

                    // Sign xml document with certificate
                    var certificate = signingCertificateHelper.GetXMLSigningCertificate(signingCertificateSubjectName, requestId);
                    var signedXmlDocument = signXmlDocumentHandler.SignSealedDeliveryXmlDocument(xmlDocument, certificate, requestId);

                    // Deserialize signed xml document to SignedDelivery
                    sDelivery = serializeHelperV3.DeserializeXmlToSealedDeliveryV3(signedXmlDocument, defaultNameSpaceV3, requestId);
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

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.DeliveryMailbox.BusinessLayer.Handler.SignedDeliveryHandler: leaving GetSealedDeliveryV3 with RequestId: {0}", requestId));
            return sDelivery;
        }
    }
}
