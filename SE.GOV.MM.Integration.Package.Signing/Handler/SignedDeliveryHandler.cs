using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SE.GOV.MM.Integration.Log;
using SE.GOV.MM.Integration.Package.Signing.Helper;

namespace SE.GOV.MM.Integration.Package.Signing.Handler
{
    public class SignedDeliveryHandler
    {
        
        /// <summary>
        /// Handler that creates a SignedDelivery from a SecureDelivery.
        /// </summary>
        public SignedDelivery1 GetSignedDeliveryV3(SecureDelivery1 secureDelivery, bool signDelivery, string defaultNameSpace, string signingCertificateSubjectName, Guid requestId)
        {
            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Handler.SignedDeliveryHandler: incoming GetSignedDeliveryV3 with RequestId: {0}", requestId));

            var sDelivery = new SignedDelivery1();
            sDelivery.Delivery = secureDelivery;

            //Check configfile to see if we should sign delivery.
            if (signDelivery)
            {
                try
                {
                    // Serialize SignedDelivery to XmlDocument
                    var serializeHelper = new SerializeHelper();
                    var xmlDocument = serializeHelper.SerializeToXmlDocumentV3(sDelivery, defaultNameSpace, requestId);

                    // Get signing certificate helper
                    var signingCertificateHelper = new SigningCertificateHelper();
                    var signXmlDocumentHandler = new SignXmlDocumentHandler();

                    // Sign xml document with certificate
                    var certificate = signingCertificateHelper.GetXMLSigningCertificate(signingCertificateSubjectName, requestId);
                    var signedXmlDocument = signXmlDocumentHandler.SignXmlDocument(xmlDocument, certificate, requestId);

                    // Deserialize signed xml document to SignedDelivery
                    sDelivery = serializeHelper.DeserializeXmlToSignedDeliveryV3(signedXmlDocument, defaultNameSpace, requestId);
                }
                catch (CryptographicException ce)
                {
                    string errorMessage = string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Handler: Certification error. RequestId: {0}. ExceptionMessage: {1}", requestId, ce.Message);
                    LogManager.Log(new Log.Log() { Exception = ce, Message = errorMessage, EventId = EventId.XmlDocumentSigningException, Level = Level.Error });
                    throw new Exception(errorMessage);
                }
                catch (Exception e)
                {
                    string errorMessage = string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Handler: Exception have been thrown. RequestId: {0}. ExceptionMessage: {1}", requestId, e.Message);
                    LogManager.Log(new Log.Log() { EventId = EventId.GenerelizedException, Exception = e, Level = Level.Error, Message = errorMessage });
                    throw e;
                }
            }

            LogManager.LogTrace(string.Format("SE.GOV.MM.Integration.Package.BusinessLayer.Handler.SignedDeliveryHandler: leaving GetSignedDeliveryV3 with RequestId: {0}", requestId));
            return sDelivery;
        }
    }
}
